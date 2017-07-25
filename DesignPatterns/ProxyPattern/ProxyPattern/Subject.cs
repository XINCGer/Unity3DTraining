using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPattern
{
    abstract class Subject
    {
        public abstract void Request();
    }

    /// <summary>
    /// 真实类
    /// </summary>
    class RealSubject : Subject
    {
        public override void Request()
        {
            Console.WriteLine("Real Request");
        }
    }

    class Proxy : Subject
    {
        private RealSubject realSubject;

        public Proxy()
        {
            realSubject = new RealSubject();
        }

        public override void Request()
        {
            realSubject.Request();
        }
    }
}
