/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.IO;
using System.Text;

namespace LuaInterface
{
    public static class LuaStatic
    {
        public static int GetMetaReference(IntPtr L, Type t)
        {
            LuaState state = LuaState.Get(L);
            return state.GetMetaReference(t);
        }

        public static int GetMissMetaReference(IntPtr L, Type t)
        {
            LuaState state = LuaState.Get(L);
            return state.GetMissMetaReference(t);
        }

        public static Type GetClassType(IntPtr L, int reference)
        {
            LuaState state = LuaState.Get(L);
            return state.GetClassType(reference);
        }

        public static LuaFunction GetFunction(IntPtr L, int reference)
        {
            LuaState state = LuaState.Get(L);
            return state.GetFunction(reference);
        }

        public static LuaTable GetTable(IntPtr L, int reference)
        {
            LuaState state = LuaState.Get(L);
            return state.GetTable(reference);
        }

        public static LuaThread GetLuaThread(IntPtr L, int reference)
        {
            LuaState state = LuaState.Get(L);
            return state.GetLuaThread(reference);
        }

        public static void GetUnpackRayRef(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.UnpackRay);
        }

        public static void GetUnpackBounds(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.UnpackBounds);
        }

        public static void GetPackRay(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.PackRay);
        }

        public static void GetPackRaycastHit(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.PackRaycastHit);
        }

        public static void GetPackTouch(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.PackTouch);
        }

        public static void GetPackBounds(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_getref(L, state.PackBounds);
        }
        
        public static int GetArrayMetatable(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            return state.ArrayMetatable;
        }

        public static int GetTypeMetatable(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            return state.TypeMetatable;
        }        

        public static int GetDelegateMetatable(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            return state.DelegateMetatable;
        }

        public static int GetEventMetatable(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            return state.EventMetatable;
        }

        public static int GetIterMetatable(IntPtr L)
        {
            LuaState state = LuaState.Get(L);
            return state.IterMetatable;
        }

        public static int GetEnumObject(IntPtr L, System.Enum e, out object obj)
        {
            LuaState state = LuaState.Get(L);
            obj = state.GetEnumObj(e);
            return state.EnumMetatable;
        }        

        public static LuaCSFunction GetPreModule(IntPtr L, Type t)
        {
            LuaState state = LuaState.Get(L);
            return state.GetPreModule(t);
        }
    }
}