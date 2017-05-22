using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern
{
    /// <summary>
    /// 客户端代码
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Director director = new Director();
            Builder builderA = new ConcreteBuliderA();
            Builder builderB = new ConcreteBuilderB();

            //指挥者建造A
            director.Construct(builderA);
            Product productA = builderA.GetResult();
            productA.Show();

            //指挥者建造B
            director.Construct(builderB);
            Product productB = builderB.GetResult();
            productB.Show();


        }
    }
}
