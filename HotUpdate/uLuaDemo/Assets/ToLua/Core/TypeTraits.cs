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
    public static class TypeTraits<T>
    {        
        static public Func<IntPtr, int, bool> Check = DefaultCheck;
        static public Type type = typeof(T);
        static public bool IsValueType = type.IsValueType;
        static public bool IsArray = type.IsArray;

        static string typeName = string.Empty;                
        static int nilType = -1;
        static int metaref = -1;

        static public void Init(Func<IntPtr, int, bool> check)
        {            
            if (check != null)
            {
                Check = check;
            }
        }

        static public string GetTypeName()
        {
            if (typeName == string.Empty)
            {
                typeName = LuaMisc.GetTypeName(type);
            }

            return typeName;
        }

        static public int GetLuaReference(IntPtr L)
        {
#if MULTI_STATE
            return LuaStatic.GetMetaReference(L, type);
#else
            if (metaref > 0)
            {                
                return metaref;
            }

            metaref = LuaStatic.GetMetaReference(L, type);

            if (metaref > 0)
            {
                LuaState.Get(L).OnDestroy += () => { metaref = -1; };
            }

            return metaref;
#endif
        }   

        static bool DefaultCheck(IntPtr L, int pos)
        {            
            LuaTypes luaType = LuaDLL.lua_type(L, pos);

            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return IsNilType();
                case LuaTypes.LUA_TUSERDATA:
                    return IsUserData(L, pos);
                case LuaTypes.LUA_TTABLE:
                    return IsUserTable(L, pos);
                default:
                    return false;
            }            
        }

        static bool IsNilType()
        {
            if (nilType != -1)
            {
                return nilType != 0;
            }

            if (!IsValueType)
            {
                nilType = 1;
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                nilType = 1;
                return true;
            }

            nilType = 0;
            return false;            
        }

        static bool IsUserData(IntPtr L, int pos)
        {
            object obj = null;
            int udata = LuaDLL.tolua_rawnetobj(L, pos);

            if (udata != -1)
            {
                ObjectTranslator translator = ObjectTranslator.Get(L);
                obj = translator.GetObject(udata);

                if (obj != null)
                {
                    return obj is T;
                }
                else
                {
                    return !IsValueType;
                }
            }

            return false;
        }

        static bool IsUserTable(IntPtr L, int pos)
        {            
            if (type == typeof(LuaTable))
            {
                return true;
            }
            else if (type.IsArray)
            {
                if (type.GetElementType().IsArray || type.GetArrayRank() > 1)
                {
                    return false;
                }

                return true;
            }
            else if (LuaDLL.tolua_isvptrtable(L, pos))
            {
                return IsUserData(L, pos);
            }

            return false;
        }
    }    

    public static class DelegateTraits<T>
    {        
        static DelegateFactory.DelegateCreate _Create = null;        

        static public void Init(DelegateFactory.DelegateCreate func)
        {
            _Create = func;            
        }

        static public Delegate Create(LuaFunction func)
        {
#if UNITY_EDITOR
            if (_Create == null)
            {
                throw new LuaException(string.Format("Delegate {0} not register", TypeTraits<T>.GetTypeName()));
            }
#endif
            if (func != null)
            {
                LuaState state = func.GetLuaState();
                LuaDelegate target = state.GetLuaDelegate(func);

                if (target != null)
                {
                    return Delegate.CreateDelegate(typeof(T), target, target.method);
                }
                else
                {
                    Delegate d = _Create(func, null, false);
                    target = d.Target as LuaDelegate;
                    state.AddLuaDelegate(target, func);
                    return d;
                }
            }

            return _Create(null, null, false);            
        }

        static public Delegate Create(LuaFunction func, LuaTable self)
        {
#if UNITY_EDITOR
            if (_Create == null)
            {
                throw new LuaException(string.Format("Delegate {0} not register", TypeTraits<T>.GetTypeName()));
            }
#endif
            if (func != null)
            {
                LuaState state = func.GetLuaState();
                LuaDelegate target = state.GetLuaDelegate(func, self);

                if (target != null)
                {
                    return Delegate.CreateDelegate(typeof(T), target, target.method);
                }
                else
                {
                    Delegate d = _Create(func, self, true);
                    target = d.Target as LuaDelegate;
                    state.AddLuaDelegate(target, func, self);
                    return d;
                }
            }

            return _Create(null, null, true);            
        }
    }

    public static class StackTraits<T>
    {
        static public Action<IntPtr, T> Push = SelectPush();
        static public Func<IntPtr, int, T> Check = DefaultCheck;
        static public Func<IntPtr, int, T> To = DefaultTo;               

        static public void Init(Action<IntPtr, T> push, Func<IntPtr, int, T> check, Func<IntPtr, int, T> to)
        {
            if (push != null)
            {
                Push = push;
            }

            if (to != null)
            {
                To = to;
            }

            if (check != null)
            {
                Check = check;
            }            
        }

        static Action<IntPtr, T> SelectPush()
        {
            if (TypeTraits<T>.IsValueType)
            {
                return PushValue;
            }
            else if (TypeTraits<T>.IsArray)
            {
                return PushArray;
            }
            else
            {
                return PushObject;
            }
        }

        static void PushValue(IntPtr L, T o)
        {
            ToLua.PushStruct(L, o);
        }

        static void PushObject(IntPtr L, T o)
        {
            ToLua.PushObject(L, o);
        }

        static void PushArray(IntPtr L, T array)
        {
            if (array == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                int arrayMetaTable = LuaStatic.GetArrayMetatable(L);
                ToLua.PushUserData(L, array, arrayMetaTable);
            }
        }

        static T DefaultTo(IntPtr L, int pos)
        {
            return (T)ToLua.ToObject(L, pos);
        }           
        
        static T DefaultCheck(IntPtr L, int stackPos)
        {
            int udata = LuaDLL.tolua_rawnetobj(L, stackPos);            

            if (udata != -1)
            {
                ObjectTranslator translator = ObjectTranslator.Get(L);
                object obj = translator.GetObject(udata);

                if (obj != null)
                {                    
                    if (obj is T)
                    {
                        return (T)obj;
                    }

                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", TypeTraits<T>.GetTypeName(), obj.GetType().FullName));
                }

                if (!TypeTraits<T>.IsValueType)
                {
                    return default(T);
                }
            }
            else if (LuaDLL.lua_isnil(L, stackPos) && !TypeTraits<T>.IsValueType)
            {
                return default(T);
            }

            LuaDLL.luaL_typerror(L, stackPos, TypeTraits<T>.GetTypeName());
            return default(T);            
        }
    }
}
