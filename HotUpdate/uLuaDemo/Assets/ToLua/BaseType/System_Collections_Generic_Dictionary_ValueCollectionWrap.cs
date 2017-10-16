using System;
using LuaInterface;
using System.Collections.Generic;
using System.Collections;

public class System_Collections_Generic_Dictionary_ValueCollectionWrap
{
    public static void Register(LuaState L)
    {
        L.BeginClass(typeof(Dictionary<,>.ValueCollection), typeof(System.Object), "ValueCollection");
        L.RegFunction("CopyTo", CopyTo);
        L.RegFunction("GetEnumerator", GetEnumerator);
        L.RegFunction("New", _CreateSystem_Collections_Generic_Dictionary_ValueCollection);
        L.RegFunction("__tostring", ToLua.op_ToString);
        L.RegVar("Count", get_Count, null);
        L.EndClass();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int _CreateSystem_Collections_Generic_Dictionary_ValueCollection(IntPtr L)
    {
        try
        {
            int count = LuaDLL.lua_gettop(L);

            if (count == 1)
            {
                object arg0 = ToLua.CheckGenericObject(L, 1, typeof(Dictionary<,>));
                Type kv = arg0.GetType().GetNestedType("ValueCollection");
                object obj = Activator.CreateInstance(kv, arg0);
                ToLua.PushObject(L, obj);
                return 1;
            }
            else
            {
                return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: System.Collections.Generic.Dictionary.ValueCollection.New");
            }
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
            Type kt, kv;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(Dictionary<,>.ValueCollection), out kt, out kv);
            object arg0 = ToLua.CheckObject(L, 2, kv.MakeArrayType());
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
            object obj = ToLua.CheckGenericObject(L, 1, typeof(Dictionary<,>.ValueCollection));
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

