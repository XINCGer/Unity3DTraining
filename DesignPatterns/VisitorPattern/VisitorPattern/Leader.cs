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
        public abstract void VisitFinancialReport(FinancialReport report);
        public abstract void VisitLogisticsReport(LogisticsReport report);
    }

    /// <summary>
    /// 财务总监
    /// </summary>
    class FinanceDirector : Leader
    {
        public override void VisitFinancialReport(FinancialReport report)
        {
            int income = report.GetIncome() - report.GetExpenditure();
            Console.WriteLine("财务报表，总收入：" + income);
        }

        public override void VisitLogisticsReport(LogisticsReport report)
        {
            //不关心销售报表
        }
    }

    class SaleDirector:Leader
    {
        public override void VisitFinancialReport(FinancialReport report)
        {
            //不关心财务报表
        }

        public override void VisitLogisticsReport(LogisticsReport report)
        {
            int stock = report.GetProduction() - report.GetSale();
            Console.WriteLine("根据销售报表，库存为:"+stock);
        }
    }
}
