using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorPattern
{
    /// <summary>
    /// 抽象同事类
    /// </summary>
    abstract class Colleague
    {
        protected Mediator mediator;

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public abstract void Notify(string message);
        public abstract void Send(string message);
    }


    class ColleagueA : Colleague
    {
        public ColleagueA(Mediator mediator) : base(mediator)
        {
        }

        public override void Notify(string message)
        {
            Console.WriteLine("A收到消息 "+message);
        }

        public override void Send(string message)
        {
           mediator.Send(message,this);
        }
    }

    class ColleagueB : Colleague
    {
        public ColleagueB(Mediator mediator) : base(mediator)
        {
        }

        public override void Notify(string message)
        {
            Console.WriteLine("B收到消息 "+message);
        }

        public override void Send(string message)
        {
            mediator.Send(message, this);
        }
    }
}
