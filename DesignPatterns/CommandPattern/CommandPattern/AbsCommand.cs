using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    /// <summary>
    /// 抽象命令
    /// </summary>
    abstract class AbsCommand
    {
        protected Reciver reciver;

        public AbsCommand(Reciver reciver)
        {
            this.reciver = reciver;
        }

        public abstract void Execute();
    }

    class ConcreteCommand : AbsCommand
    {
        public ConcreteCommand(Reciver reciver) : base(reciver)
        {
        }

        public override void Execute()
        {
            reciver.Action();
        }
    }
}
