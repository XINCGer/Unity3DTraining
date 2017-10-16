using UnityEngine;
using System;
using System.Collections;
using LuaInterface;

//例子5和6展示的两套协同系统勿交叉使用，此为推荐方案
public class TestCoroutine : MonoBehaviour 
{
    public TextAsset luaFile = null;
    private LuaState lua = null;
    private LuaLooper looper = null;

	void Awake () 
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif        
        new LuaResLoader();
        lua  = new LuaState();
        lua.Start();
        LuaBinder.Bind(lua);
        DelegateFactory.Init();         
        looper = gameObject.AddComponent<LuaLooper>();
        looper.luaState = lua;

        lua.DoString(luaFile.text, "TestLuaCoroutine.lua");
        LuaFunction f = lua.GetFunction("TestCortinue");
        f.Call();
        f.Dispose();
        f = null;        
    }

    void OnApplicationQuit()
    {
        looper.Destroy();
        lua.Dispose();
        lua = null;
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    string tips = null;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400), tips);

        if (GUI.Button(new Rect(50, 50, 120, 45), "Start Counter"))
        {
            tips = null;
            LuaFunction func = lua.GetFunction("StartDelay");
            func.Call();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(50, 150, 120, 45), "Stop Counter"))
        {
            LuaFunction func = lua.GetFunction("StopDelay");
            func.Call();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(50, 250, 120, 45), "GC"))
        {
            lua.DoString("collectgarbage('collect')", "TestCoroutine.cs");
            Resources.UnloadUnusedAssets();
        }
    }
}
