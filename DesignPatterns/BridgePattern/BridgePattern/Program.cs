using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //举例
            Departments dp;
            dp = new Computer();
            dp.SetCourse(new AdvanceMath());
            dp.Select();

            dp = new Mathematics();
            dp.SetCourse(new MathAnalysis());
            dp.Select();

            //桥接模式
            Abstraction abstraction = new RedefineAbstraction();
            abstraction.SetImplementor(new ConcreteImplementorA());
            abstraction.Operation();

            abstraction.SetImplementor(new ConcreteImplementorB());
            abstraction.Operation();

        }
    }
}
