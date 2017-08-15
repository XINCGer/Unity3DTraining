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

            GameRole gameRole = new GameRole();
            gameRole.Init();
            Console.WriteLine("===========");
            gameRole.ShowState();

            GameStateCaretaker caretakerA = new GameStateCaretaker();
            caretakerA.SetGameState(gameRole.SaveState());

            Console.WriteLine("===========");
            gameRole.DoTask();
            gameRole.ShowState();

            gameRole.Recover(caretakerA.GetGameState());
            gameRole.ShowState();
        }
    }
}
