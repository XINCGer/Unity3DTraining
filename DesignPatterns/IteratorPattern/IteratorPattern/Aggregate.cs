using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IteratorPattern
{
    /// <summary>
    /// 聚集的抽象
    /// </summary>
    abstract class Aggregate
    {
        public abstract Iterator CreateIterator();
    }

    class ConcreteAggregate : Aggregate 
    {
        private List<Object> items = new List<object>();

        public override Iterator CreateIterator()
        {
            
        }

        public int Size()
        {
            return items.Count;
        }

        public Object GetItem(int i)
        {
            return items[i];
        }

        public void SetItems(int i,Object obj)
        {
            items[i] = obj;
        }
    }
}
