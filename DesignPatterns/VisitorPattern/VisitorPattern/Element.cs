using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{

    /// <summary>
    /// 抽象元素类
    /// </summary>
    abstract class Element
    {
        /// <summary>
        /// 抽象的接受访问的方法
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept(Visitor visitor);
    }

    class ConcreteElementA : Element
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitConcreteElementA(this);
        }

        /// <summary>
        /// 操作A
        /// </summary>
        public void OperationA()
        {
            
        }
    }

    class ConcreteElementB : Element
    {
        public override void Accept(Visitor visitor)
        {
            visitor.VisitConcreteElementB(this);
        }

        public void OperationB()
        {
            
        }
    }
}
