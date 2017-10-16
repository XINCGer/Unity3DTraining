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
    public sealed class LuaField
    {
        FieldInfo field = null;
        Type kclass = null;

        [NoToLuaAttribute]
        public LuaField(FieldInfo info, Type t)
        {
            field = info;
            kclass = t;            
        }

        public int Get(IntPtr L)
        {
            try
            {
                ToLua.CheckArgsCount(L, 2);                
                object arg0 = ToLua.CheckObject(L, 2, kclass);
                object o = field.GetValue(arg0);

                if (o == null)
                {
                    if (typeof(System.MulticastDelegate).IsAssignableFrom(field.FieldType))
                    {
                        o = DelegateFactory.CreateDelegate(field.FieldType, null);
                        ToLua.Push(L, (Delegate)o);
                    }
                    else
                    {
                        LuaDLL.lua_pushnil(L);
                    }
                }
                else
                {
                    ToLua.Push(L, o);
                }

                return 1;
            }
            catch (Exception e)
            {
                return LuaDLL.toluaL_exception(L, e);
            }            
        }

        public int Set(IntPtr L)
        {
            try
            {
                int count = LuaDLL.lua_gettop(L);

                if (count == 3)
                {
                    object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                    object arg1 = ToLua.ToVarObject(L, 3);
                    if (arg1 != null) arg1 = TypeChecker.ChangeType(arg1, field.FieldType);
                    field.SetValue(arg0, arg1);
                    return 0;
                }
                else if (count == 6)
                {
                    object arg0 = ToLua.CheckVarObject(L, 2, kclass);
                    object arg1 = ToLua.ToVarObject(L, 3);
                    if (arg1 != null) arg1 = TypeChecker.ChangeType(arg1, field.FieldType);
                    BindingFlags arg2 = (BindingFlags)LuaDLL.luaL_checknumber(L, 4);
                    Binder arg3 = (Binder)ToLua.CheckObject(L, 5, typeof(Binder));
                    CultureInfo arg4 = (CultureInfo)ToLua.CheckObject(L, 6, typeof(CultureInfo));                    
                    field.SetValue(arg0, arg1, arg2, arg3, arg4);
                    return 0;
                }
                else
                {
                    return LuaDLL.luaL_throw(L, "invalid arguments to method: LuaField.Set");
                }
            }
            catch (Exception e)
            {
                return LuaDLL.toluaL_exception(L, e);
            }
        }
    }
}
