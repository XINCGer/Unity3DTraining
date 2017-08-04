using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern
{
    /// <summary>
    /// 抽象算法类
    /// </summary>
    abstract class ProduceStrategy
    {
        public abstract void Decision(int capital);
    }


    class ProduceStrategySummer:ProduceStrategy
    {
        public override void Decision(int capital)
        {
            Console.WriteLine("多生产冰激淋蛋糕");
        }
    }

    class ProduceStrategyWinter : ProduceStrategy
    {
        public override void Decision(int capital)
        {
            Console.WriteLine("多生产巧克力蛋糕");
        }
    }


    class ProduceContext
    {
        private ProduceStrategy produceStrategy;

        public ProduceContext(ProduceStrategy ps)
        {
            this.produceStrategy = ps;
        }

        public void GetDecision(int cap)
        {
            produceStrategy.Decision(cap);
        }
    }
}
