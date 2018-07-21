using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Fitness.SocketClient
{
    public enum SocketState
    {
        Idle,
        Connecting,
        Connented,
        DisConnecting,
        DisConnected
    }
    public class SocketClient
    {
        public SocketState SocketState { get; private set; }
        private Socket clientSocket;
        public bool IsConnected
        {
            get
            {
                if(clientSocket != null && clientSocket.Connected)
                {
                    this.SocketState = SocketState.Connented;
                }
                else
                {
                    if(this.SocketState != SocketState.Connecting)
                    {
                        this.SocketState = SocketState.DisConnected;
                    }
                }
                return clientSocket != null && clientSocket.Connected;
            }
        }
        private Thread threadReceive;
        private IPEndPoint ipEndPoint;
        private bool isReconnect = false;
        private byte[] Buffer = new byte[1024];
        /// <summary>
        /// 连接远程地址完成事件
        /// </summary>
        public event EventHandler<SocketEventArgs> ConnectCompleted;

        /// <summary>
        /// 事件消息接收完成
        /// </summary>
        public event EventHandler<SocketEventArgs> ReceiveMessageCompleted;

        /// <summary>
        /// 发送消息完成
        /// </summary>
        public event EventHandler<SocketEventArgs> SendMessageCompleted;

        public Action ConnectedAction { get; set; }
        public Action ReconnectToServer { get; set; }
        public SocketClient(string ip, int port)
        {
            SocketState = SocketState.Idle;
            IPHostEntry hostInfo = Dns.GetHostEntry(ip);
            IPAddress mIp = hostInfo.AddressList[0];
            ipEndPoint = new IPEndPoint(mIp, port);
        }

        /// <summary>
        /// 发送连接请求
        /// </summary>
        public void SendConnect()
        {
            SocketState = SocketState.Connecting;
            Connect();
        }
        
        /// <summary>
        /// 连接指定IP和端口的服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        private bool Connect()
        {
            //("开始连接服务器" + ipEndPoint);
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Debug.LogError("开始连接服务器");
                clientSocket.BeginConnect(ipEndPoint, OnConnectCallBack, this);
                

                //("连接服务器成功" + ipEndPoint);
                return true;
            }
            catch (Exception ex)
            {
                //("连接服务器失败" + ex + ipEndPoint);
                return false;
            }
        }
        /// <summary>
        /// 连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectCallBack(IAsyncResult ar)
        {
            Debug.LogError(string.Format("连接服务器返回状态 = {0}", ar.AsyncState));
            SocketState = clientSocket.Connected ? SocketState.Connented : SocketState.DisConnected;
            if (!clientSocket.Connected)
            {
                return;
            }
            Debug.LogError(string.Format("连接服务器成功 = {0}", ar.AsyncState));
            if (this.ConnectedAction != null)
            {
                this.ConnectedAction();
            }
            if (isReconnect)
            {
                isReconnect = false;
                if(ReconnectToServer!= null)
                {
                    this.ReconnectToServer();
                }
            }
            clientSocket.EndConnect(ar);
            if (ConnectCompleted != null)
            {
                ConnectCompleted(this, new SocketEventArgs());
            }
            
            StartReceive();
        }
         /// <summary>
         /// 开始接收消息
         /// </summary>
         private void StartReceive()
         {
            if (!clientSocket.Connected)
            {
                return;
            }
            clientSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceiveDataComplete, this);
         }

        /// <summary>
        /// 数据接收完成的回调函数
        /// </summary>
        private void OnReceiveDataComplete(IAsyncResult ar)
        {
            int ByteRead = 0;
            try
            {
                //接收完毕消息后的字节数
                ByteRead = clientSocket.EndReceive(ar);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("[SocketClient] ex is {0}.", ex));
            }
            byte[] data = new byte[ByteRead];
            Array.Copy(Buffer, 0, data, 0, ByteRead);
            // 触发接收消息事件
            if (ReceiveMessageCompleted != null)
            {
                ReceiveMessageCompleted(this, new SocketEventArgs(data));
            }
            StartReceive();
        }
        #region Send Message
 
        

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data">要传递的消息内容[字节数组]</param>
        public void OnSendMessage(byte[] data)
        {
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            var all = stream.ToArray();
            stream.Close();
            try
            {
                clientSocket.BeginSend(all, 0, all.Length, SocketFlags.None, OnSendMessageComplete, all);
            }
            catch (Exception ex)
            {
                Reconnect();
                Debug.LogError(string.Format("[OnSendMessage] ex is {0}", ex));
            }
            
        }

        /// <summary>
        /// 发送消息完成的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void OnSendMessageComplete(IAsyncResult ar)
        {
            var data = ar.AsyncState as byte[];
            SocketError socketError;
            clientSocket.EndSend(ar, out socketError);
            if (socketError != SocketError.Success)
            {
                clientSocket.Disconnect(false);
                throw new SocketException((int)socketError);
            }
            if (SendMessageCompleted != null)
            {
                SendMessageCompleted(this, new SocketEventArgs(data));
            }
        }
 
         #endregion

        public void Reconnect()
        {            
            if (this.SocketState == SocketState.Connecting || this.SocketState == SocketState.Connented)
            {
                return;
            }
            isReconnect = true;
            if (threadReceive != null && threadReceive.IsAlive)
            {
                threadReceive.Abort();
            }
            if (null != clientSocket && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Disconnect(true);
                clientSocket.Close();

            }
            SendConnect();
        }
        

        public void OnApplicationQuit()
        {
            if (threadReceive != null)
            {
                threadReceive.Abort();
            }
            if(this.clientSocket!= null)
            {
                this.clientSocket.Shutdown(SocketShutdown.Both);
                this.clientSocket.Disconnect(true);
                this.clientSocket.Close();
            }
            
        }
    }

     #region Event
 
     /// <summary>
     /// Simple socket event args
     /// </summary>
     public class SocketEventArgs : EventArgs
     {
 
         public SocketEventArgs()
         {
         }
 
         public SocketEventArgs(byte[] data) : this()
         {
             Data = data;
         }
 
         /// <summary>
         /// 相关的数据
         /// </summary>
         public byte[] Data { get; private set; }
 
     }
 
     #endregion
}
