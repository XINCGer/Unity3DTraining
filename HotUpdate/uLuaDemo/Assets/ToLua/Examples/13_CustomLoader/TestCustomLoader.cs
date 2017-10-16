using UnityEngine;
using System.IO;
using LuaInterface;

//use menu Lua->Copy lua files to Resources. 之后才能发布到手机
public class TestCustomLoader : LuaClient 
{
    string tips = "Test custom loader";

    protected override LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
    }

    protected override void CallMain()
    {
        LuaFunction func = luaState.GetFunction("Test");
        func.Call();
        func.Dispose();
    }

    protected override void StartMain()
    {
        luaState.DoFile("TestLoader.lua");
        CallMain();
    }

    new void Awake()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += Logger;
#else
        Application.RegisterLogCallback(Logger);
#endif    
        base.Awake();
    }

    new void OnApplicationQuit()
    {
        base.OnApplicationQuit();

#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= Logger;
#else
        Application.RegisterLogCallback(null);
#endif    
    }

    void Logger(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400), tips);
    }
}
