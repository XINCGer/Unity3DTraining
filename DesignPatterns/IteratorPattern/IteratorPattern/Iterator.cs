using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IteratorPattern
{
    /// <summary>
    /// 抽象的迭代器
    /// </summary>
    abstract class Iterator
    {
        public abstract Object First();
        public abstract Object Next();
        public abstract bool IsDone();
        public abstract Object CurrentItem();
    }

    class ConcreteIterator : Iterator
    {
        private ConcreteAggregate aggregate;
        private int index = 0;
        public ConcreteIterator(ConcreteAggregate aggregate)
        {
            this.aggregate = aggregate;
        }
        public override object First()
        {
            return this.aggregate.GetItem(0);
        }

        public override object Next()
        {
            Object obj = null;
            index++;
            if (index < this.aggregate.Size())
            {
                obj = this.aggregate.GetItem(index);
            }
            return obj;
        }

        public override bool IsDone()
        {
            if (index >= this.aggregate.Size())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object CurrentItem()
        {
            return this.aggregate.GetItem(index);
        }
    }
}
