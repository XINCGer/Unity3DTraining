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

namespace LuaInterface
{
    public class LuaThread : LuaBaseRef
    {        
        public LuaThread(int reference, LuaState state)
        {
            this.luaState = state;
            this.reference = reference;
        }

        protected int Resume(IntPtr L, int nArgs)
        {
            int ret = LuaDLL.lua_resume(L, nArgs);

            if (ret > (int)LuaThreadStatus.LUA_YIELD)
            {
                string error = null;
                int top = LuaDLL.lua_gettop(L);
                LuaDLL.tolua_pushtraceback(L);
                LuaDLL.lua_pushthread(L);
                LuaDLL.lua_pushvalue(L, top);

                if (LuaDLL.lua_pcall(L, 2, -1, 0) != 0)
                {
                    LuaDLL.lua_settop(L, top);
                }

                error = LuaDLL.lua_tostring(L, -1);                
                luaState.LuaSetTop(0);
                throw new LuaException(error);
            }
            
            return ret;
        }

        public int Resume()
        {                                    
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            int ret = Resume(L, 0);
            if (ret == 0)
            {
                Dispose();
            }
            return ret;
        }

        public int Resume<T1>(T1 arg1)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);            
            StackTraits<T1>.Push(L, arg1);
            int ret = Resume(L, 1);
            if (ret == 0)
            {
                Dispose();
            }
            return ret;
        }

        public int Resume<T1, T2>(T1 arg1, T2 arg2)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            StackTraits<T1>.Push(L, arg1);
            StackTraits<T2>.Push(L, arg2);                        
            int ret = Resume(L, 2);
            if (ret == 0)
            {
                Dispose();
            }
            return ret;
        }

        public int Resume<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            StackTraits<T1>.Push(L, arg1);
            StackTraits<T2>.Push(L, arg2);
            StackTraits<T3>.Push(L, arg3);
            int ret = Resume(L, 3);
            if (ret == 0)
            {
                Dispose();
            }
            return ret;
        }

        public int Resume<R1>(out R1 ret1)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            int ret = Resume(L, 0);

            if (ret == 0)
            {
                ret1 = default(R1);
                Dispose();
            }
            else
            {
                int top = LuaDLL.lua_gettop(L);
                ret1 = StackTraits<R1>.Check(L, top);
            }

            return ret;
        }

        public int Resume<T1, R1>(T1 arg1, out R1 ret1)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            StackTraits<T1>.Push(L, arg1);            
            int ret = Resume(L, 1);

            if (ret == 0)
            {
                ret1 = default(R1);
                Dispose();
            }
            else
            {
                int top = LuaDLL.lua_gettop(L);
                ret1 = StackTraits<R1>.Check(L, top);
            }

            return ret;
        }

        public int Resume<T1, T2, R1>(T1 arg1, T2 arg2, out R1 ret1)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            StackTraits<T1>.Push(L, arg1);
            StackTraits<T2>.Push(L, arg2);
            int ret = Resume(L, 2);

            if (ret == 0)
            {
                ret1 = default(R1);
                Dispose();
            }
            else
            {
                int top = LuaDLL.lua_gettop(L);
                ret1 = StackTraits<R1>.Check(L, top);
            }

            return ret;
        }

        public int Resume<T1, T2, T3, R1>(T1 arg1, T2 arg2, T3 arg3, out R1 ret1)
        {
            luaState.Push(this);
            IntPtr L = luaState.LuaToThread(-1);
            luaState.LuaPop(1);
            StackTraits<T1>.Push(L, arg1);
            StackTraits<T2>.Push(L, arg2);
            StackTraits<T3>.Push(L, arg3);
            int ret = Resume(L, 3);

            if (ret == 0)
            {
                ret1 = default(R1);
                Dispose();
            }
            else
            {
                int top = LuaDLL.lua_gettop(L);
                ret1 = StackTraits<R1>.Check(L, top);
            }

            return ret;
        }
    }
}