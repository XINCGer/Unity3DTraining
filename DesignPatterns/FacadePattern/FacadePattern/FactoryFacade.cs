using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadePattern
{
    /// <summary>
    /// 工厂办事处
    /// </summary>
    class FactoryFacade
    {
        CreamFactory creamFactory;
        HoneyFactory honeyFactory;

        //建造工厂
        public FactoryFacade()
        {
            creamFactory = new CreamFactory();
            honeyFactory = new HoneyFactory();
        }

        /// <summary>
        /// 管理工厂配送
        /// </summary>
        public void FactoryDelivery()
        {
            creamFactory.CreamDelivery();
            honeyFactory.HoneyDelivery();
        }

        /// <summary>
        /// 管理工厂汇报
        /// </summary>
        public void FactoryReport()
        {
            creamFactory.CreamReport();
            honeyFactory.HoneyReport();
        }
    }
}
