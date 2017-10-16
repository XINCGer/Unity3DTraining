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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Runtime.CompilerServices;

namespace LuaInterface
{
    public class GCRef
    {
        public int reference;
        public string name = null;

        public GCRef(int reference, string name)
        {
            this.reference = reference;
            this.name = name;
        }
    }

    //让byte[] 压入成为lua string 而不是数组 userdata
    //也可以使用LuaByteBufferAttribute来标记byte[]
    public struct LuaByteBuffer
    {        
        public LuaByteBuffer(IntPtr source, int len)
        {
            buffer = new byte[len];
            Length = len;
            Marshal.Copy(source, buffer, 0, len);
        }
        
        public LuaByteBuffer(byte[] buf)
        {
            buffer = buf;
            Length = buf.Length;            
        }

        public LuaByteBuffer(byte[] buf, int len)
        {            
            buffer = buf;
            Length = len;
        }

        public LuaByteBuffer(System.IO.MemoryStream stream)
        {
            buffer = stream.GetBuffer();
            Length = (int)stream.Length;            
        }

        public static implicit operator LuaByteBuffer(System.IO.MemoryStream stream)
        {
            return new LuaByteBuffer(stream);
        }

        public byte[] buffer;    

        public int Length
        {
            get;
            private set;
        }    
    }   

    public class LuaOut<T> { }
    //public class LuaOutMetatable {}
    public class NullObject { }

    //泛型函数参数null代替
    public struct nil { }

    public class LuaDelegate
    {
        public LuaFunction func = null;
        public LuaTable self = null;
        public MethodInfo method = null; 

        public LuaDelegate(LuaFunction func)
        {
            this.func = func;
        }

        public LuaDelegate(LuaFunction func, LuaTable self)
        {
            this.func = func;
            this.self = self;
        }

        //如果count不是1，说明还有其他人引用，只能等待gc来处理
        public virtual void Dispose()
        {
            method = null;

            if (func != null)
            {
                func.Dispose(1);
                func = null;
            }

            if (self != null)
            {
                self.Dispose(1);
                self = null;
            }
        }

        public override bool Equals(object o)
        {                                    
            if (o == null) return func == null && self == null;
            LuaDelegate ld = o as LuaDelegate;

            if (ld == null || ld.func != func || ld.self != self)
            {
                return false;
            }

            return ld.func != null;
        }

        static bool CompareLuaDelegate(LuaDelegate a, LuaDelegate b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            object l = a;
            object r = b;

            if (l == null && r != null)
            {
                return b.func == null && b.self == null;
            }

            if (l != null && r == null)
            {
                return a.func == null && a.self == null;
            }

            if (a.func != b.func || a.self != b.self)
            {
                return false;
            }

            return a.func != null;
        }

        public static bool operator == (LuaDelegate a, LuaDelegate b)
        {
            return CompareLuaDelegate(a, b);
        }

        public static bool operator != (LuaDelegate a, LuaDelegate b)
        {
            return !CompareLuaDelegate(a, b);
        }
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);            
        }
    }

    [NoToLuaAttribute]
    public static class LuaMisc
    {
        public static string GetArrayRank(Type t)
        {
            int count = t.GetArrayRank();

            if (count == 1)
            {                
                return "[]";
            }

            using (CString.Block())
            {
                CString sb = CString.Alloc(64);
                sb.Append('[');

                for (int i = 1; i < count; i++)
                {
                    sb.Append(',');
                }

                sb.Append(']');
                return sb.ToString();
            }
        }

        public static string GetTypeName(Type t)
        {
            if (t.IsArray)
            {
                string str = GetTypeName(t.GetElementType());
                str += GetArrayRank(t);
                return str;                
            }
            else if (t.IsByRef)
            {
                t = t.GetElementType();
                return GetTypeName(t);
            }
            else if (t.IsGenericType)
            {
                return GetGenericName(t);
            }
            else if (t == typeof(void))
            {
                return "void";
            }
            else
            {
                string name = GetPrimitiveStr(t);
                return name.Replace('+', '.');
            }
        }

        public static string[] GetGenericName(Type[] types, int offset, int count)
        {
            string[] results = new string[count];

            for (int i = 0; i < count; i++)
            {
                int pos = i + offset;

                if (types[pos].IsGenericType)
                {
                    results[i] = GetGenericName(types[pos]);
                }
                else
                {
                    results[i] = GetTypeName(types[pos]);
                }

            }

            return results;
        }

        static string CombineTypeStr(string space, string name)
        {
            if (string.IsNullOrEmpty(space))
            {
                return name;
            }
            else
            {
                return space + "." + name;
            }
        }

        static string GetGenericName(Type t)
        {
            Type[] gArgs = t.GetGenericArguments();
            string typeName = t.FullName;
            int count = gArgs.Length;
            int pos = typeName.IndexOf("[");

            if (pos > 0)
            {
                typeName = typeName.Substring(0, pos);
            }

            string str = null;
            string name = null;
            int offset = 0;
            pos = typeName.IndexOf("+");

            while (pos > 0)
            {
                str = typeName.Substring(0, pos);
                typeName = typeName.Substring(pos + 1);
                pos = str.IndexOf('`');

                if (pos > 0)
                {
                    count = (int)(str[pos + 1] - '0');
                    str = str.Substring(0, pos);
                    str += "<" + string.Join(",", GetGenericName(gArgs, offset, count)) + ">";
                    offset += count;
                }

                name = CombineTypeStr(name, str);
                pos = typeName.IndexOf("+");
            }

            str = typeName;

            if (offset < gArgs.Length)
            {
                pos = str.IndexOf('`');
                count = (int)(str[pos + 1] - '0');
                str = str.Substring(0, pos);
                str += "<" + string.Join(",", GetGenericName(gArgs, offset, count)) + ">";
            }

            return CombineTypeStr(name, str);
        }

        public static Delegate GetEventHandler(object obj, Type t, string eventName)
        {
            FieldInfo eventField = t.GetField(eventName, BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return (Delegate)eventField.GetValue(obj);
        }

        public static string GetPrimitiveStr(Type t)
        {
            if (t == typeof(System.Single))
            {
                return "float";
            }
            else if (t == typeof(System.String))
            {
                return "string";
            }
            else if (t == typeof(System.Int32))
            {
                return "int";
            }
            else if (t == typeof(System.Double))
            {
                return "double";
            }
            else if (t == typeof(System.Boolean))
            {
                return "bool";
            }
            else if (t == typeof(System.UInt32))
            {
                return "uint";
            }
            else if (t == typeof(System.SByte))
            {
                return "sbyte";
            }
            else if (t == typeof(System.Byte))
            {
                return "byte";
            }
            else if (t == typeof(System.Int16))
            {
                return "short";
            }
            else if (t == typeof(System.UInt16))
            {
                return "ushort";
            }
            else if (t == typeof(System.Char))
            {
                return "char";
            }
            else if (t == typeof(System.Int64))
            {
                return "long";
            }
            else if (t == typeof(System.UInt64))
            {
                return "ulong";
            }
            else if (t == typeof(System.Decimal))
            {
                return "decimal";
            }
            else if (t == typeof(System.Object))
            {
                return "object";
            }
            else
            {
                return t.ToString();
            }
        }        

        public static double ToDouble(object obj)
        {
            Type t = obj.GetType();

            if (t == typeof(double) || t == typeof(float))
            {
                double d = Convert.ToDouble(obj);
                return d;
            }
            else if (t == typeof(int))
            {
                int n = Convert.ToInt32(obj);
                return (double)n;
            }
            else if (t == typeof(uint))
            {
                uint n = Convert.ToUInt32(obj);
                return (double)n;
            }
            else if (t == typeof(long))
            {
                long n = Convert.ToInt64(obj);
                return (double)n;
            }
            else if (t == typeof(ulong))
            {
                ulong n = Convert.ToUInt64(obj);
                return (double)n;
            }
            else if (t == typeof(byte))
            {
                byte b = Convert.ToByte(obj);
                return (double)b;
            }
            else if (t == typeof(sbyte))
            {
                sbyte b = Convert.ToSByte(obj);
                return (double)b;
            }
            else if (t == typeof(char))
            {
                char c = Convert.ToChar(obj);
                return (double)c;
            }            
            else if (t == typeof(short))
            {
                Int16 n = Convert.ToInt16(obj);
                return (double)n;
            }
            else if (t == typeof(ushort))
            {
                UInt16 n = Convert.ToUInt16(obj);
                return (double)n;
            }

            return 0;
        }

        //可产生导出文件的基类
        public static Type GetExportBaseType(Type t)
        {
            Type baseType = t.BaseType;

            if (baseType == typeof(ValueType))
            {
                return null;
            }

            if (t.IsAbstract && t.IsSealed)
            {
                return baseType == typeof(object) ? null : baseType;
            }

            return baseType;
        }
    }       

    /*[NoToLuaAttribute]
    public struct LuaInteger64
    {
        public long i64;

        public LuaInteger64(long i64)
        {
            this.i64 = i64;
        }

        public static implicit operator LuaInteger64(long i64)
        {
            return new LuaInteger64(i64);
        }

        public static implicit operator long(LuaInteger64 self)
        {
            return self.i64;
        }

        public ulong ToUInt64()
        {
            return (ulong)i64;
        }

        public override string ToString()
        {
            return Convert.ToString(i64);
        }
    }*/

    public class TouchBits
    {
        public const int DeltaPosition = 1;
        public const int Position = 2;
        public const int RawPosition = 4;
        public const int ALL = 7;
    }

    public class RaycastBits
    {
        public const int Collider = 1;
        public const int Normal = 2;
        public const int Point = 4;
        public const int Rigidbody = 8;
        public const int Transform = 16;
        public const int ALL = 31;
    }

    public enum EventOp
    {
        None = 0,
        Add = 1,
        Sub = 2,
    }

    public class EventObject
    {
        [NoToLuaAttribute]
        public EventOp op = EventOp.None;
        [NoToLuaAttribute]
        public Delegate func = null;
        [NoToLuaAttribute]
        public Type type;

        [NoToLuaAttribute]
        public EventObject(Type t)
        {
            type = t;
        }

        public static EventObject operator +(EventObject a, Delegate b)
        {
            a.op = EventOp.Add;
            a.func = b;
            return a;
        }

        public static EventObject operator -(EventObject a, Delegate b)
        {
            a.op = EventOp.Sub;
            a.func = b;
            return a;
        }
    }
}

