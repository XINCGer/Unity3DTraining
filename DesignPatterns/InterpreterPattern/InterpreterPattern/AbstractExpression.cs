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
    abstract class AbstractExpression
    {
        /// <summary>
        /// 抽象的解释方法
        /// </summary>
        /// <param name="context"></param>
        public abstract void Interpret(Context context);
    }

    /// <summary>
    /// 终端解释器，实现与文法中终结符相关的解释操作
    /// </summary>
    class TerminalExpression : AbstractExpression
    {
        public override void Interpret(Context context)
        {
            Console.WriteLine(context.InputString);
            Console.WriteLine("终端解释器");
        }
    }

    /// <summary>
    /// 非终端解释器
    /// </summary>
    class NonterminalExpression : AbstractExpression
    {
        public override void Interpret(Context context)
        {
            Console.WriteLine(context.InputString);
            Console.WriteLine("非终端解释器");
        }
    }
}
