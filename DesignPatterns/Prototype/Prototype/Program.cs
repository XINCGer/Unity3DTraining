using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    class Program
    {
        static void Main(string[] args)
        {
            //原型模式测试1
            ConcretePrototype1 p1 = new ConcretePrototype1("1");
            ConcretePrototype1 p2 = (ConcretePrototype1)p1.Clone();

            Console.WriteLine("p1: "+p1.Id);
            Console.WriteLine("p2: " + p2.Id);
            //原型模式测试1 END


            //原型模式举例说明1
            Console.WriteLine("报名面点师");
            RestrationInfo restrationInfo1 = new RestrationInfo("小面");
            restrationInfo1.Birthday = "1992-2-2";
            restrationInfo1.School = "清华大学";
            restrationInfo1.Id = "001";

            //展示报名信息
            restrationInfo1.Show();
            //报名厨师
            Console.WriteLine("报名一级厨师");
            RestrationInfo restartion2 = restrationInfo1.Clone() as RestrationInfo;
            restartion2.Show();
            //原型模式举例说明End


            //原型模式举例说明2
            Console.WriteLine("报名面点师");
            DeepRestrationInfo deepRestrationInfo1 = new DeepRestrationInfo("小面");
            deepRestrationInfo1.Birthday = "1992-2-2";
            deepRestrationInfo1.School = "清华大学";
            deepRestrationInfo1.Id = "001";
            deepRestrationInfo1.SetNation("美国");

            //报名厨师
            Console.WriteLine("报名一级厨师");
            DeepRestrationInfo deepRestartion2 = deepRestrationInfo1.DeepClone() as DeepRestrationInfo;
            deepRestartion2.SetNation("中国");
            deepRestrationInfo1.Show();
            deepRestartion2.Show();
            //原型模式举例说明End
        }
    }

}
