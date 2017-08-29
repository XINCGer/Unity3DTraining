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
}
