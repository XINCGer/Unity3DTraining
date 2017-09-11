using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterPattern
{
    /// <summary>
    /// 抽象表达式
    /// </summary>
    abstract class Expression
    {
        public void Interpret(Message message)
        {
            if (message.MessageText.Length == 0)
            {
                Console.WriteLine("空短信，不作处理");
            }
            else
            {
                Operate(message.MessageText);
            }
        }

        /// <summary>
        /// 抽象的操作方法
        /// </summary>
        /// <param name="message"></param>
        public abstract void Operate(string message);
    }

    /// <summary>
    /// 正常短信的解释器
    /// </summary>
    class NormalMessageExpression : Expression
    {
        public override void Operate(string message)
        {
            if (message.Contains("面试"))
            {
                Console.WriteLine("面试短信"+message);
            }
            else
            {
                Console.WriteLine("普通短信"+message);
            }
        }
    }

    /// <summary>
    /// 疑似垃圾短信的解释器
    /// </summary>
    class RubbishMessageExpression : Expression
    {
        public override void Operate(string message)
        {
            if (message.Contains("儿子"))
            {
                Console.WriteLine("家长发来的短信，不是骗子" + message);
            }
            else
            {
                Console.WriteLine("骗子短信，过滤掉");
            }
        }
    }

}
