using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            ObjectStructure obj = new ObjectStructure();
            obj.Attach(new ConcreteElementA());
            obj.Attach(new ConcreteElementB());
            ConcreteVisitor visitor = new ConcreteVisitor();
            obj.Accept(visitor);

            ReportManager reportManager = new ReportManager();
            reportManager.Attach(new FinancialReport());
            reportManager.Attach(new LogisticsReport());

            FinanceDirector financeDirector = new FinanceDirector();
            SaleDirector saleDirector = new SaleDirector();

            reportManager.Accept(financeDirector);
            reportManager.Accept(saleDirector);
        }
    }
}
