using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    abstract class Country
    {
        protected string countryName;
        protected Spy spy;

        public Country(string countryName, Spy spy)
        {
            this.countryName = countryName;
            this.spy = spy;
        }

        /// <summary>
        /// 获取情报抽象方法
        /// </summary>
        public abstract void Update();
    }

    class CountryA : Country
    {
        public CountryA(string countryName, Spy spy) : base(countryName, spy)
        {
        }

        public override void Update()
        {
           Console.WriteLine(countryName+"得到情报"+spy.GetIntelligence()+"建交");
        }
    }

    class CountryB : Country
    {
        public CountryB(string countryName, Spy spy) : base(countryName, spy)
        {
        }

        public override void Update()
        {
            Console.WriteLine(countryName+"得到情报"+spy.GetIntelligence()+"开战");
        }
    }
}
