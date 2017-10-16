using UnityEngine;
using System.Collections;
using LuaInterface;

public class ToLua_UnityEngine_Object     
{        
    public static string DestroyDefined =
@"        try
        {
            int count = LuaDLL.lua_gettop(L);

            if (count == 1)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                ToLua.Destroy(L);
                UnityEngine.Object.Destroy(arg0);
                return 0;
            }
            else if (count == 2)
            {                
                float arg1 = (float)LuaDLL.luaL_checknumber(L, 2);
                int udata = LuaDLL.tolua_rawnetobj(L, 1);
                ObjectTranslator translator = LuaState.GetTranslator(L);
                translator.DelayDestroy(udata, arg1);
                return 0;
            }
            else
            {
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: Object.Destroy"");
            }
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";
    
    public static string DestroyImmediateDefined =
@"        try
        {
            int count = LuaDLL.lua_gettop(L);

            if (count == 1)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                ToLua.Destroy(L);
                UnityEngine.Object.DestroyImmediate(arg0);
                return 0;
            }
            else if (count == 2)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
                ToLua.Destroy(L);
                UnityEngine.Object.DestroyImmediate(arg0, arg1);
                return 0;
            }
            else
            {
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: Object.DestroyImmediate"");
            }
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }";

    public static string InstantiateDefined =
@"		IntPtr L0 = LuaException.L;

        try
        {
            ++LuaException.InstantiateCount;            
            LuaException.L = L;
            int count = LuaDLL.lua_gettop(L);

            if (count == 1)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                UnityEngine.Object o = UnityEngine.Object.Instantiate(arg0);

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                else
                {
                    ToLua.Push(L, o);
                }

                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return 1;
            }
#if UNITY_5_4_OR_NEWER
            else if (count == 2)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
                UnityEngine.Object o = UnityEngine.Object.Instantiate(arg0, arg1);

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                else
                {
                    ToLua.Push(L, o);
                }

                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return 1;
            }
#endif
            else if (count == 3 && TypeChecker.CheckTypes<UnityEngine.Vector3, UnityEngine.Quaternion>(L, 2))
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                UnityEngine.Vector3 arg1 = ToLua.ToVector3(L, 2);
                UnityEngine.Quaternion arg2 = ToLua.ToQuaternion(L, 3);
                UnityEngine.Object o = UnityEngine.Object.Instantiate(arg0, arg1, arg2);

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                else
                {
                    ToLua.Push(L, o);
                }

                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return 1;
            }
#if UNITY_5_4_OR_NEWER
            else if (count == 3 && TypeChecker.CheckTypes<UnityEngine.Transform, bool>(L, 2))
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.ToObject(L, 2);
                bool arg2 = LuaDLL.lua_toboolean(L, 3);
                UnityEngine.Object o = UnityEngine.Object.Instantiate(arg0, arg1, arg2);

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                else
                {
                    ToLua.Push(L, o);
                }

                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return 1;
            }
            else if (count == 4)
            {
                UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.CheckObject<UnityEngine.Object>(L, 1);
                UnityEngine.Vector3 arg1 = ToLua.CheckVector3(L, 2);
                UnityEngine.Quaternion arg2 = ToLua.CheckQuaternion(L, 3);
                UnityEngine.Transform arg3 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 4);
                UnityEngine.Object o = UnityEngine.Object.Instantiate(arg0, arg1, arg2, arg3);

                if (LuaDLL.lua_toboolean(L, LuaDLL.lua_upvalueindex(1)))
                {
                    string error = LuaDLL.lua_tostring(L, -1);
                    LuaDLL.lua_pop(L, 1);
                    throw new LuaException(error, LuaException.GetLastError());
                }
                else
                {
                    ToLua.Push(L, o);
                }

                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return 1;
            }
#endif
            else
            {
                LuaException.L = L0;
                --LuaException.InstantiateCount;
                return LuaDLL.luaL_throw(L, ""invalid arguments to method: UnityEngine.Object.Instantiate"");
            }
        }
        catch (Exception e)
        {
            LuaException.L = L0;
            --LuaException.InstantiateCount;
            return LuaDLL.toluaL_exception(L, e);
        }";


    [UseDefinedAttribute]
    public static void Destroy(Object obj)
    {
        
    }

    [UseDefinedAttribute]
    public static void DestroyImmediate(Object obj)
    {

    }

    [NoToLuaAttribute]
    public static void DestroyObject(Object obj)
    {

    }

    [NoToLuaAttribute]
    public static void DestroyObject(Object obj, float t)
    {

    }

    [UseDefinedAttribute]
    public static void Instantiate(Object original)
    {

    }
}
