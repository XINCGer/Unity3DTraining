using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    /// <summary>
    /// 适配器类
    /// </summary>
    class Adapter:Target
    {
        //包含一个私有的Adaptee对象，所以本类为适配器
        private Adaptee adaptee = new Adaptee();

        //覆盖基类的Request方法
        public override void Requset()
        {
            adaptee.SpecificRequest();
        }
    }
}
