using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZooKeeperSample
{
    class Program
    {
        static void Main(string[] args)
        {

            ManualResetEvent mre = new ManualResetEvent(false);
            mre.Reset();
            int i = 0;
            while (i < 30)
            {
                Action act = new Action(() =>
                {
                    mre.WaitOne();
                    ServiceTest st = new ServiceTest();
                    st.ZookeeperSimpleMutex();
                });
                i++;
                act.BeginInvoke(null, null);
            }
            mre.Set();
            while (Console.ReadLine() != "QUIT")
            {
                continue;
            };
        }
    }
}
