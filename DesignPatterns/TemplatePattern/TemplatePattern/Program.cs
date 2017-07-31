using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            MobilePhoneA mobilePhoneA = new MobilePhoneA();
            mobilePhoneA.PowerOn();
            mobilePhoneA.DialUp();
            mobilePhoneA.about();
            mobilePhoneA.PowerOff();

            MobilePhoneB mobilePhoneB = new MobilePhoneB();
            mobilePhoneB.PowerOn();
            mobilePhoneB.DialUp();
            mobilePhoneB.about();
            mobilePhoneB.PowerOff();
        }
    }
}
