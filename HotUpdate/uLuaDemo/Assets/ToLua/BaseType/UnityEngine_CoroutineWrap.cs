using System;
using LuaInterface;

public class UnityEngine_CoroutineWrap
{
    public static void Register(LuaState L)
    {
        L.BeginClass(typeof(UnityEngine.Coroutine), null);
        L.RegFunction("__tostring", ToLua.op_ToString);
        L.EndClass();
    }
}
