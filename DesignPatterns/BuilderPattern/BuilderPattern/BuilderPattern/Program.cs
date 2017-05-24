using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuilderPattern
{
    /// <summary>
    /// 客户端代码
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Director director = new Director();
            Builder builderA = new ConcreteBuliderA();
            Builder builderB = new ConcreteBuilderB();

            //指挥者建造A
            director.Construct(builderA);
            Product productA = builderA.GetResult();
            productA.Show();

            //指挥者建造B
            director.Construct(builderB);
            Product productB = builderB.GetResult();
            productB.Show();

            //学生以及老师演示
            Console.WriteLine("=================================");

            //实例化两个学生
            Student studentA = new StudentA();
            Student studentB = new StudentB();

            //实例化老师
            Teacher teacher = new Teacher(studentA);
            //指导A做实验
            teacher.DirectExperiment();

            teacher = new Teacher(studentB);
            teacher.DirectExperiment();
            
            //老师为Director角色，学生为Builder角色，Teacher隔离了客户端与具体步骤的依赖
            //在一些项目中，经常需要构建一些比较复杂的对象，并对其多个属性进行赋值的复杂操作，程序员的一些忽略可能导致某个属性未被赋值
            //而引起对象的失效，在这种情况下使用构造者模式，创建一个Director来按部就班地指挥一个对象的创建，可以有效的避免意外的发生。
            //使用创建者模式，用户只需要指定创建的类型就可以得到相应的对象，而具体的建造过程和细节就被Director和Builder隐藏了，这正是
            //依赖倒转的体现：抽象不应该依赖于细节，细节应该依赖于抽象。

        }
    }
}
