using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadePattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //外观模式代码
            Facade facade = new Facade();
            facade.MethodOne();
            facade.MethodTwo();
        }
    }
}
