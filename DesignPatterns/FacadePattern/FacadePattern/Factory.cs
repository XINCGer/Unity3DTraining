using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadePattern
{

    /// <summary>
    /// 奶油工厂
    /// </summary>
    class CreamFactory
    {
        //配送奶油
        public void CreamDelivery()
        {
            Console.WriteLine("奶油工厂配送奶油！");
        }

        //汇报工作
        public void CreamReport()
        {
            Console.WriteLine("奶油工厂汇报工作！");
        }
    }

    class HoneyFactory
    {
        //配送蜂蜜
        public void HoneyDelivery()
        {
            Console.WriteLine("蜂蜜工厂配送蜂蜜！");
        }

        //汇报工作
        public void HoneyReport()
        {
            Console.WriteLine("蜂蜜工厂汇报工作！");
        }
    }
}
