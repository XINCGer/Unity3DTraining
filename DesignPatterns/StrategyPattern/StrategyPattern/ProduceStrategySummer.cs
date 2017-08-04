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

        /// <summary>
        /// 加入简单工厂模式
        /// </summary>
        /// <param name="i"></param>
        public ProduceContext(int i)
        {
            switch (i)
            {
                case 1:
                    produceStrategy= new ProduceStrategySummer();
                    break;
                case 2:
                    produceStrategy = new ProduceStrategyWinter();
                    break;
                default:
                    Console.WriteLine("没有此种策略");
                    break;
            }
        }

        public void GetDecision(int cap)
        {
            produceStrategy.Decision(cap);
        }
    }
}
