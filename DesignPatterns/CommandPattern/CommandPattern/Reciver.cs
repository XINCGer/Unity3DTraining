using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    /// <summary>
    /// 最终命令的执行者
    /// </summary>
    class Reciver
    {
        public void Action()
        {
            Console.WriteLine("执行命令！");
        }
    }
}
