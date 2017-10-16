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
using System.Collections.Generic;
using UnityEngine;

namespace LuaInterface
{
    public class LuaFunction : LuaBaseRef
    {
        protected struct FuncData
        {
            public int oldTop;
            public int stackPos;

            public FuncData(int top, int stack)
            {
                oldTop = top;
                stackPos = stack;
            }
        }

        protected int oldTop = -1;
        private int argCount = 0;
        private int stackPos = -1;
        private Stack<FuncData> stack = new Stack<FuncData>();

        public LuaFunction(int reference, LuaState state)
        {
            this.reference = reference;
            this.luaState = state;
        }

        public override void Dispose()
        {
#if UNITY_EDITOR
            if (oldTop != -1 && count <= 1)
            {
                Debugger.LogError("You must call EndPCall before calling Dispose");
            }
#endif
            base.Dispose();
        }

        public T ToDelegate<T>() where T : class
        {
            return DelegateTraits<T>.Create(this) as T;
        }

        public virtual int BeginPCall()
        {
            if (luaState == null)
            {
                throw new LuaException("LuaFunction has been disposed");
            }

            stack.Push(new FuncData(oldTop, stackPos));
            oldTop = luaState.BeginPCall(reference);
            stackPos = -1;
            argCount = 0;
            return oldTop;
        }

        public void PCall()
        {
#if UNITY_EDITOR
            if (oldTop == -1)
            {
                Debugger.LogError("You must call BeginPCall before calling PCall");
            }
#endif

            stackPos = oldTop + 1;

            try
            {
                luaState.PCall(argCount, oldTop);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void EndPCall()
        {
            if (oldTop != -1)
            {
                luaState.EndPCall(oldTop);
                argCount = 0;
                FuncData data = stack.Pop();
                oldTop = data.oldTop;
                stackPos = data.stackPos;
            }
        }

        public void Call()
        {
            BeginPCall();
            PCall();
            EndPCall();
        }

        public void Call<T1>(T1 arg1)
        {
            BeginPCall();
            PushGeneric(arg1);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2>(T1 arg1, T2 arg2)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PushGeneric(arg8);
            PCall();
            EndPCall();
        }

        public void Call<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PushGeneric(arg8);
            PushGeneric(arg9);
            PCall();
            EndPCall();
        }

        public R1 Invoke<R1>()
        {
            BeginPCall();
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, R1>(T1 arg1)
        {
            BeginPCall();
            PushGeneric(arg1);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, R1>(T1 arg1, T2 arg2)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, R1>(T1 arg1, T2 arg2, T3 arg3)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, T5, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, T5, T6, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, T5, T6, T7, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, T5, T6, T7, T8, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PushGeneric(arg8);
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        public R1 Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, R1>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            BeginPCall();
            PushGeneric(arg1);
            PushGeneric(arg2);
            PushGeneric(arg3);
            PushGeneric(arg4);
            PushGeneric(arg5);
            PushGeneric(arg6);
            PushGeneric(arg7);
            PushGeneric(arg8);
            PushGeneric(arg9);            
            PCall();
            R1 ret1 = CheckValue<R1>();
            EndPCall();
            return ret1;
        }

        //慎用, 有gc alloc
        public object[] LazyCall(params object[] args)
        {
            BeginPCall();
            int count = args == null ? 0 : args.Length;

            if (!luaState.LuaCheckStack(count + 6))
            {
                EndPCall();
                throw new LuaException("stack overflow");
            }

            PushArgs(args);
            PCall();
            object[] objs = luaState.CheckObjects(oldTop);
            EndPCall();
            return objs;
        }

        public void CheckStack(int args)
        {
            luaState.LuaCheckStack(args + 6);
        }

        public bool IsBegin()
        {
            return oldTop != -1;
        }

        public void Push(double num)
        {
            luaState.Push(num);
            ++argCount;
        }

        public void Push(int n)
        {
            luaState.Push(n);
            ++argCount;
        }

        public void PushLayerMask(LayerMask n)
        {
            luaState.PushLayerMask(n);
            ++argCount;
        }

        public void Push(uint un)
        {
            luaState.Push(un);
            ++argCount;
        }

        public void Push(long num)
        {
            luaState.Push(num);
            ++argCount;
        }

        public void Push(ulong un)
        {
            luaState.Push(un);
            ++argCount;
        }

        public void Push(bool b)
        {
            luaState.Push(b);
            ++argCount;
        }

        public void Push(string str)
        {
            luaState.Push(str);
            ++argCount;
        }

        public void Push(IntPtr ptr)
        {
            luaState.Push(ptr);
            ++argCount;
        }

        public void Push(LuaBaseRef lbr)
        {
            luaState.Push(lbr);
            ++argCount;
        }

        public void Push(object o)
        {
            luaState.PushVariant(o);
            ++argCount;
        }

        public void Push(UnityEngine.Object o)
        {
            luaState.Push(o);
            ++argCount;
        }

        public void Push(Type t)
        {
            luaState.Push(t);
            ++argCount;
        }

        public void Push(Enum e)
        {
            luaState.Push(e);
            ++argCount;
        }

        public void Push(Array array)
        {
            luaState.Push(array);
            ++argCount;
        }

        public void Push(Vector3 v3)
        {
            luaState.Push(v3);
            ++argCount;
        }

        public void Push(Vector2 v2)
        {
            luaState.Push(v2);
            ++argCount;
        }

        public void Push(Vector4 v4)
        {
            luaState.Push(v4);
            ++argCount;
        }

        public void Push(Quaternion quat)
        {
            luaState.Push(quat);
            ++argCount;
        }

        public void Push(Color clr)
        {
            luaState.Push(clr);
            ++argCount;
        }

        public void Push(Ray ray)
        {
            try
            {
                luaState.Push(ray);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void Push(Bounds bounds)
        {
            try
            {
                luaState.Push(bounds);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void Push(RaycastHit hit)
        {
            try
            {
                luaState.Push(hit);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void Push(Touch t)
        {
            try
            {
                luaState.Push(t);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void Push(LuaByteBuffer buffer)
        {
            try
            {
                luaState.Push(buffer);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void PushValue<T>(T value) where T : struct
        {
            try
            {
                luaState.PushValue(value);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void PushObject(object o)
        {
            try
            {
                luaState.PushObject(o);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void PushSealed<T>(T o)
        {
            try
            {
                luaState.PushSealed(o);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void PushGeneric<T>(T t)
        {
            try
            {
                luaState.PushGeneric(t);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public void PushArgs(object[] args)
        {
            if (args == null)
            {
                return;
            }

            argCount += args.Length;
            luaState.PushArgs(args);
        }

        public void PushByteBuffer(byte[] buffer, int len = -1)
        {
            try
            {
                if (len == -1)
                {
                    len = buffer.Length;
                }

                luaState.PushByteBuffer(buffer, len);
                ++argCount;
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public double CheckNumber()
        {
            try
            {
                return luaState.LuaCheckNumber(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public bool CheckBoolean()
        {
            try
            {
                return luaState.LuaCheckBoolean(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public string CheckString()
        {
            try
            {
                return luaState.CheckString(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Vector3 CheckVector3()
        {
            try
            {
                return luaState.CheckVector3(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Quaternion CheckQuaternion()
        {
            try
            {
                return luaState.CheckQuaternion(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Vector2 CheckVector2()
        {
            try
            {
                return luaState.CheckVector2(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Vector4 CheckVector4()
        {
            try
            {
                return luaState.CheckVector4(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Color CheckColor()
        {
            try
            {
                return luaState.CheckColor(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Ray CheckRay()
        {
            try
            {
                return luaState.CheckRay(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Bounds CheckBounds()
        {
            try
            {
                return luaState.CheckBounds(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public LayerMask CheckLayerMask()
        {
            try
            {
                return luaState.CheckLayerMask(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public long CheckLong()
        {
            try
            {
                return luaState.CheckLong(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public ulong CheckULong()
        {
            try
            {
                return luaState.CheckULong(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public Delegate CheckDelegate()
        {
            try
            {
                return luaState.CheckDelegate(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public object CheckVariant()
        {
            return luaState.ToVariant(stackPos++);
        }

        public char[] CheckCharBuffer()
        {
            try
            {
                return luaState.CheckCharBuffer(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public byte[] CheckByteBuffer()
        {
            try
            {
                return luaState.CheckByteBuffer(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public object CheckObject(Type t)
        {
            try
            {
                return luaState.CheckObject(stackPos++, t);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public LuaFunction CheckLuaFunction()
        {
            try
            {
                return luaState.CheckLuaFunction(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public LuaTable CheckLuaTable()
        {
            try
            {
                return luaState.CheckLuaTable(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public LuaThread CheckLuaThread()
        {
            try
            {
                return luaState.CheckLuaThread(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }

        public T CheckValue<T>()
        {
            try
            {
                return luaState.CheckValue<T>(stackPos++);
            }
            catch (Exception e)
            {
                EndPCall();
                throw e;
            }
        }
    }    
}
