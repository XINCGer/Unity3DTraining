using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 演员类
    /// </summary>
    class Actor
    {
        public virtual void Act()
        {
            Console.WriteLine("演员开始表演了！");
        }
    }
}
