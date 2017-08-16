using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorPattern
{
    /// <summary>
    /// 抽象的中介者类
    /// </summary>
    abstract class Mediator
    {
        public abstract void Send(string message, Colleague colleague);
    }

    /// <summary>
    /// 具体的中介者
    /// </summary>
    class ConcreteMediator : Mediator
    {
        private ColleagueA colleagueA;
        private ColleagueB colleagueB;

        public override void Send(string message, Colleague colleague)
        {
            if (colleague == colleagueA)
            {
                colleagueB.Notify(message);
            }
            else if (colleague == colleagueB)
            {
                colleagueA.Notify(message);
            }
        }

        public void SetColleagueA(ColleagueA colleagueA)
        {
            this.colleagueA = colleagueA;
        }

        public void SetColleagueB(ColleagueB colleagueB)
        {
            this.colleagueB = colleagueB;
        }
    }
}
