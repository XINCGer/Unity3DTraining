using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    interface IConporation
    {
        void Accept(IVisitor v);
    }

    class PartA : IConporation
    {
        private string s = "PartA";

        public void Accept(IVisitor v)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return s;
        }
    }

    class PartB:IConporation
    {
        private string s = "PartB";

        public void Accept(IVisitor v)
        {
            
        }

        public string GetName()
        {
            return s;
        }
    }

    class ObjectStructureA
    {
        private List<IConporation> list = new List<IConporation>();

        public void AttVisitor(IConporation a)
        {
            list.Add(a);
        }

        public void RemoveVisitor(IConporation a)
        {
            list.Remove(a);
        }

        public void Accept(IVisitor v)
        {
            foreach (var i in list)
            {
                i.Accept(v);
            }
        }
    }
}
