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
using UnityEngine;
using System;
using System.Collections;

namespace LuaInterface
{
    public class LuaMatchType
    {
        public bool CheckNumber(IntPtr L, int pos)
        {            
            return LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TNUMBER;
        }

        public bool CheckBool(IntPtr L, int pos)
        {
            return LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TBOOLEAN;
        }

        public bool CheckLong(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNUMBER:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Int64;                    
                default:
                    return false;
            }                        
        }

        public bool CheckULong(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNUMBER:
                    return LuaDLL.lua_tonumber(L, pos) >= 0;
                case LuaTypes.LUA_TUSERDATA:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.UInt64;                    
                default:
                    return false;
            }
        }

        public bool CheckNullNumber(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TNUMBER || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckNullBool(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TBOOLEAN || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckNullLong(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TNUMBER:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Int64;                    
                default:
                    return false;
            }
        }

        public bool CheckNullULong(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TNUMBER:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.UInt64;                    
                default:
                    return false;
            }
        }

        public bool CheckString(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TSTRING:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(typeof(string), L, pos);
                default:
                    return false;
            }            
        }

        public bool CheckByteArray(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TSTRING:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(typeof(byte[]), L, pos);
                default:
                    return false;
            }
        }

        public bool CheckCharArray(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TSTRING:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(typeof(char[]), L, pos);
                default:
                    return false;
            }
        }

        public bool CheckArray(Type t, IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:                                
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(t, L, pos);
                default:
                    return false;
            }
        }

        public bool CheckBoolArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(bool[]), L, pos);
        }

        public bool CheckSByteArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(sbyte[]), L, pos);
        }

        public bool CheckInt16Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(short[]), L, pos);
        }

        public bool CheckUInt16Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(ushort[]), L, pos);
        }

        public bool CheckDecimalArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(decimal[]), L, pos);
        }

        public bool CheckSingleArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(float[]), L, pos);
        }

        public bool CheckDoubleArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(double[]), L, pos);
        }

        public bool CheckInt32Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(int[]), L, pos);
        }

        public bool CheckUInt32Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(uint[]), L, pos);
        }

        public bool CheckInt64Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(long[]), L, pos);
        }

        public bool CheckUInt64Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(ulong[]), L, pos);
        }

        public bool CheckStringArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(string[]), L, pos);
        }

        public bool CheckTypeArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(Type[]), L, pos);
        }

        public bool CheckObjectArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(object[]), L, pos);
        }        

        bool CheckValueType(IntPtr L, int pos, int valueType, Type nt)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                int vt = LuaDLL.tolua_getvaluetype(L, pos);                
                return vt == valueType;
            }

            return false;
        }

        public bool CheckVec3(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector3;
            }

            return false;            
        }

        public bool CheckQuat(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Quaternion;
            }

            return false;            
        }

        public bool CheckVec2(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector2;
            }

            return false;            
        }

        public bool CheckColor(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Color;
            }

            return false;            
        }

        public bool CheckVec4(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector4;
            }

            return false;            
        }

        public bool CheckRay(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Ray;
            }

            return false;            
        }

        public bool CheckBounds(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Bounds;
            }

            return false;            
        }

        public bool CheckTouch(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Touch;
            }

            return false;            
        }

        public bool CheckLayerMask(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.LayerMask;
            }

            return false;            
        }

        public bool CheckRaycastHit(IntPtr L, int pos)
        {
            if (LuaDLL.lua_type(L, pos) == LuaTypes.LUA_TTABLE)
            {
                return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.RaycastHit;
            }

            return false;            
        }

        public bool CheckNullVec3(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector3;
                default:
                    return false;
            }
        }

        public bool CheckNullQuat(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Quaternion;
                default:
                    return false;
            }
        }

        public bool CheckNullVec2(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector2;
                default:
                    return false;
            }
        }

        public bool CheckNullColor(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Color;
                default:
                    return false;
            }
        }

        public bool CheckNullVec4(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Vector4;
                default:
                    return false;
            }
        }

        public bool CheckNullRay(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Ray;
                default:
                    return false;
            }
        }

        public bool CheckNullBounds(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Bounds;
                default:
                    return false;
            }
        }

        public bool CheckNullTouch(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.Touch;
                default:
                    return false;
            }
        }

        public bool CheckNullLayerMask(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.LayerMask;
                default:
                    return false;
            }
        }

        public bool CheckNullRaycastHit(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return LuaDLL.tolua_getvaluetype(L, pos) == LuaValueType.RaycastHit;
                default:
                    return false;
            }
        }

        public bool CheckVec3Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(Vector3[]), L, pos);
        }

        public bool CheckQuatArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(Quaternion[]), L, pos);
        }

        public bool CheckVec2Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(Vector2[]), L, pos);
        }

        public bool CheckVec4Array(IntPtr L, int pos)
        {
            return CheckArray(typeof(Vector4[]), L, pos);
        }

        public bool CheckColorArray(IntPtr L, int pos)
        {
            return CheckArray(typeof(Color[]), L, pos);
        }

        public bool CheckPtr(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TLIGHTUSERDATA || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckLuaFunc(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TFUNCTION || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckLuaTable(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TTABLE || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckLuaThread(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TTHREAD || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckLuaBaseRef(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch(luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TFUNCTION:
                    return true;
                case LuaTypes.LUA_TTABLE:
                    return true;
                case LuaTypes.LUA_TTHREAD:
                    return true;
                default:
                    return false;
            }            
        }

        public bool CheckByteBuffer(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            return luaType == LuaTypes.LUA_TSTRING || luaType == LuaTypes.LUA_TNIL;
        }

        public bool CheckEventObject(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(typeof(EventObject), L, pos);
                default:
                    return false;
            }
        }

        public bool CheckEnumerator(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TUSERDATA:                    
                    int udata = LuaDLL.tolua_rawnetobj(L, pos);

                    if (udata != -1)
                    {
                        ObjectTranslator translator = ObjectTranslator.Get(L);
                        object obj = translator.GetObject(udata);
                        return obj == null ? true : obj is IEnumerator;
                    }
                    return false;                    
                default:
                    return false;
            }
        }

        //不存在派生类的类型
        bool CheckFinalType(Type type, IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(type, L, pos);
                default:
                    return false;
            }
        }

        public bool CheckGameObject(IntPtr L, int pos)
        {
            return CheckFinalType(typeof(GameObject), L, pos);
        }

        public bool CheckTransform(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TUSERDATA:                    
                    int udata = LuaDLL.tolua_rawnetobj(L, pos);

                    if (udata != -1)
                    {
                        ObjectTranslator translator = ObjectTranslator.Get(L);
                        object obj = translator.GetObject(udata);
                        return obj == null ? true : obj is Transform;
                    }

                    return false;                    
                default:
                    return false;
            }
        }

        static Type monoType = typeof(Type).GetType();

        public bool CheckMonoType(IntPtr L, int pos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return true;
                case LuaTypes.LUA_TUSERDATA:
                    return CheckClassType(monoType, L, pos);
                default:
                    return false;
            }            
        }        

        public bool CheckVariant(IntPtr L, int pos)
        {
            return true;
        }

        bool CheckClassType(Type t, IntPtr L, int pos)
        {            
            int udata = LuaDLL.tolua_rawnetobj(L, pos);

            if (udata != -1)
            {                
                ObjectTranslator translator = ObjectTranslator.Get(L);
                object obj = translator.GetObject(udata);
                return obj == null ? true : obj.GetType() == t;
            }

            return false;
        }
    }
}

