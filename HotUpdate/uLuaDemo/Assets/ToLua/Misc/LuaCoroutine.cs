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
using LuaInterface;
using System;
using System.Collections;

public static class LuaCoroutine
{
    static MonoBehaviour mb = null;

    static string strCo =
        @"
        local _WaitForSeconds, _WaitForFixedUpdate, _WaitForEndOfFrame, _Yield, _StopCoroutine = WaitForSeconds, WaitForFixedUpdate, WaitForEndOfFrame, Yield, StopCoroutine        
        local error = error
        local debug = debug
        local coroutine = coroutine
        local comap = {}
        setmetatable(comap, {__mode = 'k'})

        function _resume(co)
            if comap[co] then
                comap[co] = nil
                local flag, msg = coroutine.resume(co)
                    
                if not flag then
                    msg = debug.traceback(co, msg)
                    error(msg)
                end
            end        
        end

        function WaitForSeconds(t)
            local co = coroutine.running()
            local resume = function()                    
                _resume(co)                     
            end
            
            comap[co] = _WaitForSeconds(t, resume)
            return coroutine.yield()
        end

        function WaitForFixedUpdate()
            local co = coroutine.running()
            local resume = function()          
                _resume(co)     
            end
        
            comap[co] = _WaitForFixedUpdate(resume)
            return coroutine.yield()
        end

        function WaitForEndOfFrame()
            local co = coroutine.running()
            local resume = function()        
                _resume(co)     
            end
        
            comap[co] = _WaitForEndOfFrame(resume)
            return coroutine.yield()
        end

        function Yield(o)
            local co = coroutine.running()
            local resume = function()        
                _resume(co)     
            end
        
            comap[co] = _Yield(o, resume)
            return coroutine.yield()
        end

        function StartCoroutine(func)
            local co = coroutine.create(func)                       
            local flag, msg = coroutine.resume(co)

            if not flag then
                msg = debug.traceback(co, msg)
                error(msg)
            end

            return co
        end

        function StopCoroutine(co)
            local _co = comap[co]

            if _co == nil then
                return
            end

            comap[co] = nil
            _StopCoroutine(_co)
        end
        ";

    public static void Register(LuaState state, MonoBehaviour behaviour)
    {
        state.BeginModule(null);
        state.RegFunction("WaitForSeconds", _WaitForSeconds);
        state.RegFunction("WaitForFixedUpdate", WaitForFixedUpdate);
        state.RegFunction("WaitForEndOfFrame", WaitForEndOfFrame);
        state.RegFunction("Yield", Yield);
        state.RegFunction("StopCoroutine", StopCoroutine);
        state.EndModule();

        state.LuaDoString(strCo, "@LuaCoroutine.cs");
        mb = behaviour;
    }

    //另一种方式，非脚本回调方式(用脚本方式更好，可避免lua_yield异常出现在c#函数中)
    /*[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int WaitForSeconds(IntPtr L)
    {
        try
        {
            LuaState state = LuaState.Get(L);
            LuaDLL.lua_pushthread(L);
            LuaThread thread = ToLua.ToLuaThread(L, -1);
            float sec = (float)LuaDLL.luaL_checknumber(L, 1);
            mb.StartCoroutine(CoWaitForSeconds(sec, thread));
            return LuaDLL.lua_yield(L, 0);
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static IEnumerator CoWaitForSeconds(float sec, LuaThread thread)
    {
        yield return new WaitForSeconds(sec);
        thread.Resume();
    }*/

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int _WaitForSeconds(IntPtr L)
    {
        try
        {
            float sec = (float)LuaDLL.luaL_checknumber(L, 1);
            LuaFunction func = ToLua.ToLuaFunction(L, 2);
            Coroutine co = mb.StartCoroutine(CoWaitForSeconds(sec, func));
            ToLua.PushSealed(L, co);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static IEnumerator CoWaitForSeconds(float sec, LuaFunction func)
    {
        yield return new WaitForSeconds(sec);
        func.Call();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int WaitForFixedUpdate(IntPtr L)
    {
        try
        {
            LuaFunction func = ToLua.ToLuaFunction(L, 1);
            Coroutine co = mb.StartCoroutine(CoWaitForFixedUpdate(func));
            ToLua.PushSealed(L, co);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static IEnumerator CoWaitForFixedUpdate(LuaFunction func)
    {
        yield return new WaitForFixedUpdate();
        func.Call();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int WaitForEndOfFrame(IntPtr L)
    {
        try
        {
            LuaFunction func = ToLua.ToLuaFunction(L, 1);
            Coroutine co = mb.StartCoroutine(CoWaitForEndOfFrame(func));
            ToLua.PushSealed(L, co);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static IEnumerator CoWaitForEndOfFrame(LuaFunction func)
    {
        yield return new WaitForEndOfFrame();
        func.Call();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int Yield(IntPtr L)
    {
        try
        {
            object o = ToLua.ToVarObject(L, 1);
            LuaFunction func = ToLua.ToLuaFunction(L, 2);
            Coroutine co = mb.StartCoroutine(CoYield(o, func));
            ToLua.PushSealed(L, co);
            return 1;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }

    static IEnumerator CoYield(object o, LuaFunction func)
    {
        if (o is IEnumerator)
        {
            yield return mb.StartCoroutine((IEnumerator)o);
        }
        else
        {
            yield return o;
        }

        func.Call();
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int StopCoroutine(IntPtr L)
    {
        try
        {
            Coroutine co = (Coroutine)ToLua.CheckObject(L, 1, typeof(Coroutine));
            mb.StopCoroutine(co);
            return 0;
        }
        catch (Exception e)
        {
            return LuaDLL.toluaL_exception(L, e);
        }
    }
}

