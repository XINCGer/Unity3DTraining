using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Spy spy = new Spy();
            
            CountryA countryA = new CountryA("A",spy);
            CountryB countryB = new CountryB("B",spy);

            spy.Attach(countryA);
            spy.Attach(countryB);

            spy.SetIntelligence("研制武器！");

            spy.Notify();

            Subject subject = new ConcreteSubject();
            subject.Attach(new ConcreteObserver(subject,"1号观察者"));
            subject.Attach(new ConcreteObserver(subject,"2号观察者"));
            subject.Attach(new ConcreteObserver(subject,"3号观察者"));
            subject.SetState("Talking");
            subject.Notify();
        }
    }
}
