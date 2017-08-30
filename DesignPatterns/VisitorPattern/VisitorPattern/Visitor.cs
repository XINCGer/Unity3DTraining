using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    /// <summary>
    /// 抽象访问者
    /// </summary>
    abstract class Visitor
    {
        public abstract void VisitConcreteElementA(ConcreteElementA element);
        public abstract void VisitConcreteElementB(ConcreteElementB element);
    }

    class ConcreteVisitor : Visitor
    {
        public override void VisitConcreteElementA(ConcreteElementA element)
        {
            Console.WriteLine("元素A被访问者访问");
        }

        public override void VisitConcreteElementB(ConcreteElementB element)
        {
            Console.WriteLine("元素B被访问者访问");
        }
    }
}
