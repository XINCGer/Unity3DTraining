using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Proxy proxy = new Proxy();
            proxy.Request();


            WebsiteProxy websiteProxy = new WebsiteProxy("马三");
            websiteProxy.Register();
            websiteProxy.Login();
            websiteProxy.Browse();
        }
    }
}
