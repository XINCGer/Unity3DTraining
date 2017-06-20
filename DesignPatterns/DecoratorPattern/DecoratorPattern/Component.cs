using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    /// <summary>
    /// 组件抽象父类
    /// </summary>
    abstract class Component
    {
        /// <summary>
        /// 抽象的基本操作
        /// </summary>
        public abstract void Operation();
    }

    /// <summary>
    /// 具体组件类
    /// </summary>
    class ConcreteComponent : Component
    {
        /// <summary>
        /// 基本操作
        /// </summary>
        public override void Operation()
        {
            Console.WriteLine("基本操作！");
        }
    }
}
