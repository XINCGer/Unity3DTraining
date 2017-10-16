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
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

namespace LuaInterface
{
    public enum LuaTypes
    {
        LUA_TNONE = -1,
        LUA_TNIL = 0,
        LUA_TBOOLEAN = 1,
        LUA_TLIGHTUSERDATA = 2,
        LUA_TNUMBER = 3,
        LUA_TSTRING = 4,
        LUA_TTABLE = 5,
        LUA_TFUNCTION = 6,
        LUA_TUSERDATA = 7,
        LUA_TTHREAD = 8,

    }

    public enum LuaGCOptions
    {
        LUA_GCSTOP = 0,
        LUA_GCRESTART = 1,
        LUA_GCCOLLECT = 2,
        LUA_GCCOUNT = 3,
        LUA_GCCOUNTB = 4,
        LUA_GCSTEP = 5,
        LUA_GCSETPAUSE = 6,
        LUA_GCSETSTEPMUL = 7,
    }

    public enum LuaThreadStatus
    {
        LUA_YIELD = 1,
        LUA_ERRRUN = 2,
        LUA_ERRSYNTAX = 3,
        LUA_ERRMEM = 4,
        LUA_ERRERR = 5,
    }

    public class LuaIndexes
    {
        public static int LUA_REGISTRYINDEX = -10000;
        public static int LUA_ENVIRONINDEX  = -10001;
        public static int LUA_GLOBALSINDEX  = -10002;
    }

    public class LuaRIDX
    {
        public int LUA_RIDX_MAINTHREAD = 1;
        public int LUA_RIDX_GLOBALS = 2;
        public int LUA_RIDX_PRELOAD = 25;
        public int LUA_RIDX_LOADED = 26;
    }

    public static class ToLuaFlags
    {
        public const int INDEX_ERROR = 1;       //Index 失败提示error信息，false返回nil
        public const int USE_INT64 = 2;         //是否luavm内部支持原生int64(目前用的vm都不支持, 默认false)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Lua_Debug
    {        
        public int eventcode;        
        public IntPtr _name;	            /* (n) */                
        public IntPtr _namewhat;	        /* (n) `global', `local', `field', `method' */        
        public IntPtr _what;	            /* (S) `Lua', `C', `main', `tail' */        
        public IntPtr _source;	            /* (S) */        
        public int currentline;	            /* (l) */        
        public int nups;		            /* (u) number of upvalues */        
        public int linedefined;     	    /* (S) */        
        public int lastlinedefined; 	    /* (S) */        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] _short_src;                
        public int i_ci;                    /* active function */

        string tostring(IntPtr p)
        {
            if (p != IntPtr.Zero)
            {
                int len = LuaDLL.tolua_strlen(p);
                return LuaDLL.lua_ptrtostring(p, len);
            }

            return string.Empty;
        }

        public string namewhat
        {
            get
            {
                return tostring(_namewhat);
            }
        }

        public string name
        {
            get
            {
                return tostring(_name);
            }
        }

        public string what
        {
            get
            {
                return tostring(_what);
            }
        }

        public string source
        {
            get
            {
                return tostring(_source);
            }
        }        

        int GetShortSrcLen(byte[] str)
        {
            int i = 0;

            for (; i < 128; i++)
            {
                if (str[i] == '\0')
                {
                    return i;
                }
            }

            return i;
        }

        public string short_src
        {
            get
            {
                if (_short_src == null)
                {
                    return string.Empty;
                }

                int count = GetShortSrcLen(_short_src);
                return Encoding.UTF8.GetString(_short_src, 0, count);
            }
        }        
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int LuaCSFunction(IntPtr luaState);        
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LuaHook(IntPtr L, ref Lua_Debug ar);
#else
    public delegate int LuaCSFunction(IntPtr luaState);    
    public delegate void LuaHook(IntPtr L, ref Lua_Debug ar);    
#endif

    public class LuaDLL
    {
        public static string version = "1.0.7.377";
        public static int LUA_MULTRET = -1;
        public static string[] LuaTypeName = { "none", "nil", "boolean", "lightuserdata", "number", "string", "table", "function", "userdata", "thread" };        

#if !UNITY_EDITOR && UNITY_IPHONE
        const string LUADLL = "__Internal";
#else
        const string LUADLL = "tolua";
#endif
        /*
        ** third party library
        */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_pb(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_ffi(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_bit(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_struct(IntPtr L);     
   
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lpeg(IntPtr L);             

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_socket_core(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_mime_core(IntPtr L);
                   
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_cjson(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_cjson_safe(IntPtr L);

        /*
         ** pseudo-indices
         */
        public static int lua_upvalueindex(int i)
        {
            return LuaIndexes.LUA_GLOBALSINDEX - i;
        }

        /*
         * state manipulation
         */
        //[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern IntPtr lua_newstate(LuaAlloc f, IntPtr ud);                      //luajit64位不能用这个函数

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_close(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]                        //[-0, +1, m]  
        public static extern IntPtr lua_newthread(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_atpanic(IntPtr luaState, IntPtr panic);

        /*
         * basic stack manipulation
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr luaState, int top);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_remove(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_insert(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_replace(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_checkstack(IntPtr luaState, int extra);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_xmove(IntPtr from, IntPtr to, int n);

        /*
         * access functions (stack -> C)
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isnumber(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isstring(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_iscfunction(IntPtr luaState, int index);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_isuserdata(IntPtr luaState, int stackPos);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaTypes lua_type(IntPtr luaState, int index);

        public static string lua_typename(IntPtr luaState, LuaTypes type)
        {
            int t = (int)type;
            return LuaTypeName[t + 1];
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_equal(IntPtr luaState, int idx1, int idx2);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_rawequal(IntPtr luaState, int idx1, int idx2);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_lessthan(IntPtr luaState, int idx1, int idx2);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern double lua_tonumber(IntPtr luaState, int idx);

        public static int lua_tointeger(IntPtr luaState, int idx)
        {
            return tolua_tointeger(luaState, idx);
        }

        public static bool lua_toboolean(IntPtr luaState, int idx)
        {
            return tolua_toboolean(luaState, idx);            
        }

        public static IntPtr lua_tolstring(IntPtr luaState, int index, out int strLen)               //[-0, +0, m]
        {            
            return tolua_tolstring(luaState, index, out strLen);
        }

        public static int lua_objlen(IntPtr luaState, int idx)
        {
            return tolua_objlen(luaState, idx);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tocfunction(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_tothread(IntPtr L, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lua_topointer(IntPtr L, int idx);

        /* 
         * push functions (C -> stack)
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnil(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(IntPtr luaState, double number);
        
        public static void lua_pushinteger(IntPtr L, int n)
        {
            lua_pushnumber(L, n);
        }                

        public static void lua_pushlstring(IntPtr luaState, byte[] str, int size)                   //[-0, +1, m]
        {
            if (size >= 0x7fffff00)
            {
                throw new LuaException("string length overflow");
            }

            tolua_pushlstring(luaState, str, size);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushstring(IntPtr luaState, string str);                      //[-0, +1, m]

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(IntPtr luaState, IntPtr fn, int n);              //[-n, +1, m]
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(IntPtr luaState, int value);

        public static void lua_pushboolean(IntPtr luaState, bool value)
        {
            lua_pushboolean(luaState, value ? 1 : 0);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(IntPtr luaState, IntPtr udata);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_pushthread(IntPtr L);

        /*
         * get functions (Lua -> stack)
         */
        public static void lua_gettable(IntPtr L, int idx)
        {
            if (LuaDLL.tolua_gettable(L, idx) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                throw new LuaException(error);
            }
        }

        public static void lua_getfield(IntPtr L, int idx, string key)
        {
            if (LuaDLL.tolua_getfield(L, idx, key) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                throw new LuaException(error);
            }
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawget(IntPtr luaState, int idx);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawgeti(IntPtr luaState, int idx, int n);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_createtable(IntPtr luaState, int narr, int nrec);             //[-0, +1, m]        

        public static IntPtr lua_newuserdata(IntPtr luaState, int size)                             //[-0, +1, m]
        {
            return tolua_newuserdata(luaState, size);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getmetatable(IntPtr luaState, int objIndex);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_getfenv(IntPtr luaState, int idx);

        /*
         * set functions (stack -> Lua)
         */
        public static void lua_settable(IntPtr L, int idx)
        {
            if (tolua_settable(L, idx) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                throw new LuaException(error);
            }
        }

        public static void lua_setfield(IntPtr L, int idx, string key)
        {
            if (tolua_setfield(L, idx, key) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                throw new LuaException(error);
            }
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawset(IntPtr luaState, int idx);                             //[-2, +0, m]       
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr luaState, int tableIndex, int index);          //[-1, +0, m]
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_setmetatable(IntPtr luaState, int objIndex);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_setfenv(IntPtr luaState, int stackPos);

        /*
         * `load' and `call' functions (load and run Lua code)
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_call(IntPtr luaState, int nArgs, int nResults);               //[-(nargs+1), +nresults, e]       
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_cpcall(IntPtr L, IntPtr func, IntPtr ud);

        //[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int lua_load(IntPtr luaState, LuaChunkReader chunkReader, ref ReaderInfo data, string chunkName);
        //[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int lua_dump(IntPtr L, LuaWriter writer, IntPtr data);

        /* 
         * coroutine functions
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_yield(IntPtr L, int nresults);                                 //[-?, +?, e]       
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_resume(IntPtr L, int narg);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_status(IntPtr L);

        /*
         * garbage-collection function and options
         */
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gc(IntPtr luaState, LuaGCOptions what, int data);              //[-0, +0, e]

        /*
         * miscellaneous functions
         */                
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_next(IntPtr luaState, int index);                              //[-1, +(2|0), e]        
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lua_concat(IntPtr luaState, int n);                               //[-n, +1, e]

        /* 
        ** ===============================================================
        ** some useful functions
        ** ===============================================================
        */
        public static void lua_pop(IntPtr luaState, int amount)
        {
            LuaDLL.lua_settop(luaState, -(amount) - 1);
        }

        public static void lua_newtable(IntPtr luaState)
        {
            LuaDLL.lua_createtable(luaState, 0, 0);
        }

        public static void lua_register(IntPtr luaState, string name, LuaCSFunction func)
        {
            lua_pushcfunction(luaState, func);
            lua_setglobal(luaState, name);
        }

        public static void lua_pushcfunction(IntPtr luaState, LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            lua_pushcclosure(luaState, fn, 0);
        }

        public static bool lua_isfunction(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) == LuaTypes.LUA_TFUNCTION;
        }

        public static bool lua_istable(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) == LuaTypes.LUA_TTABLE;
        }

        public static bool lua_islightuserdata(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) == LuaTypes.LUA_TLIGHTUSERDATA;
        }

        public static bool lua_isnil(IntPtr luaState, int n)
        {
            return (lua_type(luaState, n) == LuaTypes.LUA_TNIL);
        }

        public static bool lua_isboolean(IntPtr luaState, int n)
        {
            LuaTypes type = lua_type(luaState, n);
            return type == LuaTypes.LUA_TBOOLEAN || type == LuaTypes.LUA_TNIL;
        }

        public static bool lua_isthread(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) == LuaTypes.LUA_TTHREAD;
        }

        public static bool lua_isnone(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) == LuaTypes.LUA_TNONE;
        }

        public static bool lua_isnoneornil(IntPtr luaState, int n)
        {
            return lua_type(luaState, n) <= LuaTypes.LUA_TNIL;
        }

        public static void lua_setglobal(IntPtr luaState, string name)
        {
            lua_setfield(luaState, LuaIndexes.LUA_GLOBALSINDEX, name);
        }

        public static void lua_getglobal(IntPtr luaState, string name)
        {
            lua_getfield(luaState, LuaIndexes.LUA_GLOBALSINDEX, name);
        }

        public static string lua_ptrtostring(IntPtr str, int len)
        {
            string ss = Marshal.PtrToStringAnsi(str, len);

            if (ss == null)
            {
                byte[] buffer = new byte[len];
                Marshal.Copy(str, buffer, 0, len);
                return Encoding.UTF8.GetString(buffer);
            }

            return ss;
        }

        public static string lua_tostring(IntPtr luaState, int index)
        {
            int len = 0;
            IntPtr str = tolua_tolstring(luaState, index, out len);

            if (str != IntPtr.Zero)
            {
                return lua_ptrtostring(str, len);
            }

            return null;
        }

        public static IntPtr lua_open()
        {
            return luaL_newstate();
        }

        public static void lua_getregistry(IntPtr L)
        {
            lua_pushvalue(L, LuaIndexes.LUA_REGISTRYINDEX);
        }

        public static int lua_getgccount(IntPtr L)
        {
            return lua_gc(L, LuaGCOptions.LUA_GCCOUNT, 0);
        }

        /*
         ** ======================================================================
         ** Debug API
         ** =======================================================================
         */

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getstack(IntPtr L, int level, ref Lua_Debug ar);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_getinfo(IntPtr L, string what, ref Lua_Debug ar);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_getlocal(IntPtr L, ref Lua_Debug ar, int n);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_setlocal(IntPtr L, ref Lua_Debug ar, int n);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_getupvalue(IntPtr L, int funcindex, int n);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_setupvalue(IntPtr L, int funcindex, int n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_sethook(IntPtr L, LuaHook func, int mask, int count);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern LuaHook lua_gethook(IntPtr L);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gethookmask(IntPtr L);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int lua_gethookcount(IntPtr L);

        //lualib.h
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_openlibs(IntPtr luaState);

        //lauxlib.h
        public static int abs_index(IntPtr L, int i)
        {
            return (i > 0 || i <= LuaIndexes.LUA_REGISTRYINDEX) ? i : lua_gettop(L) + i + 1;
        }

        public static int luaL_getn(IntPtr luaState, int i)
        {
            return (int)tolua_getn(luaState, i);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_getmetafield(IntPtr luaState, int stackPos, string field);           //[-0, +(0|1), m]

        public static int luaL_callmeta(IntPtr L, int stackPos, string field)                              //[-0, +(0|1), m]
        {
            stackPos = abs_index(L, stackPos);

            if (luaL_getmetafield(L, stackPos, field) == 0)  /* no metafield? */
            {
                return 0;
            }

            lua_pushvalue(L, stackPos);

            if (lua_pcall(L, 1, 1, 0) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                lua_pop(L, 1);
                throw new LuaException(error);
            }

            return 1;
        }

        public static int luaL_argerror(IntPtr L, int narg, string extramsg)
        {
            if (tolua_argerror(L, narg, extramsg) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);
                lua_pop(L, 1);
                throw new LuaException(error);
            }

            return 0;
        }

        public static int luaL_typerror(IntPtr L, int stackPos, string tname, string t2 = null)
        {
            if (t2 == null)
            {
                t2 = luaL_typename(L, stackPos);
            }

            string msg = string.Format("{0} expected, got {1}", tname, t2);
            return luaL_argerror(L, stackPos, msg);
        }

        public static string luaL_checklstring(IntPtr L, int numArg, out int len)
        {
            IntPtr str = tolua_tolstring(L, numArg, out len);

            if (str == IntPtr.Zero)
            {
                luaL_typerror(L, numArg, "string");
                return null;
            }

            return lua_ptrtostring(str, len);
        }

        public static string luaL_optlstring(IntPtr L, int narg, string def, out int len)
        {
            if (lua_isnoneornil(L, narg))
            {
                len = def != null ? def.Length : 0;
                return def;
            }

            return luaL_checklstring(L, narg, out len);
        }

        public static double luaL_checknumber(IntPtr L, int stackPos)
        {
            double d = lua_tonumber(L, stackPos);

            if (d == 0 && LuaDLL.lua_isnumber(L, stackPos) == 0)
            {
                luaL_typerror(L, stackPos, "number");
                return 0;
            }

            return d;
        }

        public static double luaL_optnumber(IntPtr L, int idx, double def)
        {
            if (lua_isnoneornil(L, idx))
            {
                return def;
            }

            return luaL_checknumber(L, idx);
        }

        public static int luaL_checkinteger(IntPtr L, int stackPos)
        {
            int d = tolua_tointeger(L, stackPos);

            if (d == 0 && lua_isnumber(L, stackPos) == 0)
            {
                luaL_typerror(L, stackPos, "number");
                return 0;
            }

            return d;
        }

        public static int luaL_optinteger(IntPtr L, int idx, int def)
        {
            if (lua_isnoneornil(L, idx))
            {
                return def;
            }

            return luaL_checkinteger(L, idx);
        }

        public static bool luaL_checkboolean(IntPtr luaState, int index)
        {
            if (lua_isboolean(luaState, index))
            {
                return lua_toboolean(luaState, index);
            }

            luaL_typerror(luaState, index, "boolean");
            return false;
        }

        public static void luaL_checkstack(IntPtr L, int space, string mes)
        {
            if (lua_checkstack(L, space) == 0)
            {
                throw new LuaException(string.Format("stack overflow (%s)", mes));
            }
        }

        public static void luaL_checktype(IntPtr L, int narg, LuaTypes t)
        {
            if (lua_type(L, narg) != t)
            {
                luaL_typerror(L, narg, lua_typename(L, t));
            }
        }

        public static void luaL_checkany(IntPtr L, int narg)
        {
            if (lua_type(L, narg) == LuaTypes.LUA_TNONE)
            {
                luaL_argerror(L, narg, "value expected");
            }
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_newmetatable(IntPtr luaState, string meta);                               //[-0, +1, m]

        public static IntPtr luaL_checkudata(IntPtr L, int ud, string tname)
        {
            IntPtr p = lua_touserdata(L, ud);

            if (p != IntPtr.Zero)
            {
                if (lua_getmetatable(L, ud) != 0)
                {
                    lua_getfield(L, LuaIndexes.LUA_REGISTRYINDEX, tname);  /* get correct metatable */

                    if (lua_rawequal(L, -1, -2) != 0)
                    {  /* does it have the correct mt? */
                        lua_pop(L, 2);  /* remove both metatables */
                        return p;
                    }
                }
            }

            luaL_typerror(L, ud, tname);    /* else error */
            return IntPtr.Zero;             /* to avoid warnings */
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_where(IntPtr luaState, int level);                                           //[-0, +1, e]

        //[DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        //public static extern int luaL_error(IntPtr luaState, string message);

        public static int luaL_throw(IntPtr L, string message)
        {
            tolua_pushtraceback(L);
            lua_pushstring(L, message);
            lua_pushnumber(L, 1);

            if (lua_pcall(L, 2, -1, 0) == 0)
            {
                message = lua_tostring(L, -1);
            }
            else
            {
                lua_pop(L, 1);
            }

            throw new LuaException(message, null, 2);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_ref(IntPtr luaState, int t);                                                  //[-1, +0, m]
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void luaL_unref(IntPtr luaState, int registryIndex, int reference);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadfile(IntPtr luaState, string filename);                                   //[-0, +1, e]

        public static int luaL_loadbuffer(IntPtr luaState, byte[] buff, int size, string name)
        {
            return tolua_loadbuffer(luaState, buff, size, name);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaL_loadstring(IntPtr luaState, string chunk);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_gsub(IntPtr luaState, string str, string pattern, string replacement);     //[-0, +1, e]  

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr luaL_findtable(IntPtr luaState, int idx, string fname, int szhint = 1);

        /*
         ** ===============================================================
         ** some useful functions
         ** ===============================================================
        */
        public static string luaL_typename(IntPtr luaState, int stackPos)
        {
            LuaTypes type = LuaDLL.lua_type(luaState, stackPos);
            return lua_typename(luaState, type);
        }

        public static bool luaL_dofile(IntPtr luaState, string fileName)                                              //[-0, +1, e]
        {
            int result = luaL_loadfile(luaState, fileName);

            if (result != 0)
            {
                return false;
            }

            return LuaDLL.lua_pcall(luaState, 0, LUA_MULTRET, 0) == 0;
        }

        public static bool luaL_dostring(IntPtr luaState, string chunk)
        {
            int result = LuaDLL.luaL_loadstring(luaState, chunk);

            if (result != 0)
            {
                return false;
            }

            return LuaDLL.lua_pcall(luaState, 0, LUA_MULTRET, 0) == 0;
        }

        public static void luaL_getmetatable(IntPtr luaState, string meta)
        {
            LuaDLL.lua_getfield(luaState, LuaIndexes.LUA_REGISTRYINDEX, meta);
        }


        /* compatibility with ref system */
        public static int lua_ref(IntPtr luaState)
        {
            return LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX);
        }

        public static void lua_getref(IntPtr luaState, int reference)
        {
            lua_rawgeti(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        public static void lua_unref(IntPtr luaState, int reference)
        {
            luaL_unref(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        /*
        ** ======================================================
        ** tolua libs
        ** =======================================================
        */

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_openlibs(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_openint64(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_openlualibs(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_tag();

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_newudata(IntPtr luaState, int val);                         //[-0, +0, m]

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_rawnetobj(IntPtr luaState, int obj);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_pushudata(IntPtr L, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_pushnewudata(IntPtr L, int metaRef, int index);             //[-0, +0, m]

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_beginpcall(IntPtr L, int reference);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushtraceback(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_getvec2(IntPtr luaState, int stackPos, out float x, out float y);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_getvec3(IntPtr luaState, int stackPos, out float x, out float y, out float z);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_getvec4(IntPtr luaState, int stackPos, out float x, out float y, out float z, out float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_getclr(IntPtr luaState, int stackPos, out float r, out float g, out float b, out float a);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_getquat(IntPtr luaState, int stackPos, out float x, out float y, out float z, out float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getlayermask(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushvec2(IntPtr luaState, float x, float y);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushvec3(IntPtr luaState, float x, float y, float z);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushvec4(IntPtr luaState, float x, float y, float z, float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushquat(IntPtr luaState, float x, float y, float z, float w);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushclr(IntPtr luaState, float r, float g, float b, float a);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushlayermask(IntPtr luaState, int mask);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_isint64(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern long tolua_toint64(IntPtr luaState, int stackPos);

        public static long tolua_checkint64(IntPtr L, int stackPos)
        {
            long d = tolua_toint64(L, stackPos);

            if (d == 0 && !tolua_isint64(L, stackPos))
            {
                luaL_typerror(L, stackPos, "long");
                return 0;
            }

            return d;
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushint64(IntPtr luaState, long n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_isuint64(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong tolua_touint64(IntPtr luaState, int stackPos);

        public static ulong tolua_checkuint64(IntPtr L, int stackPos)
        {
            ulong d = tolua_touint64(L, stackPos);

            if (d == 0 && !tolua_isuint64(L, stackPos))
            {
                luaL_typerror(L, stackPos, "ulong");
                return 0;
            }

            return d;
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushuint64(IntPtr luaState, ulong n);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_setindex(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_setnewindex(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int toluaL_ref(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void toluaL_unref(IntPtr L, int reference);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_getmainstate(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getvaluetype(IntPtr L, int stackPos);                

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_createtable(IntPtr L, string fullPath, int szhint = 0);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_pushluatable(IntPtr L, string fullPath);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_beginmodule(IntPtr L, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_endmodule(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_beginpremodule(IntPtr L, string fullPath, int szhint = 0);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_endpremodule(IntPtr L, int reference);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_addpreload(IntPtr L, string path);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_beginclass(IntPtr L, string name, int baseMetaRef, int reference = -1);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_endclass(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_function(IntPtr L, string name, IntPtr fn);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_tocbuffer(string name, int sz);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_freebuffer(IntPtr buffer);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_variable(IntPtr L, string name, IntPtr get, IntPtr set);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_constant(IntPtr L, string name, double val);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_beginenum(IntPtr L, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_endenum(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_beginstaticclass(IntPtr L, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_endstaticclass(IntPtr L);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_require(IntPtr L, string fileName);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getmetatableref(IntPtr L, int pos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_setflag(int bit, bool flag);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_isvptrtable(IntPtr L, int index);

        public static int toluaL_exception(IntPtr L, Exception e)
        {            
            LuaException.luaStack = new LuaException(e.Message, e, 2);            
            return tolua_error(L, e.Message);
        }

        public static int toluaL_exception(IntPtr L, Exception e, object o, string msg)
        {
            if (o != null && !o.Equals(null))
            {
                msg = e.Message;
            }
            
            LuaException.luaStack = new LuaException(msg, e, 2);
            return tolua_error(L, msg);
        }

        //适配函数
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_loadbuffer(IntPtr luaState, byte[] buff, int size, string name);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool tolua_toboolean(IntPtr luaState, int index);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_tointeger(IntPtr luaState, int idx);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_tolstring(IntPtr luaState, int index, out int strLen);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushlstring(IntPtr luaState, byte[] str, int size);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_objlen(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_newuserdata(IntPtr luaState, int size);                     //[-0, +1, m]

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_argerror(IntPtr luaState, int narg, string extramsg);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_error(IntPtr L, string msg);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getfield(IntPtr L, int idx, string key);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_setfield(IntPtr L, int idx, string key);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_gettable(IntPtr luaState, int idx);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_settable(IntPtr luaState, int idx);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getn(IntPtr luaState, int stackPos);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_strlen(IntPtr str);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushcfunction(IntPtr L, IntPtr fn);

        public static void tolua_pushcfunction(IntPtr luaState, LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            tolua_pushcfunction(luaState, fn);
        }

        public static string tolua_findtable(IntPtr L, int idx, string name, int size = 1)
        {
            int oldTop = lua_gettop(L);
            IntPtr p = LuaDLL.luaL_findtable(L, idx, name, size);

            if (p != IntPtr.Zero)
            {
                LuaDLL.lua_settop(L, oldTop);
                int len = LuaDLL.tolua_strlen(p);
                return LuaDLL.lua_ptrtostring(p, len);
            }

            return null;
        }

        public static IntPtr tolua_atpanic(IntPtr L, LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            return lua_atpanic(L, fn);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr tolua_buffinit(IntPtr luaState);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_addlstring(IntPtr b, string str, int l);      
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_addstring(IntPtr b, string s);                
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_addchar(IntPtr b, byte s);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_pushresult(IntPtr b);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_update(IntPtr L, float deltaTime, float unscaledDelta);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_lateupdate(IntPtr L);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_fixedupdate(IntPtr L, float fixedTime);
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void tolua_regthis(IntPtr L, IntPtr get, IntPtr set);

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_where(IntPtr L, int level);

        public static void tolua_bindthis(IntPtr L, LuaCSFunction get, LuaCSFunction set)
        {
            IntPtr pGet = IntPtr.Zero;
            IntPtr pSet = IntPtr.Zero;

            if (get != null)
            {
                pGet = Marshal.GetFunctionPointerForDelegate(get);
            }

            if (set != null)
            {
                pSet = Marshal.GetFunctionPointerForDelegate(set);
            }

            tolua_regthis(L, pGet, pSet);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int tolua_getclassref(IntPtr L, int pos);
    }
}
