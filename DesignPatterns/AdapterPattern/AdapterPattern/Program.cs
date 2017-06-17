using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //适配器基本代码测试
            Target target = new Target();
            target.Requset();

            //客户的特殊请求，调用Adaptee方法
            target = new Adapter();
            target.Requset();
        }
    }
}
