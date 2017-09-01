using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    /// <summary>
    /// 抽象的领导类
    /// </summary>
    abstract class Leader
    {
        public abstract void VisitFinancialReport();
        public abstract void VisitLogisticsReport();
    }
}
