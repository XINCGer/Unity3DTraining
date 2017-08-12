using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern
{
    class Originator
    {
        private string state;

        /// <summary>
        /// 建立状态存储对象
        /// </summary>
        /// <returns></returns>
        public Memento CreateMemento()
        {
            return new Memento(state);
        }

        /// <summary>
        /// 设置状态存储对象
        /// </summary>
        /// <param name="memento"></param>
        public void SetMemento(Memento memento)
        {
            state = memento.GetState();
        }

        public void Show()
        {
            Console.WriteLine("状态为："+state);
        }

        public string GetState()
        {
            return this.state;
        }

        public void SetState(string state)
        {
            this.state = state;
        }
    }


    /// <summary>
    /// 备忘录类
    /// </summary>
    class Memento
    {
        private string state;

        public Memento(string state)
        {
            this.state = state;
        }

        public string GetState()
        {
            return this.state;
        }
    }

    /// <summary>
    /// 管理者类
    /// </summary>
    class Caretaker
    {
        private Memento memento;

        public void SetMemento(Memento memento)
        {
            this.memento = memento;
        }

        public Memento GetMemento()
        {
            return this.memento;
        }
    }
}
