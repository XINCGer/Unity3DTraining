using System;
using LuaInterface;

public class ToLua_LuaInterface_EventObject
{
    [NoToLuaAttribute]
    public static string op_AdditionDefined =
@"        try
        {
            EventObject arg0 = (EventObject)ToLua.CheckObject(L, 1, typeof(EventObject));
            arg0.func = ToLua.CheckDelegate(arg0.type, L, 2);
            arg0.op = EventOp.Add;
            ToLua.Push(L, arg0);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";

    [NoToLuaAttribute]
    public static string op_SubtractionDefined =
@"        try
        {
            EventObject arg0 = (EventObject)ToLua.CheckObject(L, 1, typeof(EventObject));
            arg0.func = ToLua.CheckDelegate(arg0.type, L, 2);
            arg0.op = EventOp.Sub;
            ToLua.Push(L, arg0);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";

    [UseDefinedAttribute]
    public static ToLua_LuaInterface_EventObject operator +(ToLua_LuaInterface_EventObject a, ToLua_LuaInterface_EventObject b)
    {
        return null;
    }

    [UseDefinedAttribute]
    public static ToLua_LuaInterface_EventObject operator -(ToLua_LuaInterface_EventObject a, ToLua_LuaInterface_EventObject b)
    {
        return null;
    }
}
