using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SystemInfoController : MonoBehaviour
{


    public Text MessageText;

    private StringBuilder stringBuilder;

    // Use this for initialization
    void Start()
    {
        MessageText.text = "";
        stringBuilder = new StringBuilder();
        stringBuilder.Append("设备与系统信息:" + "\r\n");
        stringBuilder.Append("设备型号：" + SystemInfo.deviceModel + "\r\n");
        stringBuilder.Append("设备名称:" + SystemInfo.deviceName + "\r\n");
        stringBuilder.Append("设备类型：" + SystemInfo.deviceType + "\r\n");
        stringBuilder.Append("设备唯一标识符:" + SystemInfo.deviceUniqueIdentifier + "\r\n");
        stringBuilder.Append("系统内存大小MB:" + SystemInfo.systemMemorySize + "\r\n");
        stringBuilder.Append("操作系统:" + SystemInfo.operatingSystem + "\r\n");
        stringBuilder.Append("显卡ID:" + SystemInfo.graphicsDeviceID + "\r\n");
        stringBuilder.Append("显卡名称：" + SystemInfo.graphicsDeviceName + "\r\n");
        stringBuilder.Append("显卡类型:" + SystemInfo.graphicsDeviceType + "\r\n");
        stringBuilder.Append("显卡供应商:" + SystemInfo.graphicsDeviceVendor + "\r\n");
        stringBuilder.Append("显卡供应商唯一ID:" + SystemInfo.graphicsDeviceVendorID + "\r\n");
        stringBuilder.Append("显卡版本号：" + SystemInfo.graphicsDeviceVersion + "\r\n");
        stringBuilder.Append("显存大小MB:" + SystemInfo.graphicsMemorySize + "\r\n");
        stringBuilder.Append("显卡是否支持多线程渲染:" + SystemInfo.graphicsMultiThreaded + "\r\n");
        stringBuilder.Append("支持的渲染目标数量:" + SystemInfo.supportedRenderTargetCount + "\r\n");

        MessageText.text = stringBuilder.ToString();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
