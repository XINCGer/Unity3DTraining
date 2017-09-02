using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    interface IVisitor
    {
        void visit(PartA partA);
        void visit(PartB partB);
    }

    class A : IVisitor
    {
        public void visit(PartA partA)
        {
            Console.WriteLine("A主管"+partA.GetName());
        }

        public void visit(PartB partB)
        {
            //不关心B
        }
    }

    class B : IVisitor
    {
        public void visit(PartA partA)
        {
            //不关心A
        }

        public void visit(PartB partB)
        {
            Console.WriteLine("B主管"+partB.GetName());
        }
    }


    class CEO : IVisitor
    {
        public void visit(PartA partA)
        {
            Console.WriteLine("CEO主管"+partA.GetName());
        }

        public void visit(PartB partB)
        {
            Console.WriteLine("CEO主管"+partB.GetName());
        }
    }
}
