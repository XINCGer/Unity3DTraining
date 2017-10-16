using UnityEngine;
using LuaInterface;
using System;
using Debugger = LuaInterface.Debugger;

namespace LuaInterface
{
    public partial struct LuaValueType
    {
        public const int Rect = 13;
    }
}

public class PassStruct : LuaClient 
{
    private string script =
        @"
            Rect = {}
                       
            function Rect.New(x,y,w,h)
                local rt = {x = x, y = y, w = w, h = h}
                setmetatable(rt, Rect)                
                return rt
            end

            function Rect:Get()                       
                return self.x, self.y, self.w, self.h
            end
          
            Rect.__tostring = function(self)
                return '(x:'..self.x..', y:'..self.y..', width:'..self.w..', height:'..self.h..')'
            end

            function PrintRect(rt)
                print(tostring(rt))
                return rt
            end

            setmetatable(Rect, Rect)
            AddValueType(Rect, 13)
        ";

    void PushRect(IntPtr L, Rect rt)
    {
        LuaDLL.lua_getref(L, NewRect.GetReference());
        LuaDLL.lua_pushnumber(L, rt.xMin);
        LuaDLL.lua_pushnumber(L, rt.yMin);
        LuaDLL.lua_pushnumber(L, rt.width);
        LuaDLL.lua_pushnumber(L, rt.height);
        LuaDLL.lua_call(L, 4, 1);
    }

    Rect ToRectValue(IntPtr L, int pos)
    {
        pos = LuaDLL.abs_index(L, pos);
        LuaDLL.lua_getref(L, GetRect.GetReference());
        LuaDLL.lua_pushvalue(L, pos);
        LuaDLL.lua_call(L, 1, 4);
        float x = (float)LuaDLL.lua_tonumber(L, -4);
        float y = (float)LuaDLL.lua_tonumber(L, -3);
        float w = (float)LuaDLL.lua_tonumber(L, -2);
        float h = (float)LuaDLL.lua_tonumber(L, -1);
        LuaDLL.lua_pop(L, 4);

        return new Rect(x, y, w, h);
    }

    Rect CheckRectValue(IntPtr L, int pos)
    {
        int type = LuaDLL.tolua_getvaluetype(L, pos);

        if (type != LuaValueType.Rect)
        {
            luaState.LuaTypeError(pos, "Rect", LuaValueTypeName.Get(type));
            return new Rect();
        }

        return ToRectValue(L, pos);
    }

    bool CheckRectType(IntPtr L, int pos)
    {
        return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Rect;        
    }

    bool CheckNullRectType(IntPtr L, int pos)
    {
        LuaTypes luaType = LuaDLL.lua_type(L, pos);

        switch (luaType)
        {
            case LuaTypes.LUA_TNIL:
                return true;
            case LuaTypes.LUA_TTABLE:
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Rect;
            default:
                return false;
        }
    }

    object ToRectTable(IntPtr L, int pos)
    {
        return ToRectValue(L, pos);
    }

    string tips = null;

    void ShowTips(string msg, string stackTrace, LogType type)
    {
        tips += msg;
        tips += "\r\n";
    }

    new void OnApplicationQuit()
    {
        base.OnApplicationQuit();
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived -= ShowTips;
#else
        Application.RegisterLogCallback(null);
#endif                  
    }

    new void Awake()
    {
#if UNITY_5 || UNITY_2017
        Application.logMessageReceived += ShowTips;
#else
        Application.RegisterLogCallback(ShowTips);
#endif
        base.Awake();
    }

    protected override void OnLoadFinished()
    {
        base.OnLoadFinished();
        luaState.DoString(script, "PassStruct.cs");

        NewRect = luaState.GetFunction("Rect.New");
        GetRect = luaState.GetFunction("Rect.Get");
        StackTraits<Rect>.Init(PushRect, CheckRectValue, ToRectValue);           //支持压入lua以及从lua栈读取
        TypeTraits<Rect>.Init(CheckRectType);                                    //支持重载函数TypeCheck.CheckTypes
        TypeTraits<Nullable<Rect>>.Init(CheckNullRectType);                      //支持重载函数TypeCheck.CheckTypes
        LuaValueTypeName.names[LuaValueType.Rect] = "Rect";                      //CheckType失败提示的名字
        TypeChecker.LuaValueTypeMap[LuaValueType.Rect] = typeof(Rect);           //用于支持类型匹配检查操作
        ToLua.ToVarMap[LuaValueType.Rect] = ToRectTable;                         //Rect作为object读取
        ToLua.VarPushMap[typeof(Rect)] = (L, o) => { PushRect(L, (Rect)o); };    //Rect作为object压入

        //测试例子
        LuaFunction func = luaState.GetFunction("PrintRect");
        func.BeginPCall();
        func.PushValue(new Rect(10, 20, 120, 50));
        func.PCall();
        Rect rt = func.CheckValue<Rect>();
        func.EndPCall();
        Debugger.Log(rt);
        Debugger.Log(Vector3.one.ToString());
    }

    LuaFunction NewRect = null;
    LuaFunction GetRect = null;    

    protected override LuaFileUtils InitLoader()
    {
        return new LuaResLoader();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 220, Screen.height / 2 - 200, 400, 400), tips);        
    }

    //屏蔽，例子不需要运行
    protected override void CallMain() { }
}
