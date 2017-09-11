using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterPattern
{
    /// <summary>
    /// 短信包
    /// </summary>
    class Message
    {
        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }
    }
}
