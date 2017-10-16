using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaInterface
{
    public class LuaStatePtr
    {
        protected IntPtr L;

        string jit = @"            
        function Euler(x, y, z)
            x = x * 0.0087266462599716
            y = y * 0.0087266462599716
            z = z * 0.0087266462599716

            local sinX = math.sin(x)
            local cosX = math.cos(x)
            local sinY = math.sin(y)
            local cosY = math.cos(y)
            local sinZ = math.sin(z)
            local cosZ = math.cos(z)

            local w = cosY * cosX * cosZ + sinY * sinX * sinZ
            x = cosY* sinX * cosZ + sinY* cosX * sinZ
            y = sinY * cosX * cosZ - cosY * sinX * sinZ
            z = cosY* cosX * sinZ - sinY* sinX * cosZ

            return {x = x, y = y, z= z, w = w}
        end

        function Slerp(q1, q2, t)
            local x1, y1, z1, w1 = q1.x, q1.y, q1.z, q1.w
            local x2,y2,z2,w2 = q2.x, q2.y, q2.z, q2.w
            local dot = x1* x2 + y1* y2 + z1* z2 + w1* w2

            if dot< 0 then
                dot = -dot
                x2, y2, z2, w2 = -x2, -y2, -z2, -w2
            end

            if dot< 0.95 then
                local sin = math.sin
                local angle = math.acos(dot)
                local invSinAngle = 1 / sin(angle)
                local t1 = sin((1 - t) * angle) * invSinAngle
                local t2 = sin(t * angle) * invSinAngle
                return {x = x1* t1 + x2* t2, y = y1 * t1 + y2 * t2, z = z1 * t1 + z2 * t2, w = w1 * t1 + w2 * t2}
            else
                x1 = x1 + t* (x2 - x1)
                y1 = y1 + t* (y2 - y1)                
                z1 = z1 + t* (z2 - z1)                
                w1 = w1 + t* (w2 - w1)
                dot = x1* x1 + y1* y1 + z1* z1 + w1* w1

                return {x = x1 / dot, y = y1 / dot, z = z1 / dot, w = w1 / dot}
            end
        end

        if jit then
    	    if jit.status() then                
                for i=1,10000 do
                    local q1 = Euler(i, i, i)
                    Slerp({ x = 0, y = 0, z = 0, w = 1}, q1, 0.5)
                end                
            end	                   
        end";

        public int LuaUpValueIndex(int i)
        {
            return LuaIndexes.LUA_GLOBALSINDEX - i;
        }

        public IntPtr LuaNewState()
        {
            return LuaDLL.luaL_newstate();            
        }

        public void LuaOpenJit()
        {
            if (!LuaDLL.luaL_dostring(L, jit))
            {
                string str = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_settop(L, 0);
                throw new Exception(str);
            }            
        }

        public void LuaClose()
        {
            LuaDLL.lua_close(L);
            L = IntPtr.Zero;
        }

        public IntPtr LuaNewThread()
        {
            return LuaDLL.lua_newthread(L);
        }        

        public IntPtr LuaAtPanic(IntPtr panic)
        {
            return LuaDLL.lua_atpanic(L, panic);
        }

        public int LuaGetTop()
        {
            return LuaDLL.lua_gettop(L);
        }

        public void LuaSetTop(int newTop)
        {
            LuaDLL.lua_settop(L, newTop);
        }

        public void LuaPushValue(int idx)
        {
            LuaDLL.lua_pushvalue(L, idx);
        }

        public void LuaRemove(int index)
        {
            LuaDLL.lua_remove(L, index);
        }

        public void LuaInsert(int idx)
        {
            LuaDLL.lua_insert(L, idx);
        }

        public void LuaReplace(int idx)
        {
            LuaDLL.lua_replace(L, idx);
        }

        public bool LuaCheckStack(int args)
        {
            return LuaDLL.lua_checkstack(L, args) != 0;
        }

        public void LuaXMove(IntPtr to, int n)
        {
            LuaDLL.lua_xmove(L, to, n);
        }

        public bool LuaIsNumber(int idx)
        {
            return LuaDLL.lua_isnumber(L, idx) != 0;
        }

        public bool LuaIsString(int index)
        {
            return LuaDLL.lua_isstring(L, index) != 0;
        }

        public bool LuaIsCFunction(int index)
        {
            return LuaDLL.lua_iscfunction(L, index) != 0;
        }

        public bool LuaIsUserData(int index)
        {
            return LuaDLL.lua_isuserdata(L, index) != 0;
        }

        public bool LuaIsNil(int n)
        {
            return LuaDLL.lua_isnil(L, n);
        }

        public LuaTypes LuaType(int index)
        {
            return LuaDLL.lua_type(L, index);
        }

        public string LuaTypeName(LuaTypes type)
        {
            return LuaDLL.lua_typename(L, type);
        }

        public string LuaTypeName(int idx)
        {
            return LuaDLL.luaL_typename(L, idx);
        }

        public bool LuaEqual(int idx1, int idx2)
        {
            return LuaDLL.lua_equal(L, idx1, idx2) != 0;
        }

        public bool LuaRawEqual(int idx1, int idx2)
        {
            return LuaDLL.lua_rawequal(L, idx1, idx2) != 0;
        }

        public bool LuaLessThan(int idx1, int idx2)
        {
            return LuaDLL.lua_lessthan(L, idx1, idx2) != 0;
        }

        public double LuaToNumber(int idx)
        {
            return LuaDLL.lua_tonumber(L, idx);
        }

        public int LuaToInteger(int idx)
        {
            return LuaDLL.lua_tointeger(L, idx);
        }

        public bool LuaToBoolean(int idx)
        {
            return LuaDLL.lua_toboolean(L, idx);
        }

        public string LuaToString(int index)
        {
            return LuaDLL.lua_tostring(L, index);
        }

        public IntPtr LuaToLString(int index, out int len)
        {
            return LuaDLL.tolua_tolstring(L, index, out len);
        }

        public IntPtr LuaToCFunction(int idx)
        {
            return LuaDLL.lua_tocfunction(L, idx);
        }

        public IntPtr LuaToUserData(int idx)
        {
            return LuaDLL.lua_touserdata(L, idx);
        }

        public IntPtr LuaToThread(int idx)
        {
            return LuaDLL.lua_tothread(L, idx);
        }

        public IntPtr LuaToPointer(int idx)
        {
            return LuaDLL.lua_topointer(L, idx);
        }

        public int LuaObjLen(int index)
        {
            return LuaDLL.tolua_objlen(L, index);
        }

        public void LuaPushNil()
        {
            LuaDLL.lua_pushnil(L);
        }

        public void LuaPushNumber(double number)
        {
            LuaDLL.lua_pushnumber(L, number);
        }

        public void LuaPushInteger(int n)
        {
            LuaDLL.lua_pushnumber(L, n);
        }

        public void LuaPushLString(byte[] str, int size)
        {
            LuaDLL.lua_pushlstring(L, str, size);
        }

        public void LuaPushString(string str)
        {
            LuaDLL.lua_pushstring(L, str);
        }

        public void LuaPushCClosure(IntPtr fn, int n)
        {
            LuaDLL.lua_pushcclosure(L, fn, n);
        }

        public void LuaPushBoolean(bool value)
        {
            LuaDLL.lua_pushboolean(L, value ? 1 : 0);
        }

        public void LuaPushLightUserData(IntPtr udata)
        {
            LuaDLL.lua_pushlightuserdata(L, udata);
        }

        public int LuaPushThread()
        {
            return LuaDLL.lua_pushthread(L);
        }

        public void LuaGetTable(int idx)
        {
            LuaDLL.lua_gettable(L, idx);
        }

        public void LuaGetField(int index, string key)
        {
            LuaDLL.lua_getfield(L, index, key);
        }

        public void LuaRawGet(int idx)
        {
            LuaDLL.lua_rawget(L, idx);
        }

        public void LuaRawGetI(int tableIndex, int index)
        {
            LuaDLL.lua_rawgeti(L, tableIndex, index);
        }

        public void LuaCreateTable(int narr = 0, int nec = 0)
        {
            LuaDLL.lua_createtable(L, narr, nec);
        }

        public IntPtr LuaNewUserData(int size)
        {
            return LuaDLL.tolua_newuserdata(L, size);
        }

        public int LuaGetMetaTable(int idx)
        {
            return LuaDLL.lua_getmetatable(L, idx);
        }

        public void LuaGetEnv(int idx)
        {
            LuaDLL.lua_getfenv(L, idx);
        }

        public void LuaSetTable(int idx)
        {
            LuaDLL.lua_settable(L, idx);
        }

        public void LuaSetField(int idx, string key)
        {
            LuaDLL.lua_setfield(L, idx, key);
        }

        public void LuaRawSet(int idx)
        {
            LuaDLL.lua_rawset(L, idx);
        }

        public void LuaRawSetI(int tableIndex, int index)
        {
            LuaDLL.lua_rawseti(L, tableIndex, index);
        }

        public void LuaSetMetaTable(int objIndex)
        {
            LuaDLL.lua_setmetatable(L, objIndex);
        }

        public void LuaSetEnv(int idx)
        {
            LuaDLL.lua_setfenv(L, idx);
        }

        public void LuaCall(int nArgs, int nResults)
        {
            LuaDLL.lua_call(L, nArgs, nResults);
        }

        public int LuaPCall(int nArgs, int nResults, int errfunc)
        {
            return LuaDLL.lua_pcall(L, nArgs, nResults, errfunc);
        }

        public int LuaYield(int nresults)
        {
            return LuaDLL.lua_yield(L, nresults);
        }

        public int LuaResume(int narg)
        {
            return LuaDLL.lua_resume(L, narg);
        }

        public int LuaStatus()
        {
            return LuaDLL.lua_status(L);
        }

        public void LuaGC(LuaGCOptions what, int data = 0)
        {
            LuaDLL.lua_gc(L, what, data);
        }

        public bool LuaNext(int index)
        {
            return LuaDLL.lua_next(L, index) != 0;
        }

        public void LuaConcat(int n)
        {
            LuaDLL.lua_concat(L, n);
        }

        public void LuaPop(int amount)
        {
            LuaDLL.lua_pop(L, amount);
        }

        public void LuaNewTable()
        {
            LuaDLL.lua_createtable(L, 0 , 0);
        }

        public void LuaPushFunction(LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            LuaDLL.lua_pushcclosure(L, fn, 0);
        }

        public bool lua_isfunction(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TFUNCTION;
        }

        public bool lua_istable(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TTABLE;
        }

        public bool lua_islightuserdata(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TLIGHTUSERDATA;
        }

        public bool lua_isnil(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TNIL;
        }

        public bool lua_isboolean(int n)
        {
            LuaTypes type = LuaDLL.lua_type(L, n);
            return type == LuaTypes.LUA_TBOOLEAN || type == LuaTypes.LUA_TNIL;
        }

        public bool lua_isthread(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TTHREAD;
        }

        public bool lua_isnone(int n)
        {
            return LuaDLL.lua_type(L, n) == LuaTypes.LUA_TNONE;
        }

        public bool lua_isnoneornil(int n)
        {
            return LuaDLL.lua_type(L, n) <= LuaTypes.LUA_TNIL;
        }

        public void LuaRawGlobal(string name)
        {
            LuaDLL.lua_pushstring(L, name);
            LuaDLL.lua_rawget(L, LuaIndexes.LUA_GLOBALSINDEX);
        }

        public void LuaSetGlobal(string name)
        {
            LuaDLL.lua_setglobal(L, name);
        }

        public void LuaGetGlobal(string name)
        {
            LuaDLL.lua_getglobal(L, name);
        }

        public void LuaOpenLibs()
        {
            LuaDLL.luaL_openlibs(L);
        }

        public int AbsIndex(int i)
        {
            return (i > 0 || i <= LuaIndexes.LUA_REGISTRYINDEX) ? i : LuaDLL.lua_gettop(L) + i + 1;
        }

        public int LuaGetN(int i)
        {
            return LuaDLL.luaL_getn(L, i);
        }

        public double LuaCheckNumber(int stackPos)
        {
            return LuaDLL.luaL_checknumber(L, stackPos);
        }

        public int LuaCheckInteger(int idx)
        {
            return LuaDLL.luaL_checkinteger(L, idx);
        }

        public bool LuaCheckBoolean(int stackPos)
        {
            return LuaDLL.luaL_checkboolean(L, stackPos);
        }

        public string LuaCheckLString(int numArg, out int len)
        {
            return LuaDLL.luaL_checklstring(L, numArg, out len);
        }

        public int LuaLoadBuffer(byte[] buff, int size, string name)
        {
            return LuaDLL.luaL_loadbuffer(L, buff, size, name);
        }

        public IntPtr LuaFindTable(int idx, string fname, int szhint = 1)
        {
            return LuaDLL.luaL_findtable(L, idx, fname, szhint);
        }

        public int LuaTypeError(int stackPos, string tname, string t2 = null)
        {
            return LuaDLL.luaL_typerror(L, stackPos, tname, t2);
        }

        public bool LuaDoString(string chunk, string chunkName = "@LuaStatePtr.cs")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(chunk);
            int status = LuaDLL.luaL_loadbuffer(L, buffer, buffer.Length, chunkName);

            if (status != 0)
            {
                return false;                
            }

            return LuaDLL.lua_pcall(L, 0, LuaDLL.LUA_MULTRET, 0) == 0;
            //return LuaDLL.luaL_dostring(L, chunk);
        }

        public bool LuaDoFile(string fileName)
        {
            int top = LuaGetTop();

            if (LuaDLL.luaL_dofile(L, fileName))
            {
                return true;
            }

            string err = LuaToString(-1);
            LuaSetTop(top);
            throw new LuaException(err, LuaException.GetLastError());
        }

        public void LuaGetMetaTable(string meta)
        {
            LuaDLL.luaL_getmetatable(L, meta);
        }

        public int LuaRef(int t)
        {
            return LuaDLL.luaL_ref(L, t);
        }

        public void LuaGetRef(int reference)
        {
            LuaDLL.lua_getref(L, reference);
        }

        public void LuaUnRef(int reference)
        {
            LuaDLL.lua_unref(L, reference);
        }

        public int LuaRequire(string fileName)
        {
#if UNITY_EDITOR
            string str = Path.GetExtension(fileName);

            if (str == ".lua")
            {
                throw new LuaException("Require not need file extension: " + str);
            }
#endif
            return LuaDLL.tolua_require(L, fileName);
        }

        //适合Awake OnSendMsg使用
        public void ThrowLuaException(Exception e)
        {
            if (LuaException.InstantiateCount > 0 || LuaException.SendMsgCount > 0)
            {
                LuaDLL.toluaL_exception(LuaException.L, e);
            }
            else
            {
                throw e;
            }
        }

        public int ToLuaRef()
        {
            return LuaDLL.toluaL_ref(L);
        }

        public int LuaUpdate(float delta, float unscaled)
        {
            return LuaDLL.tolua_update(L, delta, unscaled);
        }

        public int LuaLateUpdate()
        {
            return LuaDLL.tolua_lateupdate(L);
        }

        public int LuaFixedUpdate(float fixedTime)
        {
            return LuaDLL.tolua_fixedupdate(L, fixedTime);
        }

        public void OpenToLuaLibs()
        {
            LuaDLL.tolua_openlibs(L);
            LuaOpenJit();
        }

        public void ToLuaPushTraceback()
        {
            LuaDLL.tolua_pushtraceback(L);
        }

        public void ToLuaUnRef(int reference)
        {
            LuaDLL.toluaL_unref(L, reference);
        }
    }
}