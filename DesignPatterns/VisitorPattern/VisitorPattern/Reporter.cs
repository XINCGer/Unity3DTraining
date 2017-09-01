using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    /// <summary>
    /// 抽象报表
    /// </summary>
    abstract class Reporter
    {
        /// <summary>
        /// 抽象的接受访问方法
        /// </summary>
        /// <param name="leader"></param>
        public abstract void Accecpt(Leader leader);
    }

    /// <summary>
    /// 财务报表
    /// </summary>
    class FinancialReport : Reporter
    {
        private int income = 1000;
        private int expenditure = 500;

        public override void Accecpt(Leader leader)
        {
            leader.VisitFinancialReport(this);
        }

        /// <summary>
        /// 获取收入
        /// </summary>
        /// <returns></returns>
        public int GetIncome()
        {
            return income;
        }

        /// <summary>
        /// 设置支出
        /// </summary>
        /// <param name="expenditure"></param>
        public void SetExpenditure(int expenditure)
        {
            this.expenditure = expenditure;
        }

        /// <summary>
        /// 获取支出
        /// </summary>
        /// <returns></returns>
        public int GetExpenditure()
        {
            return expenditure;
        }
    }


    class LogisticsReport : Reporter
    {
        private int production = 800;
        private int sale = 400;

        public override void Accecpt(Leader leader)
        {
            leader.VisitLogisticsReport(this);
        }

        /// <summary>
        /// 获取生产额
        /// </summary>
        /// <returns></returns>
        public int GetProduction()
        {
            return production;
        }

        /// <summary>
        /// 获取销售额
        /// </summary>
        /// <returns></returns>
        public int GetSale()
        {
            return sale;
        }
    }



    class ReportManager
    {
        private List<Reporter> elements = new List<Reporter>();

        public void Attach(Reporter reporter)
        {
            elements.Add(reporter);
        }

        public void Detach(Reporter reporter)
        {
            elements.Remove(reporter);
        }

        public void Accept(Leader leader)
        {
            foreach (var element in elements)
            {
                element.Accecpt(leader);
            }
        }
    }
}
