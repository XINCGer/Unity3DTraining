using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    /// <summary>
    /// 享元类接口
    /// </summary>
    public abstract class Flyweight
    {
        public abstract void Operation(int i);
    }

    /// <summary>
    /// 共享的具体的享元类
    /// </summary>
    class ConcreteFlyweight : Flyweight
    {
        public override void Operation(int i)
        {
            Console.WriteLine("具体的享元类对象： "+i);
        }
    }

    /// <summary>
    /// 不共享的享元类
    /// </summary>
    class UnsharedFlyweight : Flyweight
    {
        public override void Operation(int i)
        {
            Console.WriteLine("不共享的享元类对象： "+i);
        }
    }
}
