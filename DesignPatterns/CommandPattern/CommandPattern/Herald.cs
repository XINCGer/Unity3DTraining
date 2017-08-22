using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    /// <summary>
    /// 传令官代码
    /// </summary>
    class Herald
    {
        private Command command;

        /// <summary>
        /// 接受命令
        /// </summary>
        /// <param name="command"></param>
        public void SetCommand(Command command)
        {
            this.command = command;
        }

        /// <summary>
        /// 传令下去
        /// </summary>
        public void Notify()
        {
            command.ExcuteCommand();
        }
    }
}
