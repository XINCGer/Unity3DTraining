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
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LuaInterface
{
    public abstract class LuaBaseRef : IDisposable
    {
        public string name = null;
        protected int reference = -1;
        protected LuaState luaState;
        protected ObjectTranslator translator = null;

        protected volatile bool beDisposed;
        protected int count = 0;

        public LuaBaseRef()
        {
            IsAlive = true;
            count = 1;
        }

        ~LuaBaseRef()
        {
            IsAlive = false;
            Dispose(false);
        }

        public virtual void Dispose()
        {
            --count;

            if (count > 0)
            {
                return;
            }

            IsAlive = false;
            Dispose(true);            
        }

        public void AddRef()
        {
            ++count;            
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!beDisposed)
            {
                beDisposed = true;   

                if (reference > 0 && luaState != null)
                {
                    luaState.CollectRef(reference, name, !disposeManagedResources);
                }
                
                reference = -1;
                luaState = null;
                count = 0;
            }            
        }

        //慎用
        public void Dispose(int generation)
        {                         
            if (count > generation)
            {
                return;
            }

            Dispose(true);
        }

        public LuaState GetLuaState()
        {
            return luaState;
        }

        public void Push()
        {
            luaState.Push(this);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);            
        }

        public virtual int GetReference()
        {
            return reference;
        }

        public override bool Equals(object o)
        {
            if (o == null) return reference <= 0;
            LuaBaseRef lr = o as LuaBaseRef;      
            
            if (lr == null || lr.reference != reference)
            {
                return false;
            }

            return reference > 0;
        }

        static bool CompareRef(LuaBaseRef a, LuaBaseRef b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            object l = a;
            object r = b;

            if (l == null && r != null)
            {
                return b.reference <= 0;
            }

            if (l != null && r == null)
            {
                return a.reference <= 0;
            }

            if (a.reference != b.reference)
            {
                return false;
            }

            return a.reference > 0;
        }

        public static bool operator == (LuaBaseRef a, LuaBaseRef b)
        {
            return CompareRef(a, b);
        }

        public static bool operator != (LuaBaseRef a, LuaBaseRef b)
        {
            return !CompareRef(a, b);
        }

        public volatile bool IsAlive = true;
    }
}