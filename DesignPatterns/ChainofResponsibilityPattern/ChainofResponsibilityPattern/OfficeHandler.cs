using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChainofResponsibilityPattern
{
    /// <summary>
    /// 抽象的裁决者
    /// </summary>
    abstract class OfficeHandler
    {
        protected OfficeHandler handler;

        /// <summary>
        /// 设定裁决者
        /// </summary>
        /// <param name="handler"></param>
        public void SetHandler(OfficeHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// 抽象的处理方法
        /// </summary>
        /// <param name="count"></param>
        public abstract void HandlerReqest(int count);
    }

    /// <summary>
    /// 钦差大臣类
    /// </summary>
    class Prefects : OfficeHandler
    {
        public override void HandlerReqest(int count)
        {
            if (count < 5)
            {
                Console.WriteLine("小于5个直接处理");
            }
            else
            {
                if (this.handler != null)
                {
                    Console.WriteLine("大于5个，交给皇上处理");
                    handler.HandlerReqest(count);
                }
            }
        }
    }

    class Emperor : OfficeHandler
    {
        public override void HandlerReqest(int count)
        {
            if (count == 5)
            {
                Console.WriteLine("处理4个，带回一个");
            }
            else if (count == 6)
            {
                Console.WriteLine("处理5个，带回一个");
            }
            else if (count == 7)
            {
                Console.WriteLine("处理5个，带回两个");
            }
            else
            {
                if (this.handler != null)
                {
                    Console.WriteLine("交给下一个处理者");
                    this.handler.HandlerReqest(count);
                }
            }
        }
    }
}
