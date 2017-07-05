using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 100;
            FlyweightFactory factory = new FlyweightFactory();

            Flyweight flyA = factory.GetFlyweight("A");
            flyA.Operation(i);

            Flyweight flyB = factory.GetFlyweight("B");
            flyB.Operation(i * 2);

            Flyweight flyC = factory.GetFlyweight("C");
            flyC.Operation(i * 3);

            Flyweight flyD = factory.GetFlyweight("D");
            flyD.Operation(i * 4);

            Flyweight flyE = new UnsharedFlyweight();
            flyE.Operation(i / 2);

            Flyweight flyF = factory.GetFlyweight("A");
            flyF.Operation(i / 2);
        }
    }
}
