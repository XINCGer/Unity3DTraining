using UnityEngine;
using System.Collections;
using System;
using LuaInterface;
using System.Collections.Generic;


public class TestInt64 : MonoBehaviour
{
    private string tips = "";

    string script =
        @"            
            function TestInt64(x)                
                x = 789 + x
                assert(tostring(x) == '9223372036854775807')		                                       
                local low, high = int64.tonum2(x)                
                print('x value is: '..tostring(x)..' low is: '.. low .. ' high is: '..high.. ' type is: '.. tolua.typename(x))           
                local y = int64.new(1,2)                
                local z = int64.new(1,2)
                
                if y == z then
                    print('int64 equals is ok, value: '..int64.tostring(y))
                end

                x = int64.new(123)                   
            
                if int64.equals(x, 123) then
                    print('int64 equals to number ok')
                else
                    print('int64 equals to number failed')
                end

                x = int64.new('78962871035984074')
                print('int64 is: '..tostring(x))

                local str = tostring(int64.new(3605690779, 30459971))                
                local n2 = int64.new(str)
                local l, h = int64.tonum2(n2)                        
                print(str..':'..tostring(n2)..' low:'..l..' high:'..h)                  

                print('----------------------------uint64-----------------------------')
                x = uint64.new('18446744073709551615')                                
                print('uint64 max is: '..tostring(x))
                l, h = uint64.tonum2(x)      
                str = tostring(uint64.new(l, h))
                print(str..':'..tostring(x)..' low:'..l..' high:'..h)     

                return y
            end
        ";


    void Start()
    {
#if UNITY_5 || UNITY_2017		
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif        
        new LuaResLoader();
        LuaState lua = new LuaState();
        lua.Start();
        lua.DoString(script, "TestInt64.cs");                

        LuaFunction func = lua.GetFunction("TestInt64");
        func.BeginPCall();
        func.Push(9223372036854775807 - 789);
        func.PCall();
        long n64 = func.CheckLong();        
        Debugger.Log("int64 return from lua is: {0}", n64);
        func.EndPCall();
        func.Dispose();
        func = null;

        lua.CheckTop();
        lua.Dispose();
        lua = null;              
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    void OnDestroy()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2 - 300, 600, 600), tips);
    }
}
