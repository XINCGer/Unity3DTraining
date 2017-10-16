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
using System.Globalization;
using System.Reflection;

namespace LuaInterface
{
    //代表一个反射属性
    public sealed class LuaProperty
    {
        PropertyInfo property = null;
        Type kclass = null;        

        [NoToLuaAttribute]
        public LuaProperty(PropertyInfo prop, Type t)
        {
            property = prop;
            kclass = t;            
        }

        public int Get(IntPtr L)
        {
            int count = LuaDLL.lua_gettop(L);            

            if (count == 3)
            {
                object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                object[] arg1 = ToLua.CheckObjectArray(L, 3);
                object o = property.GetValue(arg0, arg1);                
                ToLua.Push(L, o);
                return 1;
            }
            else if (count == 6)
            {
                object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                BindingFlags arg1 = (BindingFlags)LuaDLL.luaL_checknumber(L, 3);
                Binder arg2 = (Binder)ToLua.CheckObject(L, 4, typeof(Binder));
                object[] arg3 = ToLua.CheckObjectArray(L, 5);
                CultureInfo arg4 = (CultureInfo)ToLua.CheckObject(L, 6, typeof(CultureInfo));
                object o = property.GetValue(arg0, arg1, arg2, arg3, arg4);
                ToLua.Push(L, o);
                return 1;
            }
            else
            {
                return LuaDLL.luaL_throw(L, "invalid arguments to method: LuaInterface.LuaProperty.Get");
            }
        }

        public int Set(IntPtr L)
        {
            int count = LuaDLL.lua_gettop(L);            

            if (count == 4)
            {
                object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                object arg1 = ToLua.ToVarObject(L, 3);
                if (arg1 != null) arg1 = TypeChecker.ChangeType(arg1, property.PropertyType);
                object[] arg2 = ToLua.CheckObjectArray(L, 4);                
                property.SetValue(arg0, arg1, arg2);
                return 0;
            }
            else if (count == 7)
            {
                object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                object arg1 = ToLua.ToVarObject(L, 3);
                if (arg1 != null) arg1 = TypeChecker.ChangeType(arg1, property.PropertyType);
                BindingFlags arg2 = (BindingFlags)LuaDLL.luaL_checknumber(L, 4);
                Binder arg3 = (Binder)ToLua.CheckObject(L, 5, typeof(Binder));
                object[] arg4 = ToLua.CheckObjectArray(L, 6);
                CultureInfo arg5 = (CultureInfo)ToLua.CheckObject(L, 7, typeof(CultureInfo));                
                property.SetValue(arg0, arg1, arg2, arg3, arg4, arg5);
                return 0;
            }
            else
            {
                return LuaDLL.luaL_throw(L, "invalid arguments to method: LuaInterface.LuaProperty.Set");
            }            
        }
    }
}
