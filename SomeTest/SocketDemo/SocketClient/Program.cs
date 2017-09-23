using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        private static byte[] dataBuff = new byte[1024];

        private static Socket socketClient;
        private static int port = 6666;


        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Loopback;

            socketClient = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            try
            {
                socketClient.Connect(new IPEndPoint(ip,port));
                Console.WriteLine("连接服务器成功！");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("连接服务器失败！按回车键退出！");
                return;
            }

            //通过clientSocket接受数据
            int receiveNum = socketClient.Receive(dataBuff);
            Console.WriteLine("接收服务器消息:{0}",Encoding.UTF8.GetString(dataBuff,0,receiveNum));

            //通过clientSocket发送消息
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Thread.Sleep(1000);
                    string message = string.Format("{0}{1}", "Server 你好！", DateTime.Now);
                    socketClient.Send(Encoding.UTF8.GetBytes(message));
                    Console.WriteLine("向服务器发送消息:{0}",message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    break;
                }
            }
            Console.WriteLine("发送完毕！按回车键退出！");
            Console.Read();
        }
    }
}
