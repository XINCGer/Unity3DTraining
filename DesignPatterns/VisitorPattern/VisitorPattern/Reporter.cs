using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    /// <summary>
    /// 抽象报表
    /// </summary>
    abstract class Reporter
    {
        /// <summary>
        /// 抽象的接受访问方法
        /// </summary>
        /// <param name="leader"></param>
        public abstract void Accecpt(Leader leader);
    }
}
