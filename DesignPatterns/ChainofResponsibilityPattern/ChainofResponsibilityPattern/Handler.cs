using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainofResponsibilityPattern
{
    abstract class Handler
    {
        protected Handler successor;

        /// <summary>
        /// 设定被授权者
        /// </summary>
        /// <param name="handler"></param>
        public void SetSuccesor(Handler handler)
        {
            this.successor = handler;
        }

        /// <summary>
        /// 抽象的处理请求方法
        /// </summary>
        /// <param name="requset"></param>
        public abstract void HandleRequest(int requset);
    }

    class ConcreteHandlerA : Handler
    {
        public override void HandleRequest(int requset)
        {
            if (1 == requset)
            {
                Console.WriteLine("ConcreteHandlerA处理了请求" + requset);
            }
            else
            {
                if (null != this.successor)
                {
                    Console.WriteLine("自身无法处理请求，转到下一个处理者");
                    this.successor.HandleRequest(requset);
                }
            }
        }
    }

    class ConcreteHandlerB : Handler
    {
        public override void HandleRequest(int requset)
        {
            if (2 == requset)
            {
                Console.WriteLine("ConcreteHandlerB处理了请求" + requset);
            }
            else
            {
                if (null != this.successor)
                {
                    Console.WriteLine("自身无法处理请求，转到下一个处理者");
                    this.successor.HandleRequest(requset);
                }
            }
        }
    }

    class ConcreteHandlerC : Handler
    {
        public override void HandleRequest(int requset)
        {
            if (3 == requset)
            {
                Console.WriteLine("ConcreteHandlerC处理了请求" + requset);
            }
            else
            {
                if (null != this.successor)
                {
                    Console.WriteLine("自身无法处理请求，转到下一个处理者");
                    this.successor.HandleRequest(requset);
                }
            }
        }
    }
}
