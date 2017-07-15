using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadePattern
{
    /// <summary>
    /// 子系统A
    /// </summary>
    class SubA
    {
        public void MethodA()
        {
            Console.WriteLine("子系统方法A被执行！");
        }
    }

    class SubB
    {
        public void MethodB()
        {
            Console.WriteLine("子系统方法B被执行！");
        }
    }

    class SubC
    {
        public void MethodC()
        {
            Console.WriteLine("子系统方法C被执行！");
        }
    }
}
