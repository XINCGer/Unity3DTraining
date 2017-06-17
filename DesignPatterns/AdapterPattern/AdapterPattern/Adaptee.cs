using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    /// <summary>
    /// 需要被适配的类
    /// </summary>
    class Adaptee
    {
        /// <summary>
        /// 客户的特殊请求
        /// </summary>
        public void SpecificRequest()
        {
            Console.WriteLine("客户发起了特殊请求！");
        }
    }
}
