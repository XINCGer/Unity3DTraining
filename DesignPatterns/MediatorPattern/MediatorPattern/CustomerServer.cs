using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorPattern
{
    /// <summary>
    /// 抽象的客服人员
    /// </summary>
    abstract class CustomerServer
    {
        public abstract void Communicate(string message, Communicator communicator);
    }

    class CustomerServericeMM : CustomerServer
    {
        private Customer customer;
        private Techician techician;


        public override void Communicate(string message, Communicator communicator)
        {
            if (communicator == customer)
            {
                techician.GetMessage(message);
            }
            else if (communicator == techician)
            {
                customer.GetMessage(message);
            }
        }

        public void SetCustomer(Customer customer)
        {
            this.customer = customer;
        }

        public void SetTechicain(Techician techician)
        {
            this.techician = techician;
        }
    }
}
