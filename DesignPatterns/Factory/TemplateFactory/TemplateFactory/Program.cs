using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 工厂方法模式的定义：
/// 定义一个用于创建对象的接口，让子类决定实例化哪一个类
/// 工厂方法是一个类的实例化延迟到其子类中
/// </summary>
namespace TemplateFactory
{
    /// <summary>
    /// 新的代码中如果需要增加新的面包种类，只需要增加新的面包类
    /// 和对应的新的工厂即可
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            BreadMaker breadMaker = null;
            IFactory breadFactory = null;

            //根据不同的需要实例化接口
            breadFactory = new BlackBreadFactory();
            //生产面包
            breadMaker = breadFactory.CreateBread();
            breadMaker.GetBread();

            breadFactory = new HoneyBreadFactory();
            breadMaker = breadFactory.CreateBread();
            breadMaker.GetBread();
        }
    }
    /// <summary>
    /// 工厂接口定义
    /// </summary>
    public interface IFactory
    {
        BreadMaker CreateBread();
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
    }

    /// <summary>
    /// 不同的面包建立一个具体的工厂方法来实现这个接口
    /// </summary>
    public class HoneyWhiteBreadFactory : IFactory
    {
        public BreadMaker CreateBread()
        {
            return new HoneyWhiteBread();
        }
    }


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

    public class HoneyWhiteBread : HoneyBread
    {
        public override void GetBread()
        {
            Console.WriteLine("烤出了蜂蜜白面包！");
        }
    }
}
