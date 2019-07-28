using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MyScript : MonoBehaviour
{

    private static readonly string AndroidKey = "YouAndroidQQGroupKey";

    private static readonly string iOSUid = "YouiOSUid";
    private static readonly string iOSKey = "YouiOSQQGroupKey";

    private AndroidJavaClass _jc;
    private AndroidJavaObject _jo;

    // Use this for initialization
    void Start()
    {
        _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _jo = _jc.GetStatic<AndroidJavaObject>("currentActivity");

        var btnObj = this.transform.Find("BtnQQ");
        var button = btnObj.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        bool result = JoinQQGroup();
        if (result)
        {
            //TODO 你的业务逻辑
        }
        else
        {
            Debug.LogWarning("未安装手Q或者版本不支持！");
        }
    }

    /// <summary>
    /// 加入QQ群的方法，有返回值，代表成功或者失败
    /// </summary>
    /// <returns></returns>
    private bool JoinQQGroup()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return CallAndroidMethod<bool>("joinQQGroup", AndroidKey);
#elif !UNITY_EIDTOR && UNITY_IOS
        return iOSJoinQQGroup(iOSKey, iOSUid);
#else
        return false;
#endif
    }

    /// <summary>
    /// 调用一个带有返回值的原生Android方法
    /// </summary>
    /// <typeparam name="ReturnType"></typeparam>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private ReturnType CallAndroidMethod<ReturnType>(string method, params object[] args)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        return _jo.Call<ReturnType>(method, args);
#endif
        return default(ReturnType);
    }

    //iOS方法导入
#if !UNITY_EDITOR && UNITY_IOS
    [DllImport("__Internal")]
    private static extern bool iOSJoinQQGroup(string key, string uid);
#endif

}
