using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOATest;
using static SOATest.HelloWorldService;

namespace ZooKeeperSample.SOATest
{
    public class ServiceImp : Iface
    {
        public int calculate(Work work)
        {
            switch (work.Op)
            {
                case Operation.ADD:
                    return work.Num1 + work.Num2;
                case Operation.SUBSTRACT:
                    return work.Num1 - work.Num2;

                case Operation.MULTIPLY:
                    return work.Num1 * work.Num2;
                case Operation.DIVEDE:
                    return work.Num1 / work.Num2;
            }
            return 0;
        }

        public string sayHello(string username)
        {
            Console.WriteLine("ServiceImp.sayHello." + username);
            return "hello " + username;
        }
    }
}
