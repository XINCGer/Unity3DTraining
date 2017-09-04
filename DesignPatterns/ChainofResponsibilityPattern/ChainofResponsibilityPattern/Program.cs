using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainofResponsibilityPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            Handler ha = new ConcreteHandlerA();
            Handler hb = new ConcreteHandlerB();
            Handler hc = new ConcreteHandlerC();

            ha.SetSuccesor(hb);
            hb.SetSuccesor(hc);

            for (int i = 1; i < 4; i++)
            {
                ha.HandleRequest(i);
            }
        }
    }
}
