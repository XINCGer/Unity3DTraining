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
using System.Reflection;
using System.Collections.Generic;

namespace LuaInterface
{
    public static class LuaMethodCache
    {
        public static Dictionary<Type, Dictionary<string, List<MethodInfo>>> dict = new Dictionary<Type, Dictionary<string, List<MethodInfo>>>();

        static MethodInfo GetMethod(Type t, string name, Type[] ts)
        {
            Dictionary<string, List<MethodInfo>> map = null;
            List<MethodInfo> list = null;

            if (!dict.TryGetValue(t, out map))
            {
                map = new Dictionary<string, List<MethodInfo>>();
                dict.Add(t, map);
            }

            if (!map.TryGetValue(name, out list))
            {
                list = new List<MethodInfo>();
                MethodInfo[] mds = t.GetMethods();

                for (int i = 0; i < mds.Length; i++)
                {
                    if (mds[i].Name == name)
                    {
                        list.Add(mds[i]);
                    }
                }

                map.Add(name, list);
            }

            if (list.Count == 1)
            {
                return list[0];
            }

            for (int i = 0; i < list.Count; i++)
            {
                ParameterInfo[] pis = list[i].GetParameters();
                bool flag = true;

                if (pis.Length == 0 && (ts == null || ts.Length == 0))
                {
                    return list[i];
                }
                else if (pis.Length == ts.Length)
                {
                    for (int j = 0; j < ts.Length; j++)
                    {
                        if (pis[j].ParameterType != ts[j])
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        return list[i];
                    }
                }
            }

            return null;
        }

        public static object CallSingleMethod(string name, object obj, params object[] args)
        {
            MethodInfo md = GetMethod(obj.GetType(), name, null);
            return md.Invoke(obj, args);
        }

        public static object CallMethod(string name, object obj, params object[] args)
        {
            Type[] ts = new Type[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                ts[i] = args[i].GetType();
            }

            MethodInfo md = GetMethod(obj.GetType(), name, ts);
            return md.Invoke(obj, args);
        }
    }
}
