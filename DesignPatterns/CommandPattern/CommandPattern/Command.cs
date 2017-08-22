using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    /// <summary>
    /// 抽象的命令
    /// </summary>
    abstract class Command
    {
        protected Soliders soliders;

        public Command(Soliders soliders)
        {
            this.soliders = soliders;
        }

        public abstract void ExcuteCommand();
    }

    //对应士兵的每种行为，设计不同的具体命令类

    /// <summary>
    /// 集合命令
    /// </summary>
    class CombinateCommand : Command
    {
        public CombinateCommand(Soliders soliders) : base(soliders)
        {
        }

        public override void ExcuteCommand()
        {
            soliders.Combinate();
        }
    }

    /// <summary>
    /// 战斗命令
    /// </summary>
    class FightCommand : Command
    {
        public FightCommand(Soliders soliders) : base(soliders)
        {
        }

        public override void ExcuteCommand()
        {
            soliders.Fight();
        }
    }

    /// <summary>
    /// 铁索连舟命令
    /// </summary>
    class CableBoatCommand:Command
    {
        public CableBoatCommand(Soliders soliders) : base(soliders)
        {
        }

        public override void ExcuteCommand()
        {
            soliders.CableBoat();
        }
    }
}
