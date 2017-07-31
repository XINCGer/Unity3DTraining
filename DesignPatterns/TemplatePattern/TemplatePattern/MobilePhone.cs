using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern
{
    /// <summary>
    /// 抽象手机类
    /// </summary>
    abstract class MobilePhone
    {
        public virtual void PowerOn()
        {
            Console.WriteLine("手机开机了！");
        }

        public virtual void PowerOff()
        {
            Console.WriteLine("手机关机了！");
        }

        public virtual void DialUp()
        {
            Console.WriteLine("手机拨号！");
        }

        public virtual void about()
        {
            Console.WriteLine("关于手机");
        }
    }


    class  MobilePhoneA:MobilePhone
    {
        public override void about()
        {
            base.about();
            Console.WriteLine("A");
        }
    }

    class MobilePhoneB : MobilePhone
    {
        public override void about()
        {
            base.about();
            Console.WriteLine("B");
        }
    }
}
