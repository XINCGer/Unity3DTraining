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


            ProduceContext produceContext = new ProduceContext(new ProduceStrategySummer());
            produceContext.GetDecision(1);

            produceContext = new ProduceContext(new ProduceStrategyWinter());
            produceContext.GetDecision(1);

            ProduceContext ps = new ProduceContext(1);
            ps.GetDecision(1);

            ps = new ProduceContext(2);
            ps.GetDecision(1);

        }
    }
}
