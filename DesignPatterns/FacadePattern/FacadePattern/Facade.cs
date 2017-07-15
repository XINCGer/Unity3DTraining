using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadePattern
{
    /// <summary>
    /// 外观类
    /// </summary>
    class Facade
    {
        private SubA subA;
        private SubB subB;
        private SubC subC;


        public Facade()
        {
            subA = new SubA();
            subB = new SubB();
            subC = new SubC();
        }

        //第一组方法
        public void MethodOne()
        {
            Console.WriteLine("执行第一组方法");
            subA.MethodA();
            subB.MethodB();
        }

        //第二组方法
        public void MethodTwo()
        {
            Console.WriteLine("执行第一组方法");
            subB.MethodB();
            subC.MethodC();
        }
    }
}
