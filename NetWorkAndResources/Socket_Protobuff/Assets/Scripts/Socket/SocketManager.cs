using Com.Shapejoy.Remotecontrol.Proto;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace Fitness.SocketClient
{
    public class SocketManager 
    {

        private static SocketManager instance;
        public static SocketManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SocketManager();
                }
                return instance;
            }
        }
        private bool isQuit = false;

        private static Queue<Message> sEvents = new Queue<Message>();
        private Dictionary<Event, Action<Message>> _handlerDic = new Dictionary<Event, Action<Message>>();
        public SocketClient SocketClient { get; private set; }
        public SocketManager()
        {
            isQuit = false;
        }

        public void Init(string address,int port)
        {
            this.SocketClient = new SocketClient(address, port);
            this.InitData();
        }

        private void InitData()
        {
            Instance.SocketClient.ConnectedAction = HeartManager.Instance.CToSDeviceCheck;
            this.SocketClient.ReceiveMessageCompleted += (s, e) =>
            {
                var rmc = e as SocketEventArgs;
                if (rmc == null) return;
                var data = rmc.Data as byte[];
                if (data != null)
                {
                    ProtobufEncoding.Instance.DecodeAsync(data);
                }
            };
            this.SocketClient.SendConnect();
            this.LoadSocketMono();
        }

        private void LoadSocketMono()
        {
            UnityEngine.GameObject go = new UnityEngine.GameObject("SocketMono");
            go.AddComponent<SocketMono>();
        }
        public void SendMessage(IMessage obj, int typeID)
        {
            this.SendMessage(obj as Message);
        }

        /// <summary>
        /// 发送数据给服务器
        /// </summary>
        public void SendMessage(IMessage msg)
        {
            if (null == msg)
            {
                //"消息不能为空！");
                return;
            }
            Message message = msg as Message;
            //"SendMessage Event:{0},DeviceNumber:{1}", message.Event, message.DeviceNumber);

            this.SocketClient.OnSendMessage(ProtobufEncoding.Instance.Encode(msg));
        }

        public void Reconnect()
        {
            if(SocketClient!= null)
            {
                ProtobufEncoding.Instance.ResetBuffer();
                SocketClient.Reconnect();
            }           
        }

        /// <summary>
        /// 派发协议
        /// </summary>
        /// <param name="buff"></param>
        public void DispatchProto(byte[] buff)
        {
            var result = ProtobufEncoding.Instance.Decode(buff) as Message;
            this.DispatchProto(result);
        }
        public void DispatchProto(Message result)
        {
            //"[DispatchProto] result.Event :{0}", result.Event);

            if (!ProtoDic.Instance.ContainProtoId((int)result.Event))
            {
                //TraceWarning("未知协议号");
                return;
            }
            Type protoType = ProtoDic.Instance.GetProtoTypeByProtoId((int)result.Event);

            try
            {
                sEvents.Enqueue(result);
            }
            catch
            {
                //("DispatchProto Error:" + protoType.ToString());
            }
        }

        public void UpdateProto()
        {
            if (isQuit)
            {
                return;
            }
            if (sEvents.Count > 0)
            {
                while (sEvents.Count > 0)
                {
                    Message _event = sEvents.Dequeue();
                    if (_handlerDic.ContainsKey(_event.Event))
                    {
                        _handlerDic[_event.Event](_event);
                    }
                }
            }
        }

        public void AddHandler(Event type, Action<Message> handler)
        {
            if (_handlerDic.ContainsKey(type))
            {
                _handlerDic[type] += handler;
            }
            else
            {
                _handlerDic.Add(type, handler);
            }
        }

        public void RemoveHandler(Event type, Action<Message> handler)
        {
            if (_handlerDic.ContainsKey(type))
            {
                _handlerDic[type] -= handler;
            }
        }
        public void OnApplicationQuit()
        {
            this.SocketClient.OnApplicationQuit();
            isQuit = true;
        }
    }
}
