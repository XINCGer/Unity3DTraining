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
using System.Runtime.InteropServices;
using System.Collections;

namespace LuaInterface
{
    public class LuaStackOp
    {
        public sbyte ToSByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public byte ToByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public short ToInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public ushort ToUInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public char ToChar(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public int ToInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public uint ToUInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public decimal ToDecimal(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public float ToFloat(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public LuaByteBuffer ToLuaByteBuffer(IntPtr L, int stackPos)
        {
            return new LuaByteBuffer(ToLua.ToByteBuffer(L, stackPos));
        }   

        public IEnumerator ToIter(IntPtr L, int stackPos)
        {
            return (IEnumerator)ToLua.ToObject(L, stackPos);
        }

        public Type ToType(IntPtr L, int stackPos)
        {
            return (Type)ToLua.ToObject(L, stackPos);
        }

        public EventObject ToEventObject(IntPtr L, int stackPos)
        {
            return (EventObject)ToLua.ToObject(L, stackPos);
        }

        public Transform ToTransform(IntPtr L, int stackPos)
        {
            return (Transform)ToLua.ToObject(L, stackPos);
        }

        public GameObject ToGameObject(IntPtr L, int stackPos)
        {
            return (GameObject)ToLua.ToObject(L, stackPos);
        }

        public object ToObject(IntPtr L, int stackPos)
        {
            return ToLua.ToObject(L, stackPos);
        }

        public sbyte CheckSByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public byte CheckByte(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public short CheckInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public ushort CheckUInt16(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public char CheckChar(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public int CheckInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public uint CheckUInt32(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public decimal CheckDecimal(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }     

        public float CheckFloat(IntPtr L, int stackPos)
        {
            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public IntPtr CheckIntPtr(IntPtr L, int stackPos)
        {
            LuaTypes luaType = LuaDLL.lua_type(L, stackPos);

            switch(luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return IntPtr.Zero;
                case LuaTypes.LUA_TLIGHTUSERDATA:
                    return LuaDLL.lua_touserdata(L, stackPos);
                default:
                    LuaDLL.luaL_typerror(L, stackPos, "IntPtr");
                    return IntPtr.Zero;
            }            
        }

        public UIntPtr CheckUIntPtr(IntPtr L, int stackPos)
        {
            throw new LuaException("NYI");
        }

        public LuaByteBuffer CheckLuaByteBuffer(IntPtr L, int stackPos)
        {
            return new LuaByteBuffer(ToLua.CheckByteBuffer(L, stackPos));
        }

        public EventObject CheckEventObject(IntPtr L, int stackPos)
        {
            return (EventObject)ToLua.CheckObject(L, stackPos, typeof(EventObject));
        }

        public Transform CheckTransform(IntPtr L, int stackPos)
        {
            return (Transform)ToLua.CheckObject(L, stackPos, typeof(Transform));
        }

        public GameObject CheckGameObject(IntPtr L, int stackPos)
        {
            return (GameObject)ToLua.CheckObject(L, stackPos, typeof(GameObject));
        }

        public void Push(IntPtr L, sbyte n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, byte n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, short n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, ushort n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, char n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, int n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, uint n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, decimal n)
        {
            LuaDLL.lua_pushnumber(L, (double)n);
        }

        public void Push(IntPtr L, float n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void Push(IntPtr L, UIntPtr p)
        {
            throw new LuaException("NYI");
        }

        public void Push(IntPtr L, Delegate ev)
        {
            ToLua.Push(L, ev);
        }

        public void Push(IntPtr L, object obj)
        {
            ToLua.Push(L, obj);
        }

        public void Push(IntPtr L, GameObject o)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                int reference = TypeTraits<GameObject>.GetLuaReference(L);

                if (reference <= 0)
                {
                    reference = ToLua.LoadPreType(L, typeof(GameObject));
                }

                ToLua.PushUserData(L, o, reference);
            }
        }

        public void Push(IntPtr L, Transform o)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                Type type = o.GetType();
                int reference = -1;

                if (type == typeof(Transform))
                {
                    reference = TypeTraits<Transform>.GetLuaReference(L);
                }
                else
                {
                    reference = LuaStatic.GetMetaReference(L, type);
                }

                if (reference <= 0)
                {
                    reference = ToLua.LoadPreType(L, type);
                }

                ToLua.PushUserData(L, o, reference);
            }
        }

        #region Nullable
        public Nullable<sbyte> ToNullSByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public Nullable<byte> ToNullByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public Nullable<short> ToNullInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public Nullable<ushort> ToNullUInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public Nullable<char> ToNullChar(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public Nullable<int> ToNullInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public Nullable<uint> ToNullUInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public Nullable<decimal> ToNullDecimal(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public Nullable<float> ToNullFloat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.lua_tonumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public Nullable<double> ToNullNumber(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.lua_tonumber(L, stackPos);
        }

        public Nullable<bool> ToNullBool(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.lua_toboolean(L, stackPos);
        }

        public Nullable<long> ToNullInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_toint64(L, stackPos);
        }

        public Nullable<ulong> ToNullUInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_touint64(L, stackPos);
        }

        public sbyte[] ToSByteArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<sbyte>(L, stackPos);
        }

        public short[] ToInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<short>(L, stackPos);
        }

        public ushort[] ToUInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<ushort>(L, stackPos);
        }

        public decimal[] ToDecimalArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<decimal>(L, stackPos);
        }

        public float[] ToFloatArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<float>(L, stackPos);
        }

        public double[] ToDoubleArray(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<double>(L, stackPos);
        }

        public int[] ToInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<int>(L, stackPos);
        }

        public uint[] ToUInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.ToNumberArray<uint>(L, stackPos);
        }

        public long[] ToInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<long>(L, stackPos);
        }

        public ulong[] ToUInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<ulong>(L, stackPos);
        }

        public Nullable<Vector3> ToNullVec3(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            float x = 0, y = 0, z = 0;
            LuaDLL.tolua_getvec3(L, stackPos, out x, out y, out z);
            return new Vector3(x, y, z);
        }

        public Nullable<Quaternion> ToNullQuat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            float x, y, z, w;
            LuaDLL.tolua_getquat(L, stackPos, out x, out y, out z, out w);
            return new Quaternion(x, y, z, w);
        }

        public Nullable<Vector2> ToNullVec2(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            float x, y;
            LuaDLL.tolua_getvec2(L, stackPos, out x, out y);
            return new Vector2(x, y);
        }        

        public Nullable<Color> ToNullColor(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            float r, g, b, a;
            LuaDLL.tolua_getclr(L, stackPos, out r, out g, out b, out a);
            return new Color(r, g, b, a);
        }      

        public Nullable<Vector4> ToNullVec4(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            float x, y, z, w;
            LuaDLL.tolua_getvec4(L, stackPos, out x, out y, out z, out w);
            return new Vector4(x, y, z, w);
        }   

        public Nullable<Ray> ToNullRay(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.ToRay(L, stackPos);
        }  

        public Nullable<Bounds> ToNullBounds(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.ToBounds(L, stackPos);
        }      

        public Nullable<LayerMask> ToNullLayerMask(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_getlayermask(L, stackPos);
        }

        public Vector3[] ToVec3Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<Vector3>(L, stackPos);
        }

        public Quaternion[] ToQuatArray(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<Quaternion>(L, stackPos);
        }

        public Vector2[] ToVec2Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<Vector2>(L, stackPos);
        }

        public Color[] ToColorArray(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<Color>(L, stackPos);
        }

        public Vector4[] ToVec4Array(IntPtr L, int stackPos)
        {
            return ToLua.ToStructArray<Vector4>(L, stackPos);
        }

        public Type[] ToTypeArray(IntPtr L, int stackPos)
        {
            return ToLua.ToObjectArray<Type>(L, stackPos);
        }

        public Nullable<sbyte> CheckNullSByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSByte(ret);
        }

        public Nullable<byte> CheckNullByte(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToByte(ret);
        }

        public Nullable<short> CheckNullInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt16(ret);
        }

        public Nullable<ushort> CheckNullUInt16(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt16(ret);
        }

        public Nullable<char> CheckNullChar(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToChar(ret);
        }

        public Nullable<int> CheckNullInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToInt32(ret);
        }

        public Nullable<uint> CheckNullUInt32(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToUInt32(ret);
        }

        public Nullable<decimal> CheckNullDecimal(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToDecimal(ret);
        }

        public Nullable<float> CheckNullFloat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            double ret = LuaDLL.luaL_checknumber(L, stackPos);
            return Convert.ToSingle(ret);
        }

        public Nullable<double> CheckNullNumber(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.luaL_checknumber(L, stackPos);
        }

        public Nullable<bool> CheckNullBool(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.luaL_checkboolean(L, stackPos);
        }

        public Nullable<long> CheckNullInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_checkint64(L, stackPos);
        }

        public Nullable<ulong> CheckNullUInt64(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return LuaDLL.tolua_checkuint64(L, stackPos);
        }

        public sbyte[] CheckSByteArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<sbyte>(L, stackPos);
        }

        public short[] CheckInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<short>(L, stackPos);
        }

        public ushort[] CheckUInt16Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<ushort>(L, stackPos);
        }

        public decimal[] CheckDecimalArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<decimal>(L, stackPos);
        }

        public float[] CheckFloatArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<float>(L, stackPos);
        }

        public double[] CheckDoubleArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<double>(L, stackPos);
        }

        public int[] CheckInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<int>(L, stackPos);
        }

        public uint[] CheckUInt32Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckNumberArray<uint>(L, stackPos);
        }

        public long[] CheckInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<long>(L, stackPos);
        }

        public ulong[] CheckUInt64Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<ulong>(L, stackPos);
        }

        public Nullable<Vector3> CheckNullVec3(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckVector3(L, stackPos);
        }

        public Nullable<Quaternion> CheckNullQuat(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckQuaternion(L, stackPos);
        }     

        public Nullable<Vector2> CheckNullVec2(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckVector2(L, stackPos);
        }   

        public Nullable<Color> CheckNullColor(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckColor(L, stackPos);
        }    

        public Nullable<Vector4> CheckNullVec4(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckVector4(L, stackPos);
        }      

        public Nullable<Ray> CheckNullRay(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckRay(L, stackPos);
        }       

        public Nullable<Bounds> CheckNullBounds(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckBounds(L, stackPos);
        }    

        public Nullable<LayerMask> CheckNullLayerMask(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) == LuaTypes.LUA_TNIL)
            {
                return null;
            }

            return ToLua.CheckLayerMask(L, stackPos);
        }

        public Vector3[] CheckVec3Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<Vector3>(L, stackPos);
        }

        public Quaternion[] CheckQuatArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<Quaternion>(L, stackPos);
        }

        public Vector2[] CheckVec2Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<Vector2>(L, stackPos);
        }

        public Color[] CheckColorArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<Color>(L, stackPos);
        }

        public Vector4[] CheckVec4Array(IntPtr L, int stackPos)
        {
            return ToLua.CheckStructArray<Vector4>(L, stackPos);
        }        

        public Type[] CheckTypeArray(IntPtr L, int stackPos)
        {
            return ToLua.CheckObjectArray<Type>(L, stackPos);
        }

        public void Push(IntPtr L, Nullable<sbyte> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<byte> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<short> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<ushort> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<char> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<int> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<uint> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<decimal> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, Convert.ToDouble(n.Value));
            }
        }

        public void Push(IntPtr L, Nullable<float> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<double> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushnumber(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<bool> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.lua_pushboolean(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<long> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.tolua_pushint64(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<ulong> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.tolua_pushuint64(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<Vector3> v3)
        {
            if (v3 == null)
            {
                LuaDLL.lua_pushnil(L);
                return;
            }
            else
            {
                Vector3 v = v3.Value;
                LuaDLL.tolua_pushvec3(L, v.x, v.y, v.z);
            }
        }

        public void Push(IntPtr L, Nullable<Quaternion> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                Quaternion q = n.Value;
                LuaDLL.tolua_pushquat(L, q.x, q.y, q.z, q.w);
            }
        }

        public void Push(IntPtr L, Nullable<Vector2> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                Vector2 v2 = n.Value;
                LuaDLL.tolua_pushvec2(L, v2.x, v2.y);
            }
        }

        public void Push(IntPtr L, Nullable<Color> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                Color clr = n.Value;
                LuaDLL.tolua_pushclr(L, clr.r, clr.g, clr.b, clr.a);
            }
        }

        public void Push(IntPtr L, Nullable<Vector4> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                Vector4 v4 = n.Value;
                LuaDLL.tolua_pushvec4(L, v4.x, v4.y, v4.z, v4.w);
            }
        }

        public void Push(IntPtr L, Nullable<Ray> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                ToLua.Push(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<Bounds> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                ToLua.Push(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<LayerMask> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                LuaDLL.tolua_pushlayermask(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<Touch> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                ToLua.Push(L, n.Value);
            }
        }

        public void Push(IntPtr L, Nullable<RaycastHit> n)
        {
            if (n == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                ToLua.Push(L, n.Value);
            }
        }
        #endregion
    }
}
