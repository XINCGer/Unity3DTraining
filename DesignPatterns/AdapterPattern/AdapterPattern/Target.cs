using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    /// <summary>
    /// 客户期望的接口
    /// </summary>
    class Target
    {
        /// <summary>
        /// 客户的基本请求
        /// </summary>
        public virtual void Requset()
        {
            Console.WriteLine("客户端发起了普通基本请求！");
        }
    }
}
