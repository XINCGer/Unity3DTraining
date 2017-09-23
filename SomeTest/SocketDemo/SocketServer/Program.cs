using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    class Program
    {
        //缓冲区
        private static byte[] dataBuff = new byte[1024];
        //端口号
        private static int port = 6666;
        //socket
        private static Socket socketServer;

        static void Main(string[] args)
        {
            //设置监听的IP地址和端口，这里使用回环地址
            IPAddress ip = IPAddress.Loopback;

            //实例化一个socket对象，确定网络类型，socket类型，协议类型
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //绑定端口与IP
            socketServer.Bind(new IPEndPoint(ip, port));

            //挂起连接队列的最大长度为15，并启动监听
            socketServer.Listen(15);

            Console.WriteLine("启动监听{0}成功！", socketServer.LocalEndPoint);

            //创建一个线程用来处理客户端的连接
            Thread thread = new Thread(ListenClientConnect);
            thread.Start();
        }

        /// <summary>
        /// 处理客户端的连接
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                //运行到Accept()方法会阻塞程序（同步Socket）
                //一收到客户端的请求，就新建一个线程来处理数据
                Socket socketClient = socketServer.Accept();
                socketClient.Send(Encoding.UTF8.GetBytes("Server:你好！客户端！"));
                Thread thread = new Thread(ReciveMessage);
                thread.Start(socketClient);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="socketClient"></param>
        private static void ReciveMessage(object socketClient)
        {
            if (null != socketClient)
            {
                Socket mySocketClient = socketClient as Socket;

                while (true)
                {
                    try
                    {
                        //通过socketClient来接收数据
                        int reciveNum = mySocketClient.Receive(dataBuff);
                        Console.WriteLine("接收客户端：{0}消息{1}", mySocketClient.RemoteEndPoint, Encoding.UTF8.GetString(dataBuff, 0, reciveNum));

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        mySocketClient.Shutdown(SocketShutdown.Both);
                        mySocketClient.Close();
                        break;
                    }
                }
            }
        }
    }
}
