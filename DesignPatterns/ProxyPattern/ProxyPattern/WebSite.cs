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
}
