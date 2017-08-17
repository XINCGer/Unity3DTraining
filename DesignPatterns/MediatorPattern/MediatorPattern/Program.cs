using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcreteMediator mediator = new ConcreteMediator();

            ColleagueA colleagueA = new ColleagueA(mediator);
            ColleagueB colleagueB = new ColleagueB(mediator);

            mediator.SetColleagueA(colleagueA);
            mediator.SetColleagueB(colleagueB);

            colleagueB.Send("Hello A");
            colleagueA.Send("Hello B");


            CustomerServericeMM customerMM = new CustomerServericeMM();
            Customer customer = new Customer(customerMM);
            Techician techician = new Techician(customerMM);

            customerMM.SetCustomer(customer);
            customerMM.SetTechicain(techician);

            customer.Ask("客户提问");
            techician.Answer("技术支持解答");
        }
    }
}
