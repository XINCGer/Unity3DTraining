using UnityEngine;
using System.Collections;
using LuaInterface;
using System;
using System.Runtime.InteropServices;

//检测合理报错
public class TestLuaStack : MonoBehaviour
{
    public GameObject go = null;
    public GameObject go2 = null;
    public static LuaFunction show = null;
    public static LuaFunction testRay = null;
    public static LuaFunction showStack = null;
    public static LuaFunction test4 = null;

    private static GameObject testGo = null;
    private string tips = "";
    public static TestLuaStack Instance = null;    

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Test1(IntPtr L)
    {
        try
        {                                                    
            show.BeginPCall();
            show.PCall();
            show.EndPCall();
        }
        catch (Exception e)
        {            
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int PushLuaError(IntPtr L)
    {
        try
        {
            testRay.BeginPCall();
            testRay.Push(Instance);
            testRay.PCall();
            testRay.EndPCall();
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Test3(IntPtr L)
    {
        try
        {
            testRay.BeginPCall();
            testRay.PCall();
            testRay.CheckRay();
            testRay.EndPCall();
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Test4(IntPtr L)
    {
        try
        {
            show.BeginPCall();
            show.PCall();
            show.EndPCall();
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Test5(IntPtr L)
    {
        try
        {
            test4.BeginPCall();            
            test4.PCall();
            bool ret = test4.CheckBoolean();
            ret = !ret;
            test4.EndPCall();
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Test6(IntPtr L)
    {        
        try
        {
            throw new LuaException("this a lua exception");
        }
        catch (Exception e)
        {            
            return LuaDLL.toluaL_exception(L, e);            
        }       
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestOutOfBound(IntPtr L)
    {
        try
        {
            Transform transform = testGo.transform;
            Transform node = transform.GetChild(20);
            ToLua.Push(L, node);
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }        
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestArgError(IntPtr L)
    {                
        try
        {
            LuaDLL.luaL_typerror(L, 1, "number");
        }
        catch (Exception e)
        {            
            return LuaDLL.toluaL_exception(L, e);
        }

        return 0;
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestTableInCo(IntPtr L)
    {
        try
        {
            LuaTable table = ToLua.CheckLuaTable(L, 1);
            string str = (string)table["name"];
            ToLua.Push(L, str);            
            return 1;
        }
        catch (Exception e)
        {            
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestCycle(IntPtr L)
    {
        try
        {
            LuaState state = LuaState.Get(L);
            LuaFunction func = state.GetFunction("TestCycle");
            int c = (int)LuaDLL.luaL_checknumber(L, 1);

            if (c <= 2)
            {
                LuaDLL.lua_pushnumber(L, 1);
            }
            else
            {
                func.BeginPCall();
                func.Push(c - 1);
                func.PCall();
                int n1 = (int)func.CheckNumber();
                func.EndPCall();

                func.BeginPCall();
                func.Push(c - 2);
                func.PCall();
                int n2 = (int)func.CheckNumber();
                func.EndPCall();

                LuaDLL.lua_pushnumber(L, n1 + n2);
            }
            
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestNull(IntPtr L)
    {
        try
        {
            GameObject go = (GameObject)ToLua.CheckObject(L, 1, typeof(GameObject));
            ToLua.Push(L, go.name);
            return 1;
        }
        catch (Exception e)
        {            
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestAddComponent(IntPtr L)
    {
        try
        {
            GameObject go = (GameObject)ToLua.CheckObject(L, 1, typeof(GameObject));
            go.AddComponent<TestInstantiate2>();
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static void TestMul1()
    {
        throw new Exception("multi stack error");
    }

    static void TestMul0()
    {
        TestMul1();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int TestMulStack(IntPtr L)
    {
        try
        {
            TestMul0();
            return 0;
        }
        catch (Exception e)
        {
            //Debugger.Log("xxxx" + e.StackTrace);
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    void OnSendMsg()
    {
        try
        {
            LuaFunction func = state.GetFunction("TestStack.Test6");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
        }
        catch(Exception e)
        {
            state.ThrowLuaException(e);
        }
    }
    

    LuaState state = null;
    public TextAsset text = null;

    static Action TestDelegate = delegate { };

    void Awake()
    {
#if UNITY_5 || UNITY_2017	
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        Instance = this;
        new LuaResLoader();
        testGo = gameObject;                
        state = new LuaState();
        state.Start();
        LuaBinder.Bind(state);        

        state.BeginModule(null);
        state.RegFunction("TestArgError", TestArgError);
        state.RegFunction("TestTableInCo", TestTableInCo);
        state.RegFunction("TestCycle", TestCycle);
        state.RegFunction("TestNull", TestNull);
        state.RegFunction("TestAddComponent", TestAddComponent);
        state.RegFunction("TestOutOfBound", TestOutOfBound);
        state.RegFunction("TestMulStack", TestMulStack);            
        state.BeginStaticLibs("TestStack");
        state.RegFunction("Test1", Test1);
        state.RegFunction("PushLuaError", PushLuaError);
        state.RegFunction("Test3", Test3);
        state.RegFunction("Test4", Test4);
        state.RegFunction("Test5", Test5);
        state.RegFunction("Test6", Test6);            
        state.EndStaticLibs();
        state.EndModule();

        //state.DoFile("TestErrorStack.lua");
        state.Require("TestErrorStack");
        show = state.GetFunction("Show");
        testRay = state.GetFunction("TestRay");

        showStack = state.GetFunction("ShowStack");
        test4 = state.GetFunction("Test4");

        TestDelegate += TestD1;
        TestDelegate += TestD2;
    }

    void Update()
    {
        state.CheckTop();
    }

    void OnApplicationQuit()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
        state.Dispose();
        state = null;
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";

        if (type == LogType.Error || type == LogType.Exception)
        {
            tips += stackTrace;
        }
    }

    void TestD1()
    {
        Debugger.Log("delegate 1");
        TestDelegate -= TestD2;
    }

    void TestD2()
    {
        Debugger.Log("delegate 2");
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 150, 800, 400), tips);

        if (GUI.Button(new Rect(10, 10, 120, 40), "Error1"))
        {
            tips = "";
            showStack.BeginPCall();
            showStack.Push(go);
            showStack.PCall();
            showStack.EndPCall();
            showStack.Dispose();
            showStack = null;
        }
        else if (GUI.Button(new Rect(10, 60, 120, 40), "Instantiate Error"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Instantiate");
            func.BeginPCall();
            func.Push(go);
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 110, 120, 40), "Check Error"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestRay");
            func.BeginPCall();
            func.PCall();
            func.CheckRay();        //返回值出错
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 160, 120, 40), "Push Error"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestRay");
            func.BeginPCall();
            func.Push(Instance);
            func.PCall();            
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 210, 120, 40), "LuaPushError"))
        {
            //需要改lua文件让其出错
            tips = "";
            LuaFunction func = state.GetFunction("PushLuaError");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 260, 120, 40), "Check Error"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Test5");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 310, 120, 40), "Test Resume"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Test6");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 360, 120, 40), "out of bound"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestOutOfBound");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 410, 120, 40), "TestArgError"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Test8");
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 460, 120, 40), "TestFuncDispose"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Test8");
            func.Dispose();
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 510, 120, 40), "SendMessage"))
        {
            tips = "";
            gameObject.SendMessage("OnSendMsg");
        }
        else if (GUI.Button(new Rect(10, 560, 120, 40), "SendMessageInLua"))
        {
            LuaFunction func = state.GetFunction("SendMsgError");
            func.BeginPCall();
            func.Push(gameObject);
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(10, 610, 120, 40), "AddComponent"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestAddComponent");            
            func.BeginPCall();
            func.Push(gameObject);
            func.PCall();
            func.EndPCall();
            func.Dispose();            
        }
        else if (GUI.Button(new Rect(210, 10, 120, 40), "TableGetSet"))
        {
            tips = "";
            LuaTable table = state.GetTable("testev");
            int top = state.LuaGetTop();

            try
            {
                state.Push(table);
                state.LuaGetField(-1, "Add");
                LuaFunction func = state.CheckLuaFunction(-1);                

                if (func != null)
                {
                    func.Call();
                    func.Dispose();
                }

                state.LuaPop(1);
                state.Push(123456);
                state.LuaSetField(-2, "value");
                state.LuaGetField(-1, "value");
                int n = (int)state.LuaCheckNumber(-1);
                Debugger.Log("value is: " + n);

                state.LuaPop(1);

                state.Push("Add");
                state.LuaGetTable(-2);

                func = state.CheckLuaFunction(-1);

                if (func != null)
                {
                    func.Call();
                    func.Dispose();
                }

                state.LuaPop(1);

                state.Push("look");
                state.Push(456789);                
                state.LuaSetTable(-3);                
                                
                state.LuaGetField(-1, "look");
                n = (int)state.LuaCheckNumber(-1);
                Debugger.Log("look: " + n);
            }
            catch (Exception e)
            {
                state.LuaSetTop(top);
                throw e;
            }
                        
            state.LuaSetTop(top);
        }
        else if (GUI.Button(new Rect(210, 60, 120, 40), "TestTableInCo"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestCoTable");            
            func.BeginPCall();
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(210, 110, 120, 40), "Instantiate2 Error"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("Instantiate");
            func.BeginPCall();
            func.Push(go2);
            func.PCall();
            func.EndPCall();
            func.Dispose();
        }
        else if (GUI.Button(new Rect(210, 160, 120, 40), "Instantiate3 Error"))
        {
            tips = "";
            UnityEngine.Object.Instantiate(go2);
        }
        else if (GUI.Button(new Rect(210, 210, 120, 40), "TestCycle"))
        {
            tips = "";
            int n = 20;
            LuaFunction func = state.GetFunction("TestCycle");
            func.BeginPCall();
            func.Push(n);            
            func.PCall();
            int c = (int)func.CheckNumber();
            func.EndPCall();

            Debugger.Log("Fib({0}) is {1}", n, c);
        }
        else if (GUI.Button(new Rect(210, 260, 120, 40), "TestNull"))
        {
            tips = "";
            Action action = ()=>
            {
                LuaFunction func = state.GetFunction("TestNull");
                func.BeginPCall();
                func.PushObject(null);
                func.PCall();
                func.EndPCall();
            };

            StartCoroutine(TestCo(action));
        }
        else if (GUI.Button(new Rect(210, 310, 120, 40), "TestMulti"))
        {
            tips = "";
            LuaFunction func = state.GetFunction("TestMulStack");
            func.BeginPCall();
            func.PushObject(null);
            func.PCall();
            func.EndPCall();
        }
    }

    IEnumerator TestCo(Action action)
    {
        yield return new WaitForSeconds(0.1f);
        action();
    }
}
