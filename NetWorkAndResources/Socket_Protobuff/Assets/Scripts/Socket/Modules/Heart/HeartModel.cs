using System.Threading;
using System;
using Com.Shapejoy.Remotecontrol.Proto;

namespace Fitness.SocketClient
{
    public class HeartModel : BaseModel<HeartModel>
    {

        private Thread thread;
        private bool isAlive = false;
        private bool isHeart = true;
        private Message heartMessage = new Message() { Event = Event.Heart };
        protected override void InitAddTocHandler()
        {
            this.AddTocHandler(Event.Heart, this.SToCHeart);
        }

        protected override void InitAddProtoDic()
        {
            this.AddProtoDic(Event.Heart);
            this.AddProtoDic(Event.DeviceCheck);
            this.AddProtoDic(Event.Reconnection);
        }

        void CToSHeart()
        {
            while (isAlive)
            {
                try
                {
                    if (!SocketManager.Instance.SocketClient.IsConnected)
                    {
                        Thread.Sleep(5000);
                        SocketManager.Instance.Reconnect();
                        continue;
                    }
                    if (!isHeart)
                    {
                        // "[CToSHeart]:Not Heart!!!!");
                    }
                    isHeart = false;
                    this.SendTos(heartMessage, Event.Heart);
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    //(ex, "Send heart beat error.");
                    Thread.Sleep(1000);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            
            this.thread = new Thread(new ThreadStart(this.CToSHeart));
            this.thread.IsBackground = true;
            this.isAlive = true;
            this.thread.Start();
        }
        
        void SToCHeart(object obj)
        {
            this.isHeart = true;
        }

        protected override void OnDestroy()
        {
            this.RemoveTocHandler(Event.Heart, this.SToCHeart);
            this.isAlive = false;
        }

        void OnApplicationQuit()
        {
            // "Heartmodel OnApplicationQuit");
            if(this.thread!= null)
            {
                this.thread.Abort();
            }
            this.isAlive = false;
        }
    }
}

