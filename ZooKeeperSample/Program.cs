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
            //ServiceTest st = new ServiceTest();
            //st.ZookeeperMutex();
            int i = 0;
            while (i < 5)
            {
                Action act = new Action(() =>
                {
                    ServiceTest st = new ServiceTest();
                    st.ZookeeperMutex();
                });
                i++;
                act.BeginInvoke(null, null);
            }
            while (Console.ReadLine() != "QUIT")
            {
                continue;
            };
        }
    }
}
