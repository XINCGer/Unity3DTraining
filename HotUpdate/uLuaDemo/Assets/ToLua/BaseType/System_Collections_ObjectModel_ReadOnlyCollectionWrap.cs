using System;
using LuaInterface;
using System.Collections.ObjectModel;
using System.Collections;

public class System_Collections_ObjectModel_ReadOnlyCollectionWrap
{
    public static void Register(LuaState L)
    {
        L.BeginClass(typeof(ReadOnlyCollection<>), typeof(System.Object), "ReadOnlyCollection");
        L.RegFunction("Contains", Contains);
        L.RegFunction("CopyTo", CopyTo);
        L.RegFunction("GetEnumerator", GetEnumerator);
        L.RegFunction("IndexOf", IndexOf);
        L.RegFunction(".geti", get_Item);
        L.RegFunction("get_Item", get_Item);
        L.RegFunction("__tostring", ToLua.op_ToString);
        L.RegVar("Count", get_Count, null);
        L.EndClass();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Contains(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(ReadOnlyCollection<>), out argType);
            object arg0 = ToLua.CheckVarObject(L, 2, argType);
            bool o = (bool)LuaMethodCache.CallSingleMethod("Contains", obj, arg0);
            LuaDLL.lua_pushboolean(L, o);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int CopyTo(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 3);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(ReadOnlyCollection<>), out argType);
            object arg0 = ToLua.CheckObject(L, 2, argType.MakeArrayType());
            int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
            LuaMethodCache.CallSingleMethod("CopyTo", obj, arg0, arg1);
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int GetEnumerator(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 1);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(ReadOnlyCollection<>));
            IEnumerator o = (IEnumerator)LuaMethodCache.CallSingleMethod("GetEnumerator", obj);
            ToLua.Push(L, o);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int IndexOf(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(ReadOnlyCollection<>), out argType);
            object arg0 = ToLua.CheckVarObject(L, 2, argType);
            int o = (int)LuaMethodCache.CallSingleMethod("IndexOf", obj, arg0);
            LuaDLL.lua_pushinteger(L, o);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Item(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(ReadOnlyCollection<>));
            int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
            int o = (int)LuaMethodCache.CallSingleMethod("get_Item", obj, arg0);
            LuaDLL.lua_pushinteger(L, o);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int get_Count(IntPtr L)
    {
        object o = null;

        try
        {
            o = ToLua.ToObject(L, 1);
            int ret = (int)LuaMethodCache.CallSingleMethod("get_Count", o);
            LuaDLL.lua_pushinteger(L, ret);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e, o, "attempt to index Count on a nil value");
        }
    }
}

