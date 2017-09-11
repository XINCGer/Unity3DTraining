using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context =  new Context();
            context.InputString = "输入的文字";
            List<AbstractExpression> list = new List<AbstractExpression>();
            list.Add(new TerminalExpression());
            list.Add(new NonterminalExpression());
            list.Add(new TerminalExpression());

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Interpret(context);
            }
        }
    }
}
