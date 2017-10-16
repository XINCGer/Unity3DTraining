using UnityEngine;
using System;
using LuaInterface;

public class TestInstantiate2 : MonoBehaviour 
{
    void Awake()
    {
        try
        {
            throw new Exception("Instantiate exception 2");
        }
        catch (Exception e)
        {
            LuaState state = LuaState.Get(IntPtr.Zero);
            state.ThrowLuaException(e);
        }
    }
}
