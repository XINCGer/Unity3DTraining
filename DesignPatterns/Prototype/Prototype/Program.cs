using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class Program
    {
        static void Main(string[] args)
        {
            //原型模式测试1
            ConcretePrototype1 p1 = new ConcretePrototype1("1");
            ConcretePrototype1 p2 = (ConcretePrototype1)p1.Clone();

            Console.WriteLine("p1: "+p1.Id);
            Console.WriteLine("p2: " + p2.Id);
            //原型模式测试1 END

        }
    }

}
