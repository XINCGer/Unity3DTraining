using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterPattern
{
    /// <summary>
    /// 信息包
    /// </summary>
    class Context
    {
        private string inputString;
        private string outputString;

        public string InputString
        {
            get { return inputString; }
            set { inputString = value; }
        }

        public string OutputString
        {
            get { return outputString; }
            set { outputString = value; }
        }
    }
}
