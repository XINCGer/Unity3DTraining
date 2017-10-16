using System;
using LuaInterface;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class System_Collections_Generic_ListWrap
{    
    public static void Register(LuaState L)
	{        
        L.BeginClass(typeof(List<>), typeof(System.Object), "List");
		L.RegFunction("Add", Add);
		L.RegFunction("AddRange", AddRange);
		L.RegFunction("AsReadOnly", AsReadOnly);
		L.RegFunction("BinarySearch", BinarySearch);
		L.RegFunction("Clear", Clear);
		L.RegFunction("Contains", Contains);
		L.RegFunction("CopyTo", CopyTo);
		L.RegFunction("Exists", Exists);
		L.RegFunction("Find", Find);
		L.RegFunction("FindAll", FindAll);
		L.RegFunction("FindIndex", FindIndex);
		L.RegFunction("FindLast", FindLast);
		L.RegFunction("FindLastIndex", FindLastIndex);
		L.RegFunction("ForEach", ForEach);
		L.RegFunction("GetEnumerator", GetEnumerator);
		L.RegFunction("GetRange", GetRange);
		L.RegFunction("IndexOf", IndexOf);
		L.RegFunction("Insert", Insert);
        L.RegFunction("InsertRange", InsertRange);
        L.RegFunction("LastIndexOf", LastIndexOf);
		L.RegFunction("Remove", Remove);
		L.RegFunction("RemoveAll", RemoveAll);
		L.RegFunction("RemoveAt", RemoveAt);
		L.RegFunction("RemoveRange", RemoveRange);
		L.RegFunction("Reverse", Reverse);
		L.RegFunction("Sort", Sort);
		L.RegFunction("ToArray", ToArray);
		L.RegFunction("TrimExcess", TrimExcess);
		L.RegFunction("TrueForAll", TrueForAll);
		L.RegFunction("get_Item", get_Item);
		L.RegFunction("set_Item", set_Item);
        L.RegFunction(".geti", get_Item);
        L.RegFunction(".seti", set_Item);
        L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Capacity", get_Capacity, set_Capacity);
		L.RegVar("Count", get_Count, null);
        L.EndClass();        
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Add(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            object arg0 = ToLua.CheckVarObject(L, 2, argType);
            LuaMethodCache.CallSingleMethod("Add", obj, arg0);
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int AddRange(IntPtr L)
    {
        try
        {
            ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            object arg0 = ToLua.CheckObject(L, 2, typeof(IEnumerable<>).MakeGenericType(argType));
            LuaMethodCache.CallSingleMethod("AddRange", obj, arg0);
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AsReadOnly(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            object o = LuaMethodCache.CallSingleMethod("AsReadOnly", obj);            
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BinarySearch(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{                
                object arg0 = ToLua.CheckVarObject(L, 2, argType);
                int o = (int)LuaMethodCache.CallMethod("BinarySearch", obj, arg0);                
                LuaDLL.lua_pushinteger(L, o);
                return 1;
			}
			else if (count == 3)
			{                
                object arg0 = ToLua.CheckVarObject(L, 2, argType);
                object arg1 = ToLua.CheckObject(L, 3, typeof(IComparer<>).MakeGenericType(argType));
                int o = (int)LuaMethodCache.CallMethod("BinarySearch", obj, arg0, arg1);                
                LuaDLL.lua_pushinteger(L, o);
                return 1;
			}
			else if (count == 5)
			{
                int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
                int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
                object arg2 = ToLua.CheckVarObject(L, 4, argType);
                object arg3 = ToLua.CheckObject(L, 5, typeof(IComparer<>).MakeGenericType(argType));				
                int o = (int)LuaMethodCache.CallMethod("BinarySearch", obj, arg0, arg1, arg2, arg3);
                LuaDLL.lua_pushinteger(L, o);
                return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.BinarySearch", LuaMisc.GetTypeName(argType)));
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);			
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            LuaMethodCache.CallSingleMethod("Clear", obj);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Contains(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            object arg0 = ToLua.CheckVarObject(L, 2, argType);
            object o = LuaMethodCache.CallSingleMethod("Contains", obj, arg0);            
			LuaDLL.lua_pushboolean(L, (bool)o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int CopyTo(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{				                
                object arg0 = ToLua.CheckObject(L, 2, argType.MakeArrayType());
                LuaMethodCache.CallMethod("CopyTo", obj, arg0);
				return 0;
			}
			else if (count == 3)
			{                
                object arg0 = ToLua.CheckObject(L, 2, argType.MakeArrayType());                
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
                LuaMethodCache.CallMethod("CopyTo", obj, arg0, arg1);
                return 0;
			}
			else if (count == 5)
			{				                
                int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);				
                object arg1 = ToLua.CheckObject(L, 3, argType.MakeArrayType());
                int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
				int arg3 = (int)LuaDLL.luaL_checknumber(L, 5);
                LuaMethodCache.CallMethod("CopyTo", obj, arg0, arg1, arg2, arg3);
                return 0;
			}
			else
			{
				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.CopyTo", LuaMisc.GetTypeName(argType)));
            }
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Exists(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);       
            bool o = (bool)LuaMethodCache.CallMethod("Exists", obj, arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Find(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);            
            object o = LuaMethodCache.CallMethod("Find", obj, arg0);
            ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);
            object o = LuaMethodCache.CallMethod("FindAll", obj, arg0);
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindIndex(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{
                Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);                
                int o = (int)LuaMethodCache.CallMethod("FindIndex", obj, arg0);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 3)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
                Delegate arg1 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 3);                
                int o = (int)LuaMethodCache.CallMethod("FindIndex", obj, arg0, arg1);                
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);				
                Delegate arg2 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 4);                
                int o = (int)LuaMethodCache.CallMethod("FindIndex", obj, arg0, arg1, arg2);
                LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.FindIndex", LuaMisc.GetTypeName(argType)));                
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindLast(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);            
            object o = LuaMethodCache.CallSingleMethod("FindLast", obj, arg0);
            ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int FindLastIndex(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{				
				Delegate arg0 = (Delegate)ToLua.CheckObject(L, 2, typeof(System.Predicate<>).MakeGenericType(argType));				
                int o = (int)LuaMethodCache.CallMethod("FindLastIndex", obj, arg0);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 3)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				Delegate arg1 = (Delegate)ToLua.CheckObject(L, 3, typeof(System.Predicate<>).MakeGenericType(argType));				
                int o = (int)LuaMethodCache.CallMethod("FindLastIndex", obj, arg0, arg1);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);				
                Delegate arg2 = (Delegate)ToLua.CheckObject(L, 4, typeof(System.Predicate<>).MakeGenericType(argType));
                int o = (int)LuaMethodCache.CallMethod("FindLastIndex", obj, arg0, arg1, arg2);                
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.FindLastIndex", LuaMisc.GetTypeName(argType)));
            }
        }
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ForEach(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;            
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);            
			Delegate arg0 = ToLua.CheckDelegate(typeof(System.Action<>).MakeGenericType(argType), L, 2);	
            LuaMethodCache.CallSingleMethod("ForEach", obj, arg0);
			return 0;
		}
		catch(Exception e)
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
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            IEnumerator o = LuaMethodCache.CallSingleMethod("GetEnumerator", obj) as IEnumerator;            
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetRange(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);			
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);			
            object o = LuaMethodCache.CallSingleMethod("GetRange", obj, arg0, arg1);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IndexOf(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{								
                object arg0 = ToLua.CheckVarObject(L, 2, argType);
				int o = (int)LuaMethodCache.CallMethod("IndexOf", obj, arg0);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 3)
			{				
                object arg0 = ToLua.CheckVarObject(L, 2, argType);                
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);				
                int o = (int)LuaMethodCache.CallMethod("IndexOf", obj, arg0, arg1);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4)
			{
                object arg0 = ToLua.CheckVarObject(L, 2, argType);                
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);				
                int o = (int)LuaMethodCache.CallMethod("IndexOf", obj, arg0, arg1, arg2);
                LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.IndexOf", LuaMisc.GetTypeName(argType)));
            }
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Insert(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);            
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);			
            object arg1 = ToLua.CheckVarObject(L, 3, argType);
            LuaMethodCache.CallSingleMethod("Insert", obj, arg0, arg1);			
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int InsertRange(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			IEnumerable arg1 = (IEnumerable)ToLua.CheckObject(L, 3, typeof(IEnumerable<>).MakeGenericType(argType));
            LuaMethodCache.CallSingleMethod("InsertRange", obj, arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int LastIndexOf(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 2)
			{
                object arg0 = ToLua.CheckVarObject(L, 2, argType);                
				int o = (int)LuaMethodCache.CallMethod("LastIndexOf", obj, arg0);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 3)
			{
                object arg0 = ToLua.CheckVarObject(L, 2, argType);
                int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				int o = (int)LuaMethodCache.CallMethod("LastIndexOf", obj, arg0, arg1);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else if (count == 4)
			{
                object arg0 = ToLua.CheckVarObject(L, 2, argType);
                int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				int arg2 = (int)LuaDLL.luaL_checknumber(L, 4);
				int o = (int)LuaMethodCache.CallMethod("LastIndexOf", obj, arg0, arg1, arg2);
				LuaDLL.lua_pushinteger(L, o);
				return 1;
			}
			else
			{				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.LastIndexOf", LuaMisc.GetTypeName(argType)));
            }
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Remove(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            object arg0 = ToLua.CheckVarObject(L, 2, argType);
            bool o = (bool)LuaMethodCache.CallSingleMethod("Remove", obj, arg0);			
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);            
			Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);
			int o = (int)LuaMethodCache.CallSingleMethod("RemoveAll", obj, arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveAt(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));            
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
            LuaMethodCache.CallSingleMethod("RemoveAt", obj, arg0);			
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveRange(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
            LuaMethodCache.CallSingleMethod("RemoveRange", obj, arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reverse(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 1)
			{
                LuaMethodCache.CallMethod("Reverse", obj);				
				return 0;
			}
			else if (count == 3)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
                LuaMethodCache.CallMethod("Reverse", obj, arg0, arg1);                
				return 0;
			}
			else
			{				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.LastIndexOf", LuaMisc.GetTypeName(argType)));
            }
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Sort(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);

            if (count == 1)
			{
                LuaMethodCache.CallMethod("Sort", obj);
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes(L, 2, typeof(System.Comparison<>).MakeGenericType(argType)))
			{				
				Delegate arg0 = (Delegate)ToLua.ToObject(L, 2);
                LuaMethodCache.CallMethod("Sort", obj, arg0);                
				return 0;
			}
			else if (count == 2 && TypeChecker.CheckTypes(L, 2, typeof(IComparer<>).MakeGenericType(argType)))
			{
                object arg0 = ToLua.ToObject(L, 2);
                LuaMethodCache.CallMethod("Sort", obj, arg0);
                return 0;
			}
			else if (count == 4)
			{				
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
				object arg2 = ToLua.CheckObject(L, 4, typeof(IComparer<>).MakeGenericType(argType));
                LuaMethodCache.CallMethod("Sort", obj, arg0, arg1, arg2);                
				return 0;
			}
			else
			{				
                return LuaDLL.luaL_throw(L, string.Format("invalid arguments to method: List<{0}>.LastIndexOf", LuaMisc.GetTypeName(argType)));
            }
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ToArray(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            Array o = (Array)LuaMethodCache.CallSingleMethod("ToArray", obj);			
			ToLua.Push(L, o);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TrimExcess(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            LuaMethodCache.CallSingleMethod("TrimExcess", obj);
            return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TrueForAll(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
            Type argType = null;			
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);
            Delegate arg0 = ToLua.CheckDelegate(typeof(System.Predicate<>).MakeGenericType(argType), L, 2);
            bool o = (bool)LuaMethodCache.CallSingleMethod("TrueForAll", obj, arg0);
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch(Exception e)
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
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>));
            int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
            object o = LuaMethodCache.CallSingleMethod("get_Item", obj, arg0);
            ToLua.Push(L, o);			
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Item(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
            Type argType = null;
            object obj = ToLua.CheckGenericObject(L, 1, typeof(List<>), out argType);            
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
            object arg1 = ToLua.CheckObject(L, 3, argType);
            LuaMethodCache.CallSingleMethod("set_Item", obj, arg0, arg1);
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Capacity(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);			
            int ret = (int)LuaMethodCache.CallSingleMethod("get_Capacity", o);			
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Capacity on a nil value");
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
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Count on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Capacity(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);			
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
            LuaMethodCache.CallSingleMethod("set_Capacity", o, arg0);            
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Capacity on a nil value");
		}
	}
}

