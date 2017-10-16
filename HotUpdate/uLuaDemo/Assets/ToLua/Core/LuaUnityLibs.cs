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
using System.Runtime.InteropServices;
using UnityEngine;

namespace LuaInterface
{
    public sealed class LuaUnityLibs
    {
        public static void OpenLibs(IntPtr L)
        {
            InitMathf(L);
            InitLayer(L);
        }

        public static void OpenLuaLibs(IntPtr L)
        {                        
            if (LuaDLL.tolua_openlualibs(L) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_pop(L, 1);
                throw new LuaException(error);
            }

            SetOutMethods(L, "Vector3", GetOutVector3);
            SetOutMethods(L, "Vector2", GetOutVector2);
            SetOutMethods(L, "Vector4", GetOutVector4);
            SetOutMethods(L, "Color", GetOutColor);
            SetOutMethods(L, "Quaternion", GetOutQuaternion);
            SetOutMethods(L, "Ray", GetOutRay);
            SetOutMethods(L, "Bounds", GetOutBounds);
            SetOutMethods(L, "Touch", GetOutTouch);
            SetOutMethods(L, "RaycastHit", GetOutRaycastHit);
            SetOutMethods(L, "LayerMask", GetOutLayerMask);            
        }

        static void InitMathf(IntPtr L)
        {
            LuaDLL.lua_getglobal(L, "Mathf");
            LuaDLL.lua_pushstring(L, "PerlinNoise");
            LuaDLL.tolua_pushcfunction(L, PerlinNoise);
            LuaDLL.lua_rawset(L, -3);
            LuaDLL.lua_pop(L, 1);
        }

        static void InitLayer(IntPtr L)
        {
            LuaDLL.tolua_createtable(L, "Layer");

            for (int i = 0; i < 32; i++)
            {
                string str = LayerMask.LayerToName(i);

                if (!string.IsNullOrEmpty(str))
                {
                    LuaDLL.lua_pushstring(L, str);
                    LuaDLL.lua_pushinteger(L, i);
                    LuaDLL.lua_rawset(L, -3);
                }
            }

            LuaDLL.lua_pop(L, 1);
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int PerlinNoise(IntPtr L)
        {
            try
            {
                float x = (float)LuaDLL.luaL_checknumber(L, 1);
                float y = (float)LuaDLL.luaL_checknumber(L, 2);
                float ret = Mathf.PerlinNoise(x, y);
                LuaDLL.lua_pushnumber(L, ret);
                return 1;
            }
            catch (Exception e)
            {
                return LuaDLL.toluaL_exception(L, e);
            }            
        }

        static void SetOutMethods(IntPtr L, string table, LuaCSFunction getOutFunc = null)
        {
            LuaDLL.lua_getglobal(L, table);
            IntPtr get = Marshal.GetFunctionPointerForDelegate(getOutFunc);
            LuaDLL.tolua_variable(L, "out", get, IntPtr.Zero);
            
            LuaDLL.lua_pop(L, 1);
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutVector3(IntPtr L)
        {            
            ToLua.PushOut<Vector3>(L, new LuaOut<Vector3>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutVector2(IntPtr L)
        {            
            ToLua.PushOut<Vector2>(L, new LuaOut<Vector2>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutVector4(IntPtr L)
        {            
            ToLua.PushOut<Vector4>(L, new LuaOut<Vector4>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutColor(IntPtr L)
        {            
            ToLua.PushOut<Color>(L, new LuaOut<Color>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutQuaternion(IntPtr L)
        {            
            ToLua.PushOut<Quaternion>(L, new LuaOut<Quaternion>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutRay(IntPtr L)
        {
            ToLua.PushOut<Ray>(L, new LuaOut<Ray>());
            return 1;            
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutBounds(IntPtr L)
        {
            ToLua.PushOut<Bounds>(L, new LuaOut<Bounds>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutRaycastHit(IntPtr L)
        {
            ToLua.PushOut<RaycastHit>(L, new LuaOut<RaycastHit>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutTouch(IntPtr L)
        {            
            ToLua.PushOut<Touch>(L, new LuaOut<Touch>());
            return 1;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int GetOutLayerMask(IntPtr L)
        {
            ToLua.PushOut<LayerMask>(L, new LuaOut<LayerMask>());
            return 1;
        }
    }
}
