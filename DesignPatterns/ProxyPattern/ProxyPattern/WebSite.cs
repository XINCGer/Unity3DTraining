using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPattern
{
    /// <summary>
    /// 网站类
    /// </summary>
    abstract class WebSite
    {
        /// <summary>
        /// 注册用户名
        /// </summary>
        public string userName;
        /// <summary>
        /// 注册用户
        /// </summary>
        public abstract void Register();
        /// <summary>
        /// 登录
        /// </summary>
        public abstract void Login();
        /// <summary>
        /// 浏览信息
        /// </summary>
        public abstract void Browse();
    }

    /// <summary>
    /// 某社交网站
    /// </summary>
    class SNS : WebSite
    {
        public override void Register()
        {
            Console.WriteLine(userName+"注册了社交网站");
        }

        public override void Login()
        {
            Console.WriteLine(userName + "登录了社交网站");
        }

        public override void Browse()
        {
            Console.WriteLine(userName + "浏览了社交网站");
        }
    }

    class WebsiteProxy : WebSite
    {
        private SNS sns;


        public WebsiteProxy(string name)
        {
            sns = new SNS();
            sns.userName = name;
        }

        public override void Register()
        {
            sns.Register();
        }

        public override void Login()
        {
            sns.Login();
        }

        public override void Browse()
        {
           sns.Browse();
        }
    }
}
