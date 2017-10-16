using System;
using LuaInterface;
using System.Collections.Generic;

public class System_Collections_Generic_KeyValuePairWrap
{
    public static void Register(LuaState L)
    {
        L.BeginClass(typeof(KeyValuePair<,>), null, "KeyValuePair");
        L.RegFunction("__tostring", ToLua.op_ToString);
        L.RegVar("Key", get_Key, null);
        L.RegVar("Value", get_Value, null);
        L.EndClass();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Key(IntPtr L)
    {
        object o = null;

        try
        {
            o = ToLua.ToObject(L, 1);
            object ret = LuaMethodCache.CallSingleMethod("get_Key", o);
            ToLua.Push(L, ret);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e, o, "attempt to index Key on a nil value");
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Value(IntPtr L)
    {
        object o = null;

        try
        {
            o = ToLua.ToObject(L, 1);
            object ret = LuaMethodCache.CallSingleMethod("get_Value", o);
            ToLua.Push(L, ret);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e, o, "attempt to index Value on a nil value");
        }
    }
}

