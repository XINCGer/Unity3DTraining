using System;
using LuaInterface;

public class ToLua_LuaInterface_LuaField
{
    public static string GetDefined =
@"		try
		{			
			LuaField obj = (LuaField)ToLua.CheckObject(L, 1, typeof(LuaField));            
            return obj.Get(L);						
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}";

    public static string SetDefined =
@"		try
		{			
            LuaField obj = (LuaField)ToLua.CheckObject(L, 1, typeof(LuaField));            
            return obj.Set(L);
        }
        catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}";

    [UseDefinedAttribute]
    public int Set(IntPtr L)
    {
        return 0;
    }

    [UseDefinedAttribute]
    public int Get(IntPtr L)
    {
        return 0;
    }
}
