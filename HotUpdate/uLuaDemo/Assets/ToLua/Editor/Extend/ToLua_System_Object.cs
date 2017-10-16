using System;
using LuaInterface;

public class ToLua_System_Object 
{
    public static string DestroyDefined = "\t\treturn ToLua.Destroy(L);";

    [UseDefinedAttribute]
    public static void Destroy(object obj)
    {
    }

    public static bool op_Equality(Object x, Object y)
    {
        return false;
    }
}
