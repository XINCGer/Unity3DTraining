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


            Expression expression;
            Message message1 = new Message();
            Message message2 = new Message();
            Message message3 = new Message();

            message1.MessageText = "你好，请来参加面试";
            message2.MessageText = "儿子，家里给你汇款了";
            message3.MessageText = "还记得我是老王嘛，给我汇款到123456";

            List<Message> messages = new List<Message>();
            messages.Add(message1);
            messages.Add(message2);
            messages.Add(message3);

            for (int i = 0; i < messages.Count; i++)
            {
                Message tmpMessage = messages[i];
                if (tmpMessage.MessageText.Contains("汇款"))
                {
                    expression = new RubbishMessageExpression();
                    expression.Interpret(tmpMessage);
                }
                else
                {
                    expression = new NormalMessageExpression();
                    expression.Interpret(tmpMessage);
                }
            }
            
        }
    }
}
