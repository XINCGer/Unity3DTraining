using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace StatePattern
{
    /// <summary>
    /// 抽象的状态类
    /// </summary>
    abstract class State
    {
        public string stateName;
        public abstract void Handle(Context context);
    }


    class ConcreteStateA : State
    {
        public ConcreteStateA()
        {
            stateName = "状态A";
        }

        public override void Handle(Context context)
        {
            context.SetState(new ConcreteStateB());
        }
    }

    class ConcreteStateB : State
    {
        public ConcreteStateB()
        {
            stateName = "状态B";
        }

        public override void Handle(Context context)
        {
            context.SetState(new ConcreteStateA());
        }
    }

    class Context
    {
        private State state;

        public Context(State state)
        {
            this.state = state;
        }

        public void SetState(State state)
        {
            Console.WriteLine("当前状态为："+this.state.stateName);
            this.state = state;
            Console.WriteLine("状态改变为："+this.state.stateName);
            Console.WriteLine("================");
        }

        public void Request()
        {
            state.Handle(this);
        }
    }
}
