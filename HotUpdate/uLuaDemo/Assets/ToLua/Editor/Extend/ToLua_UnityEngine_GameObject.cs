using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class ToLua_UnityEngine_GameObject
{
    public static string SendMessageDefined =
@"		IntPtr L0 = LuaException.L;

		try
		{
            ++LuaException.SendMsgCount;
            LuaException.L = L;
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<string>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				obj.SendMessage(arg0);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                
				--LuaException.SendMsgCount;
                LuaException.L = L0;
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				UnityEngine.SendMessageOptions arg1 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 3);
				obj.SendMessage(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
				return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, object>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				obj.SendMessage(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
				return 0;
			}
			else if (count == 4 && TypeChecker.CheckTypes<string, object, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				UnityEngine.SendMessageOptions arg2 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 4);
				obj.SendMessage(arg0, arg1, arg2);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
				return 0;
			}
			else
			{
                --LuaException.SendMsgCount;      
                LuaException.L = L0;                          
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: UnityEngine.GameObject.SendMessage"");     
			}
		}
		catch(Exception e)
		{
			--LuaException.SendMsgCount;
			LuaException.L = L0;
			return LuaDLL.toluaL_exception(L, e);
		}";

    public static string SendMessageUpwardsDefined =
@"		IntPtr L0 = LuaException.L;

		try
		{
            ++LuaException.SendMsgCount;
            LuaException.L = L;
            int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<string>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				obj.SendMessageUpwards(arg0);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				UnityEngine.SendMessageOptions arg1 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 3);
				obj.SendMessageUpwards(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, object>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				obj.SendMessageUpwards(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 4 && TypeChecker.CheckTypes<string, object, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				UnityEngine.SendMessageOptions arg2 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 4);
				obj.SendMessageUpwards(arg0, arg1, arg2);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else
			{
				--LuaException.SendMsgCount;
                LuaException.L = L0;                
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: UnityEngine.GameObject.SendMessageUpwards"");
			}
		}
		catch (Exception e)
		{
			--LuaException.SendMsgCount;
			LuaException.L = L0;
			return LuaDLL.toluaL_exception(L, e);
		}";

    public static string BroadcastMessageDefined =
@"		IntPtr L0 = LuaException.L;
    
		try
		{
            ++LuaException.SendMsgCount;
            LuaException.L = L;
            int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<string>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				obj.BroadcastMessage(arg0);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				UnityEngine.SendMessageOptions arg1 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 3);
				obj.BroadcastMessage(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 3 && TypeChecker.CheckTypes<string, object>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				obj.BroadcastMessage(arg0, arg1);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else if (count == 4 && TypeChecker.CheckTypes<string, object, UnityEngine.SendMessageOptions>(L, 2))
			{
				UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
				string arg0 = ToLua.ToString(L, 2);
				object arg1 = ToLua.ToVarObject(L, 3);
				UnityEngine.SendMessageOptions arg2 = (UnityEngine.SendMessageOptions)ToLua.ToObject(L, 4);
				obj.BroadcastMessage(arg0, arg1, arg2);                

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }

				--LuaException.SendMsgCount;
                LuaException.L = L0;
                return 0;
			}
			else
			{
                --LuaException.SendMsgCount;
                LuaException.L = L0;
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: UnityEngine.GameObject.BroadcastMessage"");
			}
		}
		catch (Exception e)
		{
			--LuaException.SendMsgCount;
			LuaException.L = L0;
			return LuaDLL.toluaL_exception(L, e);
		}";


    public static string AddComponentDefined = 
@"		IntPtr L0 = LuaException.L;

        try
        {
            ++LuaException.InstantiateCount;
            LuaException.L = L;
            ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject obj = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			System.Type arg0 = ToLua.CheckMonoType(L, 2);
			UnityEngine.Component o = obj.AddComponent(arg0);

            if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
            {
                string error = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_pop(L, 1);
                throw new LuaException(error, LuaException.GetLastError());
            }

            ToLua.Push(L, o);
            LuaException.L = L0;
            --LuaException.InstantiateCount;
            return 1;
		}
		catch (Exception e)
		{
            LuaException.L = L0;
            --LuaException.InstantiateCount;
            return LuaDLL.toluaL_exception(L, e);
		}";
    [UseDefinedAttribute]
    public void SendMessage(string methodName)
    {
    }

    [UseDefinedAttribute]
    public void SendMessageUpwards(string methodName)
    {
    }

    [UseDefinedAttribute]
    public void BroadcastMessage(string methodName)
    {

    }

    [UseDefinedAttribute]
    public void AddComponent(Type t)
    {

    }
}
