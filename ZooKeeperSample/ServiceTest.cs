using System;
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

            Console.WriteLine(id+" Connected");
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
        public void ZookeeperMutex() {
            WatcherWithDelegate wwd = null;
            wwd = new WatcherWithDelegate(p => {
                
            });
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
        }

    }
}
