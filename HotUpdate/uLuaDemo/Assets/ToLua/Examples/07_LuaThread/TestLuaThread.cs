using UnityEngine;
using System.Collections;
using LuaInterface;

public class TestLuaThread : MonoBehaviour 
{
    string script =
        @"
            function fib(n)
                local a, b = 0, 1
                while n > 0 do
                    a, b = b, a + b
                    n = n - 1
                end

                return a
            end

            function CoFunc(len)
                print('Coroutine started')                
                local i = 0
                for i = 0, len, 1 do                    
                    local flag = coroutine.yield(fib(i))	                    
                    if not flag then
                        break
                    end                                      
                end
                print('Coroutine ended')
            end

            function Test()                
                local co = coroutine.create(CoFunc)                                
                return co
            end            
        ";

    LuaState state = null;
    LuaThread thread = null;
    string tips = null;

    void Start () 
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        state = new LuaState();
        state.Start();
        state.LogGC = true;
        state.DoString(script);

        LuaFunction func = state.GetFunction("Test");
        func.BeginPCall();
        func.PCall();
        thread = func.CheckLuaThread();
        thread.name = "LuaThread";
        func.EndPCall();
        func.Dispose();
        func = null;

        thread.Resume(10);
	}

    void OnApplicationQuit()
    {
        if (thread != null)
        {
            thread.Dispose();
            thread = null;
        }

        state.Dispose();
        state = null;
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void Update()
    {
        state.CheckTop();
        state.Collect();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400), tips);

        if (GUI.Button(new Rect(10, 50, 120, 40), "Resume Thead"))
        {
            int ret = -1;

            if (thread != null && thread.Resume(true, out ret) == (int)LuaThreadStatus.LUA_YIELD)
            {                
                Debugger.Log("lua yield: " + ret);
            }
        }
        else if (GUI.Button(new Rect(10, 150, 120, 40), "Close Thread"))
        {
            if (thread != null)
            {                
                thread.Dispose();                
                thread = null;
            }
        }
    }
}
