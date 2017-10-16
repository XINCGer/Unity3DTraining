using System;
using LuaInterface;

public class ToLua_System_Enum
{
    public static string ToIntDefined =
@"		try
        {
            object arg0 = ToLua.CheckObject<System.Enum>(L, 1);
            int ret = Convert.ToInt32(arg0);
            LuaDLL.lua_pushinteger(L, ret);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";

    public static string ParseDefined =
@"		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<System.Type, string>(L, 1))
			{
				System.Type arg0 = (System.Type)ToLua.ToObject(L, 1);
				string arg1 = ToLua.ToString(L, 2);
				object o = System.Enum.Parse(arg0, arg1);
				ToLua.Push(L, (Enum)o);
				return 1;
			}
			else if (count == 3 && TypeChecker.CheckTypes<System.Type, string, bool>(L, 1))
			{
				System.Type arg0 = (System.Type)ToLua.ToObject(L, 1);
				string arg1 = ToLua.ToString(L, 2);
				bool arg2 = LuaDLL.lua_toboolean(L, 3);
				object o = System.Enum.Parse(arg0, arg1, arg2);
				ToLua.Push(L, (Enum)o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, ""invalid arguments to method: System.Enum.Parse"");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}";

    public static string ToObjectDefined =
@"		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2 && TypeChecker.CheckTypes<System.Type, int>(L, 1))
			{
				System.Type arg0 = (System.Type)ToLua.ToObject(L, 1);
				int arg1 = (int)LuaDLL.lua_tonumber(L, 2);
				object o = System.Enum.ToObject(arg0, arg1);
				ToLua.Push(L, (Enum)o);
				return 1;
			}
			else if (count == 2 && TypeChecker.CheckTypes<System.Type, object>(L, 1))
			{
				System.Type arg0 = (System.Type)ToLua.ToObject(L, 1);
				object arg1 = ToLua.ToVarObject(L, 2);
				object o = System.Enum.ToObject(arg0, arg1);
				ToLua.Push(L, (Enum)o);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, ""invalid arguments to method: System.Enum.ToObject"");
			}
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}";

    [UseDefinedAttribute]
    public static void ToInt(System.Enum obj)
    {
    }

    [UseDefinedAttribute]
    public static object ToObject(Type enumType, int value)
    {
        return null;
    }

    [UseDefinedAttribute]
    public static object Parse(Type enumType, string value)
    {
        return null;
    }
}
