using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread thread = new Thread(Go);
            //thread.Start();
            //thread.Join();
            //Console.WriteLine("Thread is end!");

            Thread thread = new Thread(()=> { Console.ReadKey(); });
            if (args.Length > 0)
            {
                thread.IsBackground = true;
            }
            thread.Start();
        }

        private static void Go()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("y");
            }
        }
    }


}
