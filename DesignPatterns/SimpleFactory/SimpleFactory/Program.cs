using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFactory
{
    /// <summary>
    /// 对应的客户端代码
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            BreadMaker breadMaker = null;
            breadMaker = BreadFactory.MakeBread(BreadType.BlackBread);
            breadMaker.GetBread(); 
            breadMaker = BreadFactory.MakeBread(BreadType.HoneyBread);
            breadMaker.GetBread();
            breadMaker = BreadFactory.MakeBread(BreadType.WhiteBread);
            breadMaker.GetBread();
            breadMaker = BreadFactory.MakeBread(BreadType.HoneyWhiteBread);
            breadMaker.GetBread();
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

    /// <summary>
    /// 面包的类型枚举
    /// </summary>
    public enum BreadType : byte
    {
        BlackBread,
        HoneyBread,
        WhiteBread,
        HoneyWhiteBread
    }

    /// <summary>
    /// 简单面包工厂类
    /// 简单工厂模式是由一个工厂对象决定创建出哪一种产品的实例，可以理解为不同工厂模式的一个特殊实现
    /// </summary>
    public class BreadFactory
    {
        public static BreadMaker MakeBread(BreadType breadType)
        {
            BreadMaker breadMaker = null;
            switch (breadType)
            {
                    case BreadType.BlackBread:
                    breadMaker=new BlackBread();
                    break;
                    case BreadType.HoneyBread:
                    breadMaker=new HoneyBread();
                    break;
                    case BreadType.WhiteBread:
                    breadMaker=new WhiteBread();
                    break;
                    case BreadType.HoneyWhiteBread:
                    breadMaker=new HoneyWhiteBread();
                    break;

            }
            return breadMaker;
        }
    }


}
