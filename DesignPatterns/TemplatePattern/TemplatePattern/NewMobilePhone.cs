using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern
{
    abstract class NewMobilePhone
    {
        public virtual void PowerOn()
        {
            Console.WriteLine(User() + "手机开机了！");
        }

        public virtual void PowerOff()
        {
            Console.WriteLine(User() + "手机关机了！");
        }

        public virtual void DialUp()
        {
            Console.WriteLine(User() + "手机拨号！");
        }

        public virtual void about()
        {
            Console.WriteLine(User() + "关于手机");
        }

        protected abstract string User();
    }

    class NewMobilePhoneA : NewMobilePhone
    {
        protected override string User()
        {
            return "大老板";
        }
    }

    class NewMobilePhoneB : NewMobilePhone
    {
        protected override string User()
        {
            return "秘书";
        }
    }


    class AndroidMobilePhone : NewMobilePhone 
    {
        protected override string User()
        {
            return "Android系统手机";
        }
    }

    class WPMobilePhone:NewMobilePhone 
    {
        protected override string User()
        {
            return "WP系统手机";
        }
    }
}
