using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IteratorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcreteAggregate aggregate = new ConcreteAggregate();

            for (int i = 0; i < 10; i++)
            {
                aggregate.SetItems(i,"求职者"+(i+1));
            }
            Iterator iterator = aggregate.CreateIterator();
            while (!iterator.IsDone())
            {
                Console.WriteLine(iterator.CurrentItem()+"来我公司面试");
                iterator.Next();
            }
        }
    }
}
