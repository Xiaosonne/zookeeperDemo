﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;
using static ZooKeeperNet.ZooKeeper;

namespace ZooKeeperSample
{
    public class WatcherWithDelegate : IWatcher
    {
        Action<WatchedEvent> process;
        public WatcherWithDelegate(Action<WatchedEvent> proce)
        {
            process = proce;
        }
        public void Process(WatchedEvent @event)
        {
            process.Invoke(@event);
        }
    }
    public class ServiceTest : IWatcher
    {
        public void Process(WatchedEvent @event)
        {
            Console.WriteLine(@event);
        }
        ZooKeeper zk = null;
        public void ZookeeperSimpleMutex()
        {
            zk = new ZooKeeper("127.0.0.1:2181", TimeSpan.FromMinutes(3), null);
            string line = "";
            ManualResetEvent mre = new ManualResetEvent(false);
            Action act = (() =>
            {
                while (zk.State != States.CONNECTED)
                {
                    Console.WriteLine("Loading");
                    Thread.Sleep(300);
                }
                mre.Set();
            });
            act.BeginInvoke(null, null);
            mre.WaitOne();
            string id = Guid.NewGuid().ToString();

            Console.WriteLine(id + " Connected");
            bool getLock = false;
            WatcherWithDelegate wwd = null;
            wwd = new WatcherWithDelegate(p =>
             {
                 //有进程成功创建了一个锁
                 if (p.Type == EventType.NodeCreated)
                 {
                     var m = zk.GetData("/zookeeper/mylock", false, null);
                     string id1 = Encoding.UTF8.GetString(m);
                     //成功的抢到了锁 哈哈哈
                     if (id1 == id)
                     {
                         Console.WriteLine(id + " get lock");
                         getLock = true;
                     }
                     //但是 不是我 (⊙o⊙)？ What？ 继续观察
                     else
                     {
                         zk.Exists("/zookeeper/mylock", wwd);
                     }
                 }
             });
            var stat = zk.Exists("/zookeeper/mylock", wwd);
            //此处太浪费资源 
            while (!getLock)
            {
                try
                {
                    zk.Create("/zookeeper/mylock", Encoding.UTF8.GetBytes(id), Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
                    Console.WriteLine(id + " wait for lock");
                    break;
                }
                catch
                {

                }
            }
            Console.WriteLine(id + " wait for input anything to shutdown");
            Thread.Sleep(300);
            Console.WriteLine(id + " shut down");
            zk.Dispose();
        }
        public void Test()
        {
            zk = new ZooKeeper("127.0.0.1:2181", TimeSpan.FromMinutes(3), this);
            string line = "";
            ManualResetEvent mre = new ManualResetEvent(false);
            Action act = (() =>
            {
                while (zk.State != States.CONNECTED)
                {
                    Console.WriteLine("Loading");
                    Thread.Sleep(300);
                }
                mre.Set();
            });
            act.BeginInvoke(null, null);
            mre.WaitOne();
            Console.WriteLine("zookeeper connected");
            IEnumerable<string> paths = null;
            int i = 0;
            do
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "c":
                        {
                            string path = Console.ReadLine();
                            string data = Console.ReadLine();
                            var stat = zk.Exists(path, this);
                            var str = zk.Create(path, Encoding.UTF8.GetBytes(data), Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                            Console.WriteLine(str);
                            continue; ;
                        }
                    case "s":
                        {
                            string path = Console.ReadLine();
                            string data = Console.ReadLine();
                            var acls = zk.GetACL(path, new Stat());
                            zk.Exists(path, new WatcherWithDelegate(p =>
                            {
                                Console.WriteLine(" zk.Exists " + p);
                            }));
                            //var data1 = zk.GetData(path, new WatcherWithDelegate(p =>
                            //  {
                            //      Console.WriteLine("zk.GetData" + p);
                            //  }), null);
                            zk.SetData(path, Encoding.UTF8.GetBytes(data), -1);
                            continue;
                        }
                    default:
                        line = Console.ReadLine();
                        paths = zk.GetChildren(line, this);
                        foreach (var item in paths)
                        {
                            Console.WriteLine(item);
                        }
                        paths = zk.GetChildren(line, true);

                        foreach (var item in paths)
                        {
                            Console.WriteLine(item);
                        }
                        paths = zk.GetChildren(line, false);

                        foreach (var item in paths)
                        {
                            Console.WriteLine(item);
                        }
                        continue;
                }

            } while (line != "quit");


        }
        public void ZookeeperMutex()
        {
            WatcherWithDelegate wwd = null;

            zk = new ZooKeeper("127.0.0.1:2181", TimeSpan.FromSeconds(60), null);
            string line = "";
            ManualResetEvent mre = new ManualResetEvent(false);
            Action act = (() =>
            {
                while (zk.State != States.CONNECTED)
                { 
                    Thread.Sleep(300);
                }
                mre.Set();
            });
            act.BeginInvoke(null, null);
            mre.WaitOne();
            mre.Reset();
            string nodeData = Guid.NewGuid().ToString();
            string myId = zk.Create("/zookeeper/locks/mylock", Encoding.UTF8.GetBytes(nodeData), Ids.OPEN_ACL_UNSAFE, CreateMode.EphemeralSequential); 
            long lid = getSequcentialId(myId); 
            Console.WriteLine(myId + " created ");
            //获取zookeeper分配的序列号
            //当前进程是第一个锁时 证明持有该互斥锁
            if (lid == 0 )
            {
                //模拟处理程序
                Thread.Sleep(10000);
                //释放当前锁
                zk.Dispose();
                Console.WriteLine(myId + " release lock");
                return;
            }
            else
            { 
                IEnumerable<string> childs = null;
                //获取上一个可能持锁进程节点
                string littleId = "/zookeeper/locks/mylock" + string.Format("{0:D10}", lid - 1);
                WatcherWithDelegate wwd2 = null;
                
                wwd2 = new WatcherWithDelegate(p =>
                {
                    //之前持锁进程已经结束
                    //本进程可以执行
                    if (p.Type == EventType.NodeDeleted && p.Path == littleId)
                    {
                        Console.WriteLine(p.Path + " is deleted");
                        Console.WriteLine(myId + " get lock ");
                        mre.Set();
                    }
                    else//继续监听
                    {
                        zk.Exists(littleId, wwd2);
                    }
                });
                //开始监听上一持锁对象
                var stat = zk.Exists(littleId, wwd2);
                try
                {
                    childs = zk.GetChildren("/zookeeper/locks", false, null);
                    var mmm = "/zookeeper/locks/" + childs?.OrderBy(p => p).FirstOrDefault();
                    if (string.Equals(mmm, myId))
                    {
                        zk.Dispose();
                        Console.WriteLine("first dispose :{0}", myId);
                        return;
                    }
                }
                catch
                {
                }

                mre.WaitOne();
                Console.WriteLine(myId + " doing something");
                Thread.Sleep(10000);
                Console.WriteLine(myId + " release lock");
                zk.Dispose();
                Console.WriteLine(myId + " release lock2");

            }
        }

        private static long getSequcentialId(string id1)
        {
            string num = id1.TrimStart("/zookeeper/locks/mylock".ToCharArray());
            long lid = long.Parse(num);
            return lid;
        }
    }
}
