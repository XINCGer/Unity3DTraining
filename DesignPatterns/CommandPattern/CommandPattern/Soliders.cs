using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    /// <summary>
    /// 士兵类
    /// </summary>
    class Soliders
    {
        /// <summary>
        /// 集合
        /// </summary>
        public void Combinate()
        {
            Console.WriteLine("集合士兵");
        }

        /// <summary>
        /// 战斗
        /// </summary>
        public void Fight()
        {
            Console.WriteLine("士兵战斗");
        }

        /// <summary>
        /// 铁索连舟
        /// </summary>
        public void CableBoat()
        {
            Console.WriteLine("铁索连舟");
        }
    }
}
