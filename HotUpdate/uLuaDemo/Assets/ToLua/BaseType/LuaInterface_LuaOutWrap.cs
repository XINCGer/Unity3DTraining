using System;
using LuaInterface;

public class LuaInterface_LuaOutWrap
{
    public static void Register(LuaState L)
    {
        L.BeginPreLoad();
        L.RegFunction("tolua.out", LuaOpen_ToLua_Out);
        L.EndPreLoad();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_ToLua_Out(IntPtr L)
    {
        try
        {
            LuaDLL.lua_newtable(L);

            RawSetOutType<int>(L);
            RawSetOutType<uint>(L);
            RawSetOutType<float>(L);
            RawSetOutType<double>(L);
            RawSetOutType<long>(L);
            RawSetOutType<ulong>(L);
            RawSetOutType<byte>(L);
            RawSetOutType<sbyte>(L);
            RawSetOutType<char>(L);
            RawSetOutType<short>(L);
            RawSetOutType<ushort>(L);
            RawSetOutType<bool>(L);
            RawSetOutType<string>(L);

            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static void RawSetOutType<T>(IntPtr L)
    {
        string str = TypeTraits<T>.GetTypeName();
        LuaDLL.lua_pushstring(L, str);
        ToLua.PushOut<T>(L, new LuaOut<T>());
        LuaDLL.lua_rawset(L, -3);
    }
}
