using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System;
using System.Reflection;


public class TestReflection : LuaClient
{
    string script =
@"    
    require 'tolua.reflection'          
    tolua.loadassembly('Assembly-CSharp')        
    local BindingFlags = require 'System.Reflection.BindingFlags'

    function DoClick()
        print('do click')        
    end 

    function Test()  
        local t = typeof('TestExport')        
        local func = tolua.getmethod(t, 'TestReflection')           
        func:Call()        
        func:Destroy()
        func = nil
        
        local objs = {Vector3.one, Vector3.zero}
        local array = tolua.toarray(objs, typeof(Vector3))
        local obj = tolua.createinstance(t, array)
        --local constructor = tolua.getconstructor(t, typeof(Vector3):MakeArrayType())
        --local obj = constructor:Call(array)        
        --constructor:Destroy()

        func = tolua.getmethod(t, 'Test', typeof('System.Int32'):MakeByRefType())        
        local r, o = func:Call(obj, 123)
        print(r..':'..o)
        func:Destroy()

        local property = tolua.getproperty(t, 'Number')
        local num = property:Get(obj, null)
        print('object Number: '..num)
        property:Set(obj, 456, null)
        num = property:Get(obj, null)
        property:Destroy()
        print('object Number: '..num)

        local field = tolua.getfield(t, 'field')
        num = field:Get(obj)
        print('object field: '.. num)
        field:Set(obj, 2048)
        num = field:Get(obj)
        field:Destroy()
        print('object field: '.. num)       
        
        field = tolua.getfield(t, 'OnClick')
        local onClick = field:Get(obj)        
        onClick = onClick + DoClick        
        field:Set(obj, onClick)        
        local click = field:Get(obj)
        click:DynamicInvoke()
        field:Destroy()
        click:Destroy()
    end  
";

    string tips = null;

    protected override LuaFileUtils InitLoader()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif  
        return new LuaResLoader();
    }

    //屏蔽，例子不需要运行
    protected override void CallMain() { }

    void TestAction()
    {
        Debugger.Log("Test Action");
    }

    protected override void OnLoadFinished()
    {        
        base.OnLoadFinished();

        /*Type t = typeof(TestExport);
        MethodInfo md = t.GetMethod("TestReflection");
        md.Invoke(null, null);

        Vector3[] array = new Vector3[] { Vector3.zero, Vector3.one };
        object obj = Activator.CreateInstance(t, array);
        md = t.GetMethod("Test", new Type[] { typeof(int).MakeByRefType() });
        object o = 123;
        object[] args = new object[] { o };
        object ret = md.Invoke(obj, args);
        Debugger.Log(ret + " : " + args[0]);

        PropertyInfo p = t.GetProperty("Number");
        int num = (int)p.GetValue(obj, null);
        Debugger.Log("object Number: {0}", num);
        p.SetValue(obj, 456, null);
        num = (int)p.GetValue(obj, null);
        Debugger.Log("object Number: {0}", num);

        FieldInfo f = t.GetField("field");
        num = (int)f.GetValue(obj);
        Debugger.Log("object field: {0}", num);
        f.SetValue(obj, 2048);
        num = (int)f.GetValue(obj);
        Debugger.Log("object field: {0}", num);*/

        luaState.CheckTop();
        luaState.DoString(script, "TestReflection.cs");
        LuaFunction func = luaState.GetFunction("Test");
        func.Call();
        func.Dispose();
        func = null;
    }

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    new void OnApplicationQuit()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif  
        Destroy();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 150, 500, 300), tips);       
    }
}
