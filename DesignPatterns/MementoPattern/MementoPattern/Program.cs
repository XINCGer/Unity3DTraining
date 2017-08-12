using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            Originator originator = new Originator();
            originator.SetState("开始");
            originator.Show();

            Caretaker caretaker = new Caretaker();
            caretaker.SetMemento(originator.CreateMemento());

            originator.SetState("停止");
            originator.Show();

            originator.SetMemento(caretaker.GetMemento());
            originator.Show();


        }
    }
}
