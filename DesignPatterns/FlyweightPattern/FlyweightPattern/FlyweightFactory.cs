using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    /// <summary>
    /// 享元工厂类
    /// </summary>
    class FlyweightFactory
    {
        private Dictionary<string, ConcreteFlyweight> flyweights = new Dictionary<string, ConcreteFlyweight>();

        public FlyweightFactory()
        {
            flyweights.Add("A", new ConcreteFlyweight());
            flyweights.Add("B", new ConcreteFlyweight());
            flyweights.Add("C", new ConcreteFlyweight());
            flyweights.Add("D", new ConcreteFlyweight());
        }

        public Flyweight GetFlyweight(string key)
        {
            if (flyweights.ContainsKey(key))
            {
                return flyweights[key];
            }
            return null;
        }
    }
}
