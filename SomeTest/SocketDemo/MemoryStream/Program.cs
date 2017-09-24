using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryStream
{
    class Program
    {
        static void Main(string[] args)
        {

            //构造MemeoryStream实例
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            Console.WriteLine("初始化分配容量：{0}",memoryStream.Capacity);
            Console.WriteLine("初始使用量：{0}",memoryStream.Length);

            //将待写入数据从字符串转换为字节数组
            UnicodeEncoding encoder = new UnicodeEncoding();
            byte[] bytes = encoder.GetBytes("新增数据");

            //向内存流中写入数据
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("第{0}写入新数据",i);
                memoryStream.Write(bytes,0,bytes.Length);
            }

            //写入数据后MemeoryStream实例的容量和使用量大小
            Console.WriteLine("当前分配容量：{0}",memoryStream.Capacity);
            Console.WriteLine("当前使用量：{0}",memoryStream.Length);

            Console.Read();
        }
    }
}
