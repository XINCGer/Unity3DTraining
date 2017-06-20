using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcreteComponent concreteComponent = new ConcreteComponent();
            //实例化两个装饰器
            ConcreteDecoratorA concreteDecoratorA = new ConcreteDecoratorA();
            ConcreteDecoratorB concreteDecoratorB = new ConcreteDecoratorB();

            //对concreteComponent对象进行装饰
            concreteDecoratorA.SetComponent(concreteComponent);
            concreteDecoratorB.SetComponent(concreteComponent);

            //显示装饰后的结果
            Console.WriteLine("经过装饰器A装饰后的操作：");
            concreteDecoratorA.Operation();
            Console.WriteLine("经过装饰器B装饰后的操作：");
            concreteDecoratorB.Operation();
        }
    }
}
