using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Task创建
            //var task = new Task(() =>
            //{
            //    Console.WriteLine("Hello,Task1");
            //});
            //task.Start();

            //var task1 = Task.Factory.StartNew(() =>
            //{
            //    Console.WriteLine("Hello Task2 created by factory");
            //});
            //Console.ReadKey();

            //Task状态
            //var task1 = new Task(() =>
            //{
            //    Console.WriteLine("Begin");
            //    System.Threading.Thread.Sleep(2000);
            //    Console.WriteLine("Finish");
            //});
            //Console.WriteLine("Before start:" + task1.Status);
            //task1.Start();
            //Console.WriteLine("After start:" + task1.Status);
            //task1.Wait();
            //Console.WriteLine("After Finish:" + task1.Status);

            //Console.Read();
            
            //Task Wait
            var task1 = new Task(() =>
            {
                Console.WriteLine("Task 1 Begin");
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine("Task 1 Finish");
            });
            var task2 = new Task(() =>
            {
                Console.WriteLine("Task 2 Begin");
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("Task 2 Finish");
            });

            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);
            Console.WriteLine("All task finished!");

            Console.Read();
        }
    }
}
