using UnityEngine;
using System;
using Google.Protobuf;
using Com.Shapejoy.Remotecontrol.Proto;
using Fitness.SocketClient;

namespace Fitness.SocketClient
{
    public abstract class BaseModel<T> : MonoBehaviour where T : BaseModel<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        public static bool Exists
        {
            get;
            private set;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                Exists = true;
            }
            else if (_instance != this)
            {
                //throw new InvalidOperationException("Can't have two instances of a view");
            }
        }

        protected virtual void Start()
        {
            Init();
        }

        public void Init()
        {
            InitAddProtoDic();
            InitAddTocHandler();
        }

        protected abstract void InitAddTocHandler();
        protected abstract void InitAddProtoDic();

        protected void AddTocHandler(Com.Shapejoy.Remotecontrol.Proto.Event type, Action<Message> handler)
        {
            SocketManager.Instance.AddHandler(type, handler);
        }

        protected void RemoveTocHandler(Com.Shapejoy.Remotecontrol.Proto.Event type, Action<Message> handler)
        {
            SocketManager.Instance.RemoveHandler(type, handler);
        }

        protected void SendTos(IMessage obj, Com.Shapejoy.Remotecontrol.Proto.Event typeID)
        {
            SocketManager.Instance.SendMessage(obj, (int)typeID);
        }

        protected virtual void AddProtoDic<T2>(Com.Shapejoy.Remotecontrol.Proto.Event typeID) where T2 : IMessage
        {
            ProtoDic.Instance.AddProtoType((int)typeID, typeof(T2));
        }

        protected virtual void AddProtoDic(Com.Shapejoy.Remotecontrol.Proto.Event typeID)
        {
            ProtoDic.Instance.AddProtoType((int)typeID, typeof(IMessage));
        }

        protected virtual void OnDestroy()
        {

        }

    }
}

