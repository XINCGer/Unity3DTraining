using UnityEngine;
using System.Collections;
using LuaInterface;

public class ToLua_System_String
{
    [NoToLuaAttribute]
    public static string ToLua_System_StringDefined =
@"        try
        {
            LuaTypes luatype = LuaDLL.lua_type(L, 1);

            if (luatype == LuaTypes.LUA_TSTRING)
            {
                string arg0 = LuaDLL.lua_tostring(L, 1);
                ToLua.PushSealed(L, arg0);
                return 1;
            }
            else
            {
                return LuaDLL.luaL_throw(L, ""invalid arguments to string's ctor method"");
            }            
        }
        catch(Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";

    [UseDefinedAttribute]
    public ToLua_System_String()
    {

    }
}
