using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgePattern
{
    //数学课程
    abstract class Math
    {
        public abstract void Select();
    }

    //数学课程的具体类代码:

    /// <summary>
    /// 数学分析
    /// </summary>
    class MathAnalysis : Math
    {
        public override void Select()
        {
            Console.WriteLine("选择了数学分析");
        }
    }

    class AdvanceMath : Math
    {
        public override void Select()
        {
            Console.WriteLine("选择了高等数学");
        }
    }
}
