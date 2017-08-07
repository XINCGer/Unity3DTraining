using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatePattern
{
    /// <summary>
    /// 火车类
    /// </summary>
    class Train
    {
        private TrainState state;

        public Train()
        {
            state = 
        }

        public int GetSpeed()
        {
            return 1;
        }

        public void SetState(TrainState state)
        {
            
        }

        public void Run()
        {
            
        }
    }

    /// <summary>
    /// 抽象状态
    /// </summary>
    abstract class TrainState
    {
        public abstract void Run(Train train);
    }

    class StartState : TrainState
    {
        public override void Run(Train train)
        {
            if (train.GetSpeed() == 0)
            {
                Console.WriteLine("火车启动");
            }
            else
            {
                train.SetState(new FastState());
                train.Run();
            }
        }
    }

    class FastState : TrainState
    {
        public override void Run(Train train)
        {
            if (train.GetSpeed() < 200)
            {
                Console.WriteLine("火车加速");
            }
            else
            {
                train.SetState(new SlowState());
            }
        }
    }

    class SlowState : TrainState
    {
        public override void Run(Train train)
        {
            throw new NotImplementedException();
        }
    }

    class RunState : TrainState
    {
        public override void Run(Train train)
        {
            throw new NotImplementedException();
        }
    }
}
