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
using System.Globalization;
using System.Reflection;

namespace LuaInterface
{
    public sealed class LuaConstructor
    {
        ConstructorInfo method = null;
        List<Type> list = null;

        [NoToLuaAttribute]
        public LuaConstructor(ConstructorInfo func, Type[] types)
        {
            method = func;            

            if (types != null)
            {
                list = new List<Type>(types);
            }
        }

        public int Call(IntPtr L)
        {
            object[] args = null;
            ToLua.CheckArgsCount(L, list.Count + 1);

            if (list.Count > 0)
            {
                args = new object[list.Count];

                for (int i = 0; i < list.Count; i++)
                {
                    bool isRef = list[i].IsByRef;
                    Type t0 = isRef ? list[i].GetElementType() : list[i];
                    object o = ToLua.CheckVarObject(L, i + 2, t0);
                    args[i] = o;
                }
            }

            object ret = method.Invoke(args);
            int count = 1;
            ToLua.Push(L, ret);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsByRef)
                {
                    ++count;
                    ToLua.Push(L, args[i]);
                }
            }

            return count;
        }

        public void Destroy()
        {
            method = null;
            list.Clear();
        }
    }
}
