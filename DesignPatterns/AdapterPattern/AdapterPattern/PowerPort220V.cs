using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterPattern
{
    /// <summary>
    /// 220v电源接口类
    /// </summary>
    class PowerPort220V
    {
        /// <summary>
        /// 适配器提供电源
        /// </summary>
        public void PowerSupply()
        {
            Console.WriteLine("输出220电压！");
        }
    }


    class PowerPort110V
    {
        public void PowerSupply()
        {
            Console.WriteLine("输出110V电压！");
        }
    }
}
