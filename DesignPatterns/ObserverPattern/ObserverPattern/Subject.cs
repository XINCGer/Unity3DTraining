using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern
{
    abstract class Subject
    {
        protected string state;

        private List<Observer> listeners =new List<Observer>();

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="observer"></param>
        public void Attach(Observer observer)
        {
            listeners.Add(observer);
        }

        //反注册
        public void Detch(Observer observer)
        {
            listeners.Remove(observer);
        }

        public void Notify()
        {
            foreach (var listener in listeners)
            {
                listener.Update();
            }
        }

        public abstract void SetState(string state);
        public abstract string GetState();
    }

    class ConcreteSubject : Subject
    {
        public override void SetState(string state)
        {
            this.state = state;
        }

        public override string GetState()
        {
            return this.state;
        }
    }
}
