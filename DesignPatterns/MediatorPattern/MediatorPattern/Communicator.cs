using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorPattern
{
    /// <summary>
    /// 抽象的交流者
    /// </summary>
    abstract class Communicator
    {
        protected CustomerServer customer;

        public Communicator(CustomerServer customerServer)
        {
            this.customer = customerServer;
        }
    }

    class Customer : Communicator
    {
        public Customer(CustomerServer customerServer) : base(customerServer)
        {
        }

        public void Ask(string message)
        {
            customer.Communicate(message,this);
        }

        public void GetMessage(string message)
        {
            Console.WriteLine("顾客得到咨询技术人员的回复：" + message);
        }
    }

    class Techician : Communicator
    {
        public Techician(CustomerServer customerServer) : base(customerServer)
        {
        }

        public void Answer(string message)
        {
            customer.Communicate(message, this);
        }

        public void GetMessage(string message)
        {
            Console.WriteLine("顾客资讯技术人员：" + message);
        }
    }
}
