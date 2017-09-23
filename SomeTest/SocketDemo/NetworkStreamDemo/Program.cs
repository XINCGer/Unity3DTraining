using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkStreamDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("www.baidu.com", 80);
            NetworkStream networkStream = tcpClient.GetStream();

            //是否有数据可读
            if (networkStream.CanRead)
            {
                //接收数据的缓冲区
                byte[] dataBuff = new byte[1024];
                StringBuilder stringBuilder = new StringBuilder();
                int ReceiveNum = 0;
                //准备接受的消息长度可能大于1024，用循环读取
                do
                {
                    ReceiveNum = networkStream.Read(dataBuff, 0, dataBuff.Length);
                    stringBuilder.AppendFormat("{0}", Encoding.UTF8.GetString(dataBuff, 0, ReceiveNum));
                } while (networkStream.DataAvailable);
                Console.WriteLine("接收到的消息为：" + stringBuilder);
            }
            else
            {
                Console.WriteLine("当前没有可供读取的数据！");
            }
        }
    }
}
