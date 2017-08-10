using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    /// <summary>
    /// 抽象的观察者
    /// </summary>
    abstract class Observer
    {
        public abstract void Update();
    }

    class ConcreteObserver : Observer
    {
        private string observerName;
        private string State;
        private Subject subject;


        public Subject GetSubject()
        {
            return this.subject;
        }

        public void SetSubject(Subject subject)
        {
            this.subject = subject;
        }

        public ConcreteObserver(Subject subject, string observerName)
        {
            this.subject = subject;
            this.observerName = observerName;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
