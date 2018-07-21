using UnityEngine;
using Fitness.SocketClient;
using Com.Shapejoy.Remotecontrol.Proto;

public class HeartManager
{
    private static HeartManager instance;
    public static HeartManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new HeartManager();
            }
            return instance;
        }
    }

    private string deviceUniqueIdentifier;
    public void Init()
    {
        this.deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
    }

    public void CreateModel(GameObject obj)
    {
        obj.AddComponent<HeartModel>();
    }
    public void CToSDeviceCheck()
    {
        if (SocketManager.Instance.SocketClient.IsConnected)
        {
            var deviceNumber = this.deviceUniqueIdentifier;
            Message msg = new Message();
            msg.Event = Com.Shapejoy.Remotecontrol.Proto.Event.DeviceCheck;
            msg.DeviceNumber = deviceNumber;
            SocketManager.Instance.SendMessage(msg, (int)Com.Shapejoy.Remotecontrol.Proto.Event.DeviceCheck);
        }

    }

    public void CToSReconnect()
    {
        if (SocketManager.Instance.SocketClient.IsConnected)
        {
            Message msg = new Message();
            msg.Event = Com.Shapejoy.Remotecontrol.Proto.Event.Reconnection;
            //IsLogin 是否登陆用户
            //if (IsLogin)
            //{
            //    msg.UserId = UserInfo.Id;
            //}
            SocketManager.Instance.SendMessage(msg, (int)Com.Shapejoy.Remotecontrol.Proto.Event.Reconnection);
        }
    }
}
