using UnityEngine;
using LuaInterface;

public class AccessingArray : MonoBehaviour 
{
    private string script =
        @"
            function TestArray(array)
                local len = array.Length
                
                for i = 0, len - 1 do
                    print('Array: '..tostring(array[i]))
                end

                local iter = array:GetEnumerator()

                while iter:MoveNext() do
                    print('iter: '..iter.Current)
                end

                local t = array:ToTable()                
                
                for i = 1, #t do
                    print('table: '.. tostring(t[i]))
                end

                local pos = array:BinarySearch(3)
                print('array BinarySearch: pos: '..pos..' value: '..array[pos])

                pos = array:IndexOf(4)
                print('array indexof bbb pos is: '..pos)
                
                return 1, '123', true
            end            
        ";

    LuaState lua = null;
    LuaFunction func = null;
    string tips = null;

    void Start()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        new LuaResLoader();
        lua = new LuaState();
        lua.Start();
        lua.DoString(script, "AccessingArray.cs");
        tips = "";

        int[] array = { 1, 2, 3, 4, 5};        
        func = lua.GetFunction("TestArray");

        func.BeginPCall();
        func.Push(array);
        func.PCall();
        double arg1 = func.CheckNumber();
        string arg2 = func.CheckString();
        bool arg3 = func.CheckBoolean();
        Debugger.Log("return is {0} {1} {2}", arg1, arg2, arg3);
        func.EndPCall();

        //调用通用函数需要转换一下类型，避免可变参数拆成多个参数传递
        object[] objs = func.LazyCall((object)array);

        if (objs != null)
        {
            Debugger.Log("return is {0} {1} {2}", objs[0], objs[1], objs[2]);
        }

        lua.CheckTop();                
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600), tips);
    }

    void OnApplicationQuit()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
        func.Dispose();
        lua.Dispose();
    }
}
