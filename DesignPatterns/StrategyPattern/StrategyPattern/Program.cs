using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context;
            context = new Context(new ConcreteStrategyA());
            context.ContextOperation();

            context= new Context(new ConcreteStrategyB());
            context.ContextOperation();

            context= new Context(new ConcreteStrategyC());
            context.ContextOperation();

        }
    }
}
