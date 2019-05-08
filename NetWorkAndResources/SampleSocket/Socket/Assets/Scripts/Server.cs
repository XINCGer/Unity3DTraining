using UnityEngine;  
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;  
using UnityEngine.UI;

public class Server: MonoBehaviour {  

	private static byte[] result = new byte[1024];  
	static Socket serverSocket;
    Thread myThread;

    void Start()  
	{  
		//服务器IP地址  
		IPAddress ip = IPAddress.Parse(GameConst.IP);  
		serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
		serverSocket.Bind(new IPEndPoint(ip, GameConst.Port));  //绑定IP地址：端口  
		serverSocket.Listen(10);    //设定最多10个排队连接请求  
		//通过Clientsoket发送数据  
		myThread = new Thread(ListenClientConnect);  
		myThread.Start();  
		Console.ReadLine();  
	}  

	/// <summary>  
	/// 监听客户端连接  
	/// </summary>  
	private void ListenClientConnect()  
	{  
		while (true)  
		{ 
			Socket clientSocket = serverSocket.Accept();  
			Thread receiveThread = new Thread(ReceiveMessage);  
			receiveThread.Start(clientSocket); 
			Debug.Log(clientSocket.RemoteEndPoint.ToString());
		}  
	}  

	/// <summary>  
	/// 接收消息
	/// </summary>  
	/// <param name="clientSocket"></param>  
	private static void ReceiveMessage(object clientSocket)  
	{  
		Socket myClientSocket = (Socket)clientSocket;  
		while (true)  
		{  
			try  
			{  
				int receiveNumber = myClientSocket.Receive(result);
                if (receiveNumber > 0)
                {
                    myClientSocket.Send(result, receiveNumber,0);
                }
			}  
			catch(Exception ex)  
			{  
				Console.WriteLine(ex.Message);  
				myClientSocket.Shutdown(SocketShutdown.Both);  
				myClientSocket.Close();  
				break;  
			}  
		}  
	}

    void OnApplicationQuit()
    {

        if (myThread != null)
        {
            myThread.Abort();
            myThread = null;
        }
    }


}  