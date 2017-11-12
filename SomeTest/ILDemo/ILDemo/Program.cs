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

}
