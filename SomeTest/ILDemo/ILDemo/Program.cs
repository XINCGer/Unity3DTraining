using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDemo
{

    class Program
    {
        public static int n = 0;
        static void Main(string[] args)
        {
            Test test = new Test();
            test.Func1(Test.n);
            Console.WriteLine(Test.n);
            test.Func2(ref Test.n);
            Console.WriteLine(Test.n);

            Console.WriteLine("============");
            Func1(n);
            Console.WriteLine(n);
            Func2(ref n);
            Console.WriteLine(n);
        }

        public static void Func1(int n)
        {
            n += 5;
        }

        public static void Func2(ref int n)
        {
            n += 5;
        }
    }

    class Test
    {
        public static int n = 0;
        public void Func1(int n)
        {
            n += 5;
        }
        public void Func2(ref int n)
        {
            n += 5;
        }

    }

}
