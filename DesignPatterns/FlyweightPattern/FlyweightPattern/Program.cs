using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyweightPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 100;
            FlyweightFactory factory = new FlyweightFactory();

            Flyweight flyA = factory.GetFlyweight("A");
            flyA.Operation(i);

            Flyweight flyB = factory.GetFlyweight("B");
            flyB.Operation(i * 2);

            Flyweight flyC = factory.GetFlyweight("C");
            flyC.Operation(i * 3);

            Flyweight flyD = factory.GetFlyweight("D");
            flyD.Operation(i * 4);

            Flyweight flyE = new UnsharedFlyweight();
            flyE.Operation(i / 2);

            Flyweight flyF = factory.GetFlyweight("A");
            flyF.Operation(i / 2);

            Console.WriteLine("================================");
            //共享图片工厂
            ImageNodeFactory imageNodeFactory = new ImageNodeFactory();

            Dictionary<int,ImageNode> imageList = new Dictionary<int, ImageNode>();
            int randomNum = 0;
            Random random= new Random();

            for (int j = 0; j < 10; j++)
            {
                randomNum = random.Next(0, 10);
                switch (randomNum/2)
                {
                    case 0:
                        imageList.Add(j,imageNodeFactory.GetImage("Lion"));
                        break;
                    case 1:
                        imageList.Add(j, imageNodeFactory.GetImage("Tiger"));
                        break;
                    case 2:
                        imageList.Add(j, imageNodeFactory.GetImage("Cat"));
                        break;
                    default:
                        imageList.Add(j, imageNodeFactory.GetImage("Dog"));
                        break;
                }
            }

            //展示所生成的图像
            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine(j+"号坐标的");
                imageList[i].Show();
            }

            //随机选取两个坐标的图像进行比较
            int a = 0;
            int b = 0;

            while (a==b)
            {
                a = random.Next(0, 10);
                b = random.Next(0, 10);
            }

            //由于使用了共享，所以直接判断对象是否相等即可
            if (imageList[a] == imageList[b])
            {
                Console.WriteLine("可以消除");
            }
            else
            {
                Console.WriteLine("不可以消除");
            }
        }
    }
}
