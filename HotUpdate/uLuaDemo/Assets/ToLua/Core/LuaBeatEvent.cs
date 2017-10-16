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
using LuaInterface;

namespace LuaInterface
{    
    public class LuaBeatEvent : IDisposable
    {
        protected LuaState luaState;
        protected bool beDisposed;

        LuaTable self = null;
        LuaFunction _add = null;
        LuaFunction _remove = null;
        //LuaFunction _call = null;

        public LuaBeatEvent(LuaTable table)            
        {
            self = table;
            luaState = table.GetLuaState();
            self.AddRef();
            
            _add = self.GetLuaFunction("Add");
            _remove = self.GetLuaFunction("Remove");
            //_call = self.GetLuaFunction("__call");            
        }

        public void Dispose()
        {
            self.Dispose();            
            _add.Dispose();
            _remove.Dispose();
            //_call.Dispose();
            Clear();
        }

        void Clear()
        {
            //_call = null;
            _add = null;
            _remove = null;
            self = null;            
            luaState = null;
        }

        public void Dispose(bool disposeManagedResources)
        {
            if (!beDisposed)
            {
                beDisposed = true;

                //if (_call != null)
                //{
                //    _call.Dispose(disposeManagedResources);
                //    _call = null;
                //}

                if (_add != null)
                {
                    _add.Dispose(disposeManagedResources);
                    _add = null;
                }

                if (_remove != null)
                {
                    _remove.Dispose(disposeManagedResources);
                    _remove = null;
                }

                if (self != null)
                {
                    self.Dispose(disposeManagedResources);
                }

                Clear();
            }
        }

        public void Add(LuaFunction func, LuaTable obj)
        {
            if (func == null)
            {
                return;
            }

            _add.BeginPCall();
            _add.Push(self);
            _add.Push(func);
            _add.Push(obj);
            _add.PCall();
            _add.EndPCall();
        }

        public void Remove(LuaFunction func, LuaTable obj)
        {
            if (func == null)
            {
                return;
            }

            _remove.BeginPCall();
            _remove.Push(self);
            _remove.Push(func);
            _remove.Push(obj);
            _remove.PCall();
            _remove.EndPCall();
        }

        //public override int GetReference()
        //{
        //    return self.GetReference();
        //}
    }
}
