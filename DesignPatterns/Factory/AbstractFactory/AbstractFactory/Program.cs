using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AbstractFactory
{
    /// <summary>
    /// 抽象工厂：提供一个创建一系列相关或者相互依赖对象的接口，而毋须指定他们的具体的类。
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            BreadMaker breadMaker = null;
            PizzaMaker pizzaMaker = null;
            IFactory factory = null;
            factory = new BlackBreadFactory();
            breadMaker = factory.CreateBread();
            breadMaker.GetBread();

            factory=new HoneyBreadFactory();
            pizzaMaker = factory.CreatePizza();
            pizzaMaker.GetPizza();

        }
    }

    #region 工厂

    /// <summary>
    /// 工厂接口定义
    /// </summary>
    public interface IFactory
    {
        BreadMaker CreateBread();
        PizzaMaker CreatePizza();
    }

    /// <summary>
    /// 不同的面包建立一个具体的工厂方法来实现这个接口
    /// </summary>
    public class BlackBreadFactory : IFactory
    {
        public BreadMaker CreateBread()
        {
            return new BlackBread();
        }

        public PizzaMaker CreatePizza()
        {
            return new BlackPizza();
        }
    }

    /// <summary>
    /// 不同的面包建立一个具体的工厂方法来实现这个接口
    /// </summary>
    public class HoneyBreadFactory : IFactory
    {
        public BreadMaker CreateBread()
        {
            return new HoneyBread();
        }

        public PizzaMaker CreatePizza()
        {
            return new HoneyPizza();
        }
    }

    /// <summary>
    /// 不同的面包建立一个具体的工厂方法来实现这个接口
    /// </summary>
    public class WhiteBreadFactory : IFactory
    {
        public BreadMaker CreateBread()
        {
            return new WhiteBread();
        }

        public PizzaMaker CreatePizza()
        {
            return new WhitePizza();
        }
    }
    #endregion

    #region 面包类
    public abstract class BreadMaker
    {
        public abstract void GetBread();
    }

    /// <summary>
    /// 黑面包
    /// </summary>
    public class BlackBread : BreadMaker
    {
        public override void GetBread()
        {
            Console.WriteLine("烤出了黑面包!");
        }
    }

    /// <summary>
    /// 蜂蜜面包
    /// </summary>
    public class HoneyBread : BreadMaker
    {

        public override void GetBread()
        {
            Console.WriteLine("烤出了蜂蜜面包!");
        }
    }

    /// <summary>
    /// 白面包
    /// </summary>
    public class WhiteBread : BreadMaker
    {
        public override void GetBread()
        {
            Console.WriteLine("烤出了白面包!");
        }
    }

    #endregion

    #region 披萨类

    public abstract class PizzaMaker
    {
        public abstract void GetPizza();
    }

    public class BlackPizza : PizzaMaker
    {
        public override void GetPizza()
        {
            Console.WriteLine("生产出了黑披萨");
        }
    }

    public class HoneyPizza : PizzaMaker
    {
        public override void GetPizza()
        {
            Console.WriteLine("生产出了蜂蜜披萨");
        }
    }

    public class WhitePizza : PizzaMaker
    {
        public override void GetPizza()
        {
            Console.WriteLine("生产出了白披萨");
        }
    }

    #endregion
}
