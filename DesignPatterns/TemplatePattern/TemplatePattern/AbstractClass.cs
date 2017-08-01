using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TemplatePattern
{
    /// <summary>
    /// 模板方法的基本代码
    /// </summary>


    //抽象模板类
    abstract class AbstractClass
    {
        public abstract void PrimitOpA();
        public abstract void PrimitOpB();
        public abstract void PrimitOpC();

        /// <summary>
        /// 模板方法
        /// </summary>
        public void TemplateMethod()
        {
            PrimitOpA();
            PrimitOpB();
            PrimitOpC();
        }
    }


    class ConcreteClassA : AbstractClass
    {
        public override void PrimitOpA()
        {
            Console.WriteLine("具体类A操作A");
        }

        public override void PrimitOpB()
        {
            Console.WriteLine("具体类A操作B");
        }

        public override void PrimitOpC()
        {
            Console.WriteLine("具体类A操作C");
        }
    }


    class ConcreteClassB : AbstractClass
    {
        public override void PrimitOpA()
        {
            Console.WriteLine("具体类B操作A");
        }

        public override void PrimitOpB()
        {
            Console.WriteLine("具体类B操作B");
        }

        public override void PrimitOpC()
        {
            Console.WriteLine("具体类B操作C");
        }
    }
}
