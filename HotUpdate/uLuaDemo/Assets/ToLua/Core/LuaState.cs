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
#define MISS_WARNING

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace LuaInterface
{
    public class LuaState : LuaStatePtr, IDisposable
    {
        public ObjectTranslator translator = new ObjectTranslator();
        public LuaReflection reflection = new LuaReflection();

        public int ArrayMetatable { get; private set; }
        public int DelegateMetatable { get; private set; }
        public int TypeMetatable { get; private set; }
        public int EnumMetatable { get; private set; }
        public int IterMetatable { get; private set; }        
        public int EventMetatable { get; private set; }

        //function ref                
        public int PackBounds { get; private set; }
        public int UnpackBounds { get; private set; }
        public int PackRay { get; private set; }
        public int UnpackRay { get; private set; }
        public int PackRaycastHit { get; private set; }        
        public int PackTouch { get; private set; }

        public bool LogGC 
        {
            get
            {
                return beLogGC;
            }

            set
            {
                beLogGC = value;
                translator.LogGC = value;
            }
        }

        public Action OnDestroy = delegate { };
        
        Dictionary<string, WeakReference> funcMap = new Dictionary<string, WeakReference>();
        Dictionary<int, WeakReference> funcRefMap = new Dictionary<int, WeakReference>();
        Dictionary<long, WeakReference> delegateMap = new Dictionary<long, WeakReference>();

        List<GCRef> gcList = new List<GCRef>();
        List<LuaBaseRef> subList = new List<LuaBaseRef>();

        Dictionary<Type, int> metaMap = new Dictionary<Type, int>();        
        Dictionary<Enum, object> enumMap = new Dictionary<Enum, object>();
        Dictionary<Type, LuaCSFunction> preLoadMap = new Dictionary<Type, LuaCSFunction>();

        Dictionary<int, Type> typeMap = new Dictionary<int, Type>();
        HashSet<Type> genericSet = new HashSet<Type>();
        HashSet<string> moduleSet = null;

        private static LuaState mainState = null;
        private static Dictionary<IntPtr, LuaState> stateMap = new Dictionary<IntPtr, LuaState>();

        private int beginCount = 0;
        private bool beLogGC = false;

#if UNITY_EDITOR
        private bool beStart = false;
#endif

#if MISS_WARNING
        HashSet<Type> missSet = new HashSet<Type>();
#endif

        public LuaState()            
        {
            if (mainState == null)
            {
                mainState = this;
            }

            float time = Time.realtimeSinceStartup;
            InitTypeTraits();
            InitStackTraits();
            L = LuaNewState();            
            LuaException.Init(L);
            stateMap.Add(L, this);                        
            OpenToLuaLibs();            
            ToLua.OpenLibs(L);
            OpenBaseLibs();
            LuaSetTop(0);
            InitLuaPath();
            Debugger.Log("Init lua state cost: {0}", Time.realtimeSinceStartup - time);
        }        

        void OpenBaseLibs()
        {            
            BeginModule(null);

            BeginModule("System");
            System_ObjectWrap.Register(this);
            System_NullObjectWrap.Register(this);            
            System_StringWrap.Register(this);
            System_DelegateWrap.Register(this);
            System_EnumWrap.Register(this);
            System_ArrayWrap.Register(this);
            System_TypeWrap.Register(this);                                               
            BeginModule("Collections");
            System_Collections_IEnumeratorWrap.Register(this);

            BeginModule("ObjectModel");
            System_Collections_ObjectModel_ReadOnlyCollectionWrap.Register(this);
            EndModule();//ObjectModel

            BeginModule("Generic");
            System_Collections_Generic_ListWrap.Register(this);
            System_Collections_Generic_DictionaryWrap.Register(this);
            System_Collections_Generic_KeyValuePairWrap.Register(this);

            BeginModule("Dictionary");
            System_Collections_Generic_Dictionary_KeyCollectionWrap.Register(this);
            System_Collections_Generic_Dictionary_ValueCollectionWrap.Register(this);
            EndModule();//Dictionary
            EndModule();//Generic
            EndModule();//Collections     
            EndModule();//end System

            BeginModule("LuaInterface");
            LuaInterface_LuaOutWrap.Register(this);
            LuaInterface_EventObjectWrap.Register(this);
            EndModule();//end LuaInterface

            BeginModule("UnityEngine");
            UnityEngine_ObjectWrap.Register(this);            
            UnityEngine_CoroutineWrap.Register(this);
            EndModule(); //end UnityEngine

            EndModule(); //end global
                        
            LuaUnityLibs.OpenLibs(L);            
            LuaReflection.OpenLibs(L);
            ArrayMetatable = metaMap[typeof(System.Array)];
            TypeMetatable = metaMap[typeof(System.Type)];
            DelegateMetatable = metaMap[typeof(System.Delegate)];
            EnumMetatable = metaMap[typeof(System.Enum)];
            IterMetatable = metaMap[typeof(IEnumerator)];
            EventMetatable = metaMap[typeof(EventObject)];
        }

        void InitLuaPath()
        {
            InitPackagePath();

            if (!LuaFileUtils.Instance.beZip)
            {
#if UNITY_EDITOR
                if (!Directory.Exists(LuaConst.luaDir))
                {
                    string msg = string.Format("luaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.luaDir);
                    throw new LuaException(msg);
                }

                if (!Directory.Exists(LuaConst.toluaDir))
                {
                    string msg = string.Format("toluaDir path not exists: {0}, configer it in LuaConst.cs", LuaConst.toluaDir);
                    throw new LuaException(msg);
                }

                AddSearchPath(LuaConst.toluaDir);
                AddSearchPath(LuaConst.luaDir);
#endif
                if (LuaFileUtils.Instance.GetType() == typeof(LuaFileUtils))
                {
                    AddSearchPath(LuaConst.luaResDir);
                }
            }
        }

        void OpenBaseLuaLibs()
        {
            DoFile("tolua.lua");            //tolua table名字已经存在了,不能用require
            LuaUnityLibs.OpenLuaLibs(L);
        }

        public void Start()
        {
#if UNITY_EDITOR
            beStart = true;
#endif
            Debugger.Log("LuaState start");
            OpenBaseLuaLibs();
            PackBounds = GetFuncRef("Bounds.New");
            UnpackBounds = GetFuncRef("Bounds.Get");
            PackRay = GetFuncRef("Ray.New");
            UnpackRay = GetFuncRef("Ray.Get");
            PackRaycastHit = GetFuncRef("RaycastHit.New");
            PackTouch = GetFuncRef("Touch.New");
        }

        public int OpenLibs(LuaCSFunction open)
        {
            int ret = open(L);            
            return ret;
        }

        public void BeginPreLoad()
        {
            LuaGetGlobal("package");
            LuaGetField(-1, "preload");
            moduleSet = new HashSet<string>();
        }

        public void EndPreLoad()
        {
            LuaPop(2);
            moduleSet = null;
        }

        public void AddPreLoad(string name, LuaCSFunction func, Type type)
        {            
            if (!preLoadMap.ContainsKey(type))
            {
                LuaDLL.tolua_pushcfunction(L, func);
                LuaSetField(-2, name);
                preLoadMap[type] = func;
                string module = type.Namespace;

                if (!string.IsNullOrEmpty(module) && !moduleSet.Contains(module))
                {
                    LuaDLL.tolua_addpreload(L, module);
                    moduleSet.Add(module);
                }
            }            
        }

        //慎用，需要自己保证不会重复Add相同的name,并且上面函数没有使用过这个name
        public void AddPreLoad(string name, LuaCSFunction func)
        {
            LuaDLL.tolua_pushcfunction(L, func);
            LuaSetField(-2, name);
        }

        public int BeginPreModule(string name)
        {
            int top = LuaGetTop();

            if (string.IsNullOrEmpty(name))
            {
                LuaDLL.lua_pushvalue(L, LuaIndexes.LUA_GLOBALSINDEX);
                ++beginCount;
                return top;
            }
            else if (LuaDLL.tolua_beginpremodule(L, name))
            {
                ++beginCount;
                return top;
            }
            
            throw new LuaException(string.Format("create table {0} fail", name));            
        }

        public void EndPreModule(int reference)
        {
            --beginCount;            
            LuaDLL.tolua_endpremodule(L, reference);
        }

        public void EndPreModule(IntPtr L, int reference)
        {
            --beginCount;
            LuaDLL.tolua_endpremodule(L, reference);
        }

        public void BindPreModule(Type t, LuaCSFunction func)
        {
            preLoadMap[t] = func;
        }

        public LuaCSFunction GetPreModule(Type t)
        {
            LuaCSFunction func = null;
            preLoadMap.TryGetValue(t, out func);
            return func;
        }

        public bool BeginModule(string name)
        {
#if UNITY_EDITOR
            if (name != null)
            {                
                LuaTypes type = LuaType(-1);

                if (type != LuaTypes.LUA_TTABLE)
                {                    
                    throw new LuaException("open global module first");
                }
            }
#endif
            if (LuaDLL.tolua_beginmodule(L, name))
            {
                ++beginCount;
                return true;
            }

            LuaSetTop(0);
            throw new LuaException(string.Format("create table {0} fail", name));            
        }

        public void EndModule()
        {
            --beginCount;            
            LuaDLL.tolua_endmodule(L);
        }

        void BindTypeRef(int reference, Type t)
        {
            metaMap.Add(t, reference);
            typeMap.Add(reference, t);

            if (t.IsGenericTypeDefinition)
            {
                genericSet.Add(t);
            }
        }

        public Type GetClassType(int reference)
        {
            Type t = null;
            typeMap.TryGetValue(reference, out t);
            return t;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        public static int Collect(IntPtr L)
        {
            int udata = LuaDLL.tolua_rawnetobj(L, 1);

            if (udata != -1)
            {
                ObjectTranslator translator = GetTranslator(L);
                translator.RemoveObject(udata);
            }

            return 0;
        }

        string GetToLuaTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                string str = t.Name;
                int pos = str.IndexOf('`');

                if (pos > 0)
                {
                    str = str.Substring(0, pos);
                }

                return str;
            }

            return t.Name;
        }

        public int BeginClass(Type t, Type baseType, string name = null)
        {
            if (beginCount == 0)
            {
                throw new LuaException("must call BeginModule first");
            }

            int baseMetaRef = 0;
            int reference = 0;            

            if (name == null)
            {
                name = GetToLuaTypeName(t);
            }

            if (baseType != null && !metaMap.TryGetValue(baseType, out baseMetaRef))
            {
                LuaCreateTable();
                baseMetaRef = LuaRef(LuaIndexes.LUA_REGISTRYINDEX);                
                BindTypeRef(baseMetaRef, baseType);
            }

            if (metaMap.TryGetValue(t, out reference))
            {
                LuaDLL.tolua_beginclass(L, name, baseMetaRef, reference);
                RegFunction("__gc", Collect);
            }
            else
            {
                reference = LuaDLL.tolua_beginclass(L, name, baseMetaRef);
                RegFunction("__gc", Collect);                
                BindTypeRef(reference, t);
            }

            return reference;
        }

        public void EndClass()
        {
            LuaDLL.tolua_endclass(L);
        }

        public int BeginEnum(Type t)
        {
            if (beginCount == 0)
            {
                throw new LuaException("must call BeginModule first");
            }

            int reference = LuaDLL.tolua_beginenum(L, t.Name);
            RegFunction("__gc", Collect);            
            BindTypeRef(reference, t);
            return reference;
        }

        public void EndEnum()
        {
            LuaDLL.tolua_endenum(L);
        }

        public void BeginStaticLibs(string name)
        {
            if (beginCount == 0)
            {
                throw new LuaException("must call BeginModule first");
            }

            LuaDLL.tolua_beginstaticclass(L, name);
        }

        public void EndStaticLibs()
        {
            LuaDLL.tolua_endstaticclass(L);
        }

        public void RegFunction(string name, LuaCSFunction func)
        {
            IntPtr fn = Marshal.GetFunctionPointerForDelegate(func);
            LuaDLL.tolua_function(L, name, fn);            
        }

        public void RegVar(string name, LuaCSFunction get, LuaCSFunction set)
        {            
            IntPtr fget = IntPtr.Zero;
            IntPtr fset = IntPtr.Zero;

            if (get != null)
            {
                fget = Marshal.GetFunctionPointerForDelegate(get);
            }

            if (set != null)
            {
                fset = Marshal.GetFunctionPointerForDelegate(set);
            }

            LuaDLL.tolua_variable(L, name, fget, fset);
        }

        public void RegConstant(string name, double d)
        {
            LuaDLL.tolua_constant(L, name, d);
        }

        public void RegConstant(string name, bool flag)
        {
            LuaDLL.lua_pushstring(L, name);
            LuaDLL.lua_pushboolean(L, flag);
            LuaDLL.lua_rawset(L, -3);
        }

        int GetFuncRef(string name)
        {
            if (PushLuaFunction(name, false))
            {
                return LuaRef(LuaIndexes.LUA_REGISTRYINDEX);
            }

            throw new LuaException("get lua function reference failed: " + name);                         
        }

        public static LuaState Get(IntPtr ptr)
        {
#if !MULTI_STATE
            return mainState;
#else

            if (mainState != null && mainState.L == ptr)
            {
                return mainState;
            }

            LuaState state = null;

            if (stateMap.TryGetValue(ptr, out state))
            {
                return state;
            }
            else
            {                
                return Get(LuaDLL.tolua_getmainstate(ptr));
            }
#endif
        }

        public static ObjectTranslator GetTranslator(IntPtr ptr)
        {
#if !MULTI_STATE
            return mainState.translator;
#else
            if (mainState != null && mainState.L == ptr)
            {
                return mainState.translator;
            }

            return Get(ptr).translator;
#endif
        }

        public static LuaReflection GetReflection(IntPtr ptr)
        {
#if !MULTI_STATE
            return mainState.reflection;
#else
            if (mainState != null && mainState.L == ptr)
            {
                return mainState.reflection;
            }

            return Get(ptr).reflection;
#endif
        }

        public void DoString(string chunk, string chunkName = "LuaState.cs")
        {
#if UNITY_EDITOR
            if (!beStart)
            {
                throw new LuaException("you must call Start() first to initialize LuaState");
            }
#endif
            byte[] buffer = Encoding.UTF8.GetBytes(chunk);
            LuaLoadBuffer(buffer, chunkName);
        }

        public T DoString<T>(string chunk, string chunkName = "LuaState.cs")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(chunk);
            return LuaLoadBuffer<T>(buffer, chunkName);
        }

        byte[] LoadFileBuffer(string fileName)
        {
#if UNITY_EDITOR
            if (!beStart)
            {
                throw new LuaException("you must call Start() first to initialize LuaState");
            }
#endif
            byte[] buffer = LuaFileUtils.Instance.ReadFile(fileName);

            if (buffer == null)
            {
                string error = string.Format("cannot open {0}: No such file or directory", fileName);
                error += LuaFileUtils.Instance.FindFileError(fileName);
                throw new LuaException(error);
            }

            return buffer;
        }

        string LuaChunkName(string name)
        {
            if (LuaConst.openLuaDebugger)
            {
                name = LuaFileUtils.Instance.FindFile(name);
            }

            return "@" + name;
        }

        public void DoFile(string fileName)
        {
            byte[] buffer = LoadFileBuffer(fileName);
            fileName = LuaChunkName(fileName);
            LuaLoadBuffer(buffer, fileName);
        }

        public T DoFile<T>(string fileName)
        {
            byte[] buffer = LoadFileBuffer(fileName);
            fileName = LuaChunkName(fileName);
            return LuaLoadBuffer<T>(buffer, fileName);
        }

        //注意fileName与lua文件中require一致。
        public void Require(string fileName)
        {
            int top = LuaGetTop();
            int ret = LuaRequire(fileName);

            if (ret != 0)
            {                
                string err = LuaToString(-1);
                LuaSetTop(top);
                throw new LuaException(err, LuaException.GetLastError());
            }

            LuaSetTop(top);            
        }

        public T Require<T>(string fileName)
        {
            int top = LuaGetTop();
            int ret = LuaRequire(fileName);

            if (ret != 0)
            {
                string err = LuaToString(-1);
                LuaSetTop(top);
                throw new LuaException(err, LuaException.GetLastError());
            }

            T o = CheckValue<T>(-1);
            LuaSetTop(top);
            return o;
        }

        public void InitPackagePath()
        {
            LuaGetGlobal("package");
            LuaGetField(-1, "path");
            string current = LuaToString(-1);
            string[] paths = current.Split(';');

            for (int i = 0; i < paths.Length; i++)
            {
                if (!string.IsNullOrEmpty(paths[i]))
                {
                    string path = paths[i].Replace('\\', '/');
                    LuaFileUtils.Instance.AddSearchPath(path);
                }
            }

            LuaPushString("");            
            LuaSetField(-3, "path");
            LuaPop(2);
        }

        string ToPackagePath(string path)
        {
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                sb.Append(path);
                sb.Replace('\\', '/');

                if (sb.Length > 0 && sb[sb.Length - 1] != '/')
                {
                    sb.Append('/');
                }

                sb.Append("?.lua");
                return sb.ToString();
            }
        }

        public void AddSearchPath(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
            {
                throw new LuaException(fullPath + " is not a full path");
            }

            fullPath = ToPackagePath(fullPath);
            LuaFileUtils.Instance.AddSearchPath(fullPath);        
        }

        public void RemoveSeachPath(string fullPath)
        {
            if (!Path.IsPathRooted(fullPath))
            {
                throw new LuaException(fullPath + " is not a full path");
            }

            fullPath = ToPackagePath(fullPath);
            LuaFileUtils.Instance.RemoveSearchPath(fullPath);
        }        

        public int BeginPCall(int reference)
        {
            return LuaDLL.tolua_beginpcall(L, reference);
        }

        public void PCall(int args, int oldTop)
        {            
            if (LuaDLL.lua_pcall(L, args, LuaDLL.LUA_MULTRET, oldTop) != 0)
            {
                string error = LuaToString(-1);
                throw new LuaException(error, LuaException.GetLastError());
            }            
        }

        public void EndPCall(int oldTop)
        {
            LuaDLL.lua_settop(L, oldTop - 1);            
        }

        public void PushArgs(object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                PushVariant(args[i]);
            }
        }

        void CheckNull(LuaBaseRef lbr, string fmt, object arg0)
        {
            if (lbr == null)
            {
                string error = string.Format(fmt, arg0);
                throw new LuaException(error, null, 2);
            }            
        }

        //压入一个存在的或不存在的table, 但不增加引用计数
        bool PushLuaTable(string fullPath, bool checkMap = true)
        {
            if (checkMap)
            {
                WeakReference weak = null;

                if (funcMap.TryGetValue(fullPath, out weak))
                {
                    if (weak.IsAlive)
                    {
                        LuaTable table = weak.Target as LuaTable;
                        CheckNull(table, "{0} not a lua table", fullPath);
                        Push(table);
                        return true;
                    }
                    else
                    {
                        funcMap.Remove(fullPath);
                    }
                }
            }

            if (!LuaDLL.tolua_pushluatable(L, fullPath))
            {                
                return false;
            }

            return true;
        }

        bool PushLuaFunction(string fullPath, bool checkMap = true)
        {
            if (checkMap)
            {
                WeakReference weak = null;

                if (funcMap.TryGetValue(fullPath, out weak))
                {
                    if (weak.IsAlive)
                    {
                        LuaFunction func = weak.Target as LuaFunction;
                        CheckNull(func, "{0} not a lua function", fullPath);

                        if (func.IsAlive)
                        {
                            func.AddRef();
                            return true;
                        }
                    }

                    funcMap.Remove(fullPath);
                }
            }

            int oldTop = LuaDLL.lua_gettop(L);
            int pos = fullPath.LastIndexOf('.');

            if (pos > 0)
            {
                string tableName = fullPath.Substring(0, pos);

                if (PushLuaTable(tableName, checkMap))
                {
                    string funcName = fullPath.Substring(pos + 1);
                    LuaDLL.lua_pushstring(L, funcName);
                    LuaDLL.lua_rawget(L, -2);

                    LuaTypes type = LuaDLL.lua_type(L, -1);

                    if (type == LuaTypes.LUA_TFUNCTION)
                    {
                        LuaDLL.lua_insert(L, oldTop + 1);
                        LuaDLL.lua_settop(L, oldTop + 1);
                        return true;
                    }
                }

                LuaDLL.lua_settop(L, oldTop);
                return false;
            }
            else
            {
                LuaDLL.lua_getglobal(L, fullPath);
                LuaTypes type = LuaDLL.lua_type(L, -1);

                if (type != LuaTypes.LUA_TFUNCTION)
                {
                    LuaDLL.lua_settop(L, oldTop);
                    return false;
                }
            }

            return true;
        }

        void RemoveFromGCList(int reference)
        {            
            lock (gcList)
            {                
                for (int i = 0; i < gcList.Count; i++)
                {
                    if (gcList[i].reference == reference)
                    {
                        gcList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public LuaFunction GetFunction(string name, bool beLogMiss = true)
        {
            WeakReference weak = null;

            if (funcMap.TryGetValue(name, out weak))
            {
                if (weak.IsAlive)
                {
                    LuaFunction func = weak.Target as LuaFunction;
                    CheckNull(func, "{0} not a lua function", name);

                    if (func.IsAlive)
                    {
                        func.AddRef();
                        RemoveFromGCList(func.GetReference());
                        return func;
                    }
                }

                funcMap.Remove(name);
            }

            if (PushLuaFunction(name, false))
            {
                int reference = ToLuaRef();

                if (funcRefMap.TryGetValue(reference, out weak))
                {
                    if (weak.IsAlive)
                    {
                        LuaFunction func = weak.Target as LuaFunction;
                        CheckNull(func, "{0} not a lua function", name);

                        if (func.IsAlive)
                        {
                            funcMap.Add(name, weak);
                            func.AddRef();
                            RemoveFromGCList(reference);
                            return func;
                        }
                    }

                    funcRefMap.Remove(reference);
                    delegateMap.Remove(reference);
                }
                
                LuaFunction fun = new LuaFunction(reference, this);
                fun.name = name;
                funcMap.Add(name, new WeakReference(fun));
                funcRefMap.Add(reference, new WeakReference(fun));
                RemoveFromGCList(reference);
                if (LogGC) Debugger.Log("Alloc LuaFunction name {0}, id {1}", name, reference);                
                return fun;
            }

            if (beLogMiss)
            {
                Debugger.Log("Lua function {0} not exists", name);                
            }

            return null;
        }

        LuaBaseRef TryGetLuaRef(int reference)
        {            
            WeakReference weak = null;

            if (funcRefMap.TryGetValue(reference, out weak))
            {
                if (weak.IsAlive)
                {
                    LuaBaseRef luaRef = (LuaBaseRef)weak.Target;

                    if (luaRef.IsAlive)
                    {
                        luaRef.AddRef();
                        return luaRef;
                    }
                }                

                funcRefMap.Remove(reference);                
            }

            return null;
        }

        public LuaFunction GetFunction(int reference)
        {
            LuaFunction func = TryGetLuaRef(reference) as LuaFunction;

            if (func == null)
            {                
                func = new LuaFunction(reference, this);
                funcRefMap.Add(reference, new WeakReference(func));
                if (LogGC) Debugger.Log("Alloc LuaFunction name , id {0}", reference);      
            }

            RemoveFromGCList(reference);
            return func;
        }

        public LuaTable GetTable(string fullPath, bool beLogMiss = true)
        {
            WeakReference weak = null;

            if (funcMap.TryGetValue(fullPath, out weak))
            {
                if (weak.IsAlive)
                {
                    LuaTable table = weak.Target as LuaTable;
                    CheckNull(table, "{0} not a lua table", fullPath);

                    if (table.IsAlive)
                    {
                        table.AddRef();
                        RemoveFromGCList(table.GetReference());
                        return table;
                    }
                }

                funcMap.Remove(fullPath);
            }

            if (PushLuaTable(fullPath, false))
            {
                int reference = ToLuaRef();
                LuaTable table = null;

                if (funcRefMap.TryGetValue(reference, out weak))
                {
                    if (weak.IsAlive)
                    {
                        table = weak.Target as LuaTable;
                        CheckNull(table, "{0} not a lua table", fullPath);

                        if (table.IsAlive)
                        {
                            funcMap.Add(fullPath, weak);
                            table.AddRef();
                            RemoveFromGCList(reference);
                            return table;
                        }
                    }

                    funcRefMap.Remove(reference);
                }

                table = new LuaTable(reference, this);
                table.name = fullPath;
                funcMap.Add(fullPath, new WeakReference(table));
                funcRefMap.Add(reference, new WeakReference(table));
                if (LogGC) Debugger.Log("Alloc LuaTable name {0}, id {1}", fullPath, reference);     
                RemoveFromGCList(reference);
                return table;
            }

            if (beLogMiss)
            {
                Debugger.LogWarning("Lua table {0} not exists", fullPath);
            }

            return null;
        }

        public LuaTable GetTable(int reference)
        {
            LuaTable table = TryGetLuaRef(reference) as LuaTable;

            if (table == null)
            {                
                table = new LuaTable(reference, this);
                funcRefMap.Add(reference, new WeakReference(table));
            }

            RemoveFromGCList(reference);
            return table;
        }

        public LuaThread GetLuaThread(int reference)
        {
            LuaThread thread = TryGetLuaRef(reference) as LuaThread;

            if (thread == null)
            {                
                thread = new LuaThread(reference, this);
                funcRefMap.Add(reference, new WeakReference(thread));
            }

            RemoveFromGCList(reference);
            return thread;
        }

        public LuaDelegate GetLuaDelegate(LuaFunction func)
        {
            WeakReference weak = null;
            int reference = func.GetReference();            
            delegateMap.TryGetValue(reference, out weak);

            if (weak != null)
            {
                if (weak.IsAlive)
                {
                    return weak.Target as LuaDelegate;
                }

                delegateMap.Remove(reference);
            }

            return null;
        }

        public LuaDelegate GetLuaDelegate(LuaFunction func, LuaTable self)
        {
            WeakReference weak = null;
            long high = func.GetReference();
            long low = self == null ? 0 : self.GetReference();
            low = low >= 0 ? low : 0;
            long key = high << 32 | low;            
            delegateMap.TryGetValue(key, out weak);

            if (weak != null)
            {
                if (weak.IsAlive)
                {
                    return weak.Target as LuaDelegate;
                }

                delegateMap.Remove(key);
            }

            return null;
        }

        public void AddLuaDelegate(LuaDelegate target, LuaFunction func)
        {            
            int key = func.GetReference();

            if (key > 0)
            {
                delegateMap[key] = new WeakReference(target);
            }
        }

        public void AddLuaDelegate(LuaDelegate target, LuaFunction func, LuaTable self)
        {
            long high = func.GetReference();
            long low = self == null ? 0 : self.GetReference();
            low = low >= 0 ? low : 0;
            long key = high << 32 | low;

            if (key > 0)
            {
                delegateMap[key] = new WeakReference(target);
            }
        }

        public bool CheckTop()
        {
            int n = LuaGetTop();

            if (n != 0)
            {
                Debugger.LogWarning("Lua stack top is {0}", n);
                return false;
            }

            return true;
        }

        public void Push(bool b)
        {
            LuaDLL.lua_pushboolean(L, b);
        }

        public void Push(double d)
        {
            LuaDLL.lua_pushnumber(L, d);
        }

        public void Push(uint un)
        {
            LuaDLL.lua_pushnumber(L, un);
        }

        public void Push(int n)
        {
            LuaDLL.lua_pushinteger(L, n);
        }

        public void Push(short s)
        {
            LuaDLL.lua_pushnumber(L, s);
        }

        public void Push(ushort us)
        {
            LuaDLL.lua_pushnumber(L, us);
        }

        public void Push(long l)
        {
            LuaDLL.tolua_pushint64(L, l);
        }

        public void Push(ulong ul)
        {
            LuaDLL.tolua_pushuint64(L, ul);
        }

        public void Push(string str)
        {
            LuaDLL.lua_pushstring(L, str);
        }

        public void Push(IntPtr p)
        {
            LuaDLL.lua_pushlightuserdata(L, p);
        }

        public void Push(Vector3 v3)
        {            
            LuaDLL.tolua_pushvec3(L, v3.x, v3.y, v3.z);
        }

        public void Push(Vector2 v2)
        {
            LuaDLL.tolua_pushvec2(L, v2.x, v2.y);
        }

        public void Push(Vector4 v4)
        {
            LuaDLL.tolua_pushvec4(L, v4.x, v4.y, v4.z, v4.w);
        }

        public void Push(Color clr)
        {
            LuaDLL.tolua_pushclr(L, clr.r, clr.g, clr.b, clr.a);
        }

        public void Push(Quaternion q)
        {
            LuaDLL.tolua_pushquat(L, q.x, q.y, q.z, q.w);
        }          

        public void Push(Ray ray)
        {
            ToLua.Push(L, ray);
        }

        public void Push(Bounds bound)
        {
            ToLua.Push(L, bound);
        }

        public void Push(RaycastHit hit)
        {
            ToLua.Push(L, hit);
        }

        public void Push(Touch touch)
        {
            ToLua.Push(L, touch);
        }

        public void PushLayerMask(LayerMask mask)
        {
            LuaDLL.tolua_pushlayermask(L, mask.value);
        }

        public void Push(LuaByteBuffer bb)
        {
            LuaDLL.lua_pushlstring(L, bb.buffer, bb.Length);
        }

        public void PushByteBuffer(byte[] buffer)
        {
            LuaDLL.lua_pushlstring(L, buffer, buffer.Length);
        }

        public void PushByteBuffer(byte[] buffer, int len)
        {
            LuaDLL.lua_pushlstring(L, buffer, len);
        }

        public void Push(LuaBaseRef lbr)
        {
            if (lbr == null)
            {                
                LuaPushNil();
            }
            else
            {
                LuaGetRef(lbr.GetReference());
            }
        }

        void PushUserData(object o, int reference)
        {
            int index;

            if (translator.Getudata(o, out index))
            {
                if (LuaDLL.tolua_pushudata(L, index))
                {
                    return;
                }

                translator.Destroyudata(index);
            }

            index = translator.AddObject(o);
            LuaDLL.tolua_pushnewudata(L, reference, index);
        }

        public void Push(Array array)
        {
            if (array == null)
            {                
                LuaPushNil();
            }
            else
            {
                PushUserData(array, ArrayMetatable);
            }
        }

        public void Push(Type t)
        {
            if (t == null)
            {
                LuaPushNil();
            }
            else
            {
                PushUserData(t, TypeMetatable);
            }
        }

        public void Push(Delegate ev)
        {
            if (ev == null)
            {                
                LuaPushNil();
            }
            else
            {
                PushUserData(ev, DelegateMetatable);
            }
        }

        public object GetEnumObj(Enum e)
        {
            object o = null;

            if (!enumMap.TryGetValue(e, out o))
            {
                o = e;
                enumMap.Add(e, o);
            }

            return o;
        }

        public void Push(Enum e)
        {
            if (e == null)
            {                
                LuaPushNil();
            }
            else
            {
                object o = GetEnumObj(e);
                PushUserData(o, EnumMetatable);
            }
        }

        public void Push(IEnumerator iter)
        {
            ToLua.Push(L, iter);
        }

        public void Push(UnityEngine.Object obj)
        {
            ToLua.Push(L, obj);
        }

        public void Push(UnityEngine.TrackedReference tracker)
        {
            ToLua.Push(L, tracker);
        }

        public void PushVariant(object obj)
        {
            ToLua.Push(L, obj);
        }

        public void PushObject(object obj)
        {
            ToLua.PushObject(L, obj);
        }

        public void PushSealed<T>(T o)
        {
            ToLua.PushSealed<T>(L, o);
        }

        public void PushValue<T>(T v) where T : struct
        {
            StackTraits<T>.Push(L, v);
        }

        public void PushGeneric<T>(T o)
        {
            StackTraits<T>.Push(L, o);
        }

        Vector3 ToVector3(int stackPos)
        {            
            float x, y, z;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getvec3(L, stackPos, out x, out y, out z);
            return new Vector3(x, y, z);
        }

        public Vector3 CheckVector3(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Vector3)
            {
                LuaTypeError(stackPos, "Vector3",  LuaValueTypeName.Get(type));
                return Vector3.zero;
            }
            
            float x, y, z;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getvec3(L, stackPos, out x, out y, out z);
            return new Vector3(x, y, z);
        }

        public Quaternion CheckQuaternion(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Quaternion)
            {
                LuaTypeError(stackPos, "Quaternion", LuaValueTypeName.Get(type));
                return Quaternion.identity;
            }

            float x, y, z, w;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getquat(L, stackPos, out x, out y, out z, out w);
            return new Quaternion(x, y, z, w);
        }

        public Vector2 CheckVector2(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Vector2)
            {
                LuaTypeError(stackPos, "Vector2", LuaValueTypeName.Get(type));                
                return Vector2.zero;
            }

            float x, y;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getvec2(L, stackPos, out x, out y);
            return new Vector2(x, y);
        }

        public Vector4 CheckVector4(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Vector4)
            {
                LuaTypeError(stackPos, "Vector4", LuaValueTypeName.Get(type));
                return Vector4.zero;
            }

            float x, y, z, w;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getvec4(L, stackPos, out x, out y, out z, out w);
            return new Vector4(x, y, z, w);
        }

        public Color CheckColor(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Color)
            {
                LuaTypeError(stackPos, "Color", LuaValueTypeName.Get(type));    
                return Color.black;
            }

            float r, g, b, a;
            stackPos = LuaDLL.abs_index(L, stackPos);
            LuaDLL.tolua_getclr(L, stackPos, out r, out g, out b, out a);
            return new Color(r, g, b, a);
        }

        public Ray CheckRay(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Ray)
            {
                LuaTypeError(stackPos, "Ray", LuaValueTypeName.Get(type));
                return new Ray();
            }

            stackPos = LuaDLL.abs_index(L, stackPos);
            int oldTop = BeginPCall(UnpackRay);
            LuaPushValue(stackPos);

            try
            {
                PCall(1, oldTop);
                Vector3 origin = ToVector3(oldTop + 1);
                Vector3 dir = ToVector3(oldTop + 2);
                EndPCall(oldTop);                
                return new Ray(origin, dir);
            }
            catch(Exception e)
            {
                EndPCall(oldTop);
                throw e;
            }
        }

        public Bounds CheckBounds(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.Bounds)
            {
                LuaTypeError(stackPos, "Bounds", LuaValueTypeName.Get(type));    
                return new Bounds();
            }

            stackPos = LuaDLL.abs_index(L, stackPos);
            int oldTop = BeginPCall(UnpackBounds);
            LuaPushValue(stackPos);

            try
            {
                PCall(1, oldTop);
                Vector3 center = ToVector3(oldTop + 1);
                Vector3 size = ToVector3(oldTop + 2);
                EndPCall(oldTop);
                return new Bounds(center, size);
            }
            catch(Exception e)
            {
                EndPCall(oldTop);
                throw e;
            }
        }

        public LayerMask CheckLayerMask(int stackPos)
        {            
            int type = LuaDLL.tolua_getvaluetype(L, stackPos);

            if (type != LuaValueType.LayerMask)
            {
                LuaTypeError(stackPos, "LayerMask", LuaValueTypeName.Get(type));
                return 0;
            }

            stackPos = LuaDLL.abs_index(L, stackPos);
            return LuaDLL.tolua_getlayermask(L, stackPos);
        }

        public long CheckLong(int stackPos)
        {
            stackPos = LuaDLL.abs_index(L, stackPos);
            return LuaDLL.tolua_checkint64(L, stackPos);
        }

        public ulong CheckULong(int stackPos)
        {
            stackPos = LuaDLL.abs_index(L, stackPos);
            return LuaDLL.tolua_checkuint64(L, stackPos);
        }

        public string CheckString(int stackPos)
        {
            return ToLua.CheckString(L, stackPos);
        }

        public Delegate CheckDelegate(int stackPos)
        {            
            int udata = LuaDLL.tolua_rawnetobj(L, stackPos);

            if (udata != -1)
            {
                object obj = translator.GetObject(udata);

                if (obj != null)
                {                                                  
                    if (obj is Delegate)
                    {
                        return (Delegate)obj;
                    }

                    LuaTypeError(stackPos, "Delegate", obj.GetType().FullName);
                }

                return null;
            }
            else if (LuaDLL.lua_isnil(L,stackPos))
            {
                return null;
            }

            LuaTypeError(stackPos, "Delegate");
            return null;
        }

        public char[] CheckCharBuffer(int stackPos)
        {
            return ToLua.CheckCharBuffer(L, stackPos);
        }

        public byte[] CheckByteBuffer(int stackPos)
        {
            return ToLua.CheckByteBuffer(L, stackPos);
        }

        public T[] CheckNumberArray<T>(int stackPos) where T : struct
        {
            return ToLua.CheckNumberArray<T>(L, stackPos);
        }

        public object CheckObject(int stackPos, Type type)
        {
            return ToLua.CheckObject(L, stackPos, type);
        }

        public object CheckVarObject(int stackPos, Type type)
        {
            return ToLua.CheckVarObject(L, stackPos, type);
        }

        public object[] CheckObjects(int oldTop)
        {
            int newTop = LuaGetTop();

            if (oldTop == newTop)
            {
                return null;
            }
            else
            {
                List<object> returnValues = new List<object>();

                for (int i = oldTop + 1; i <= newTop; i++)
                {
                    returnValues.Add(ToVariant(i));
                }

                return returnValues.ToArray();
            }
        }

        public LuaFunction CheckLuaFunction(int stackPos)
        {            
            return ToLua.CheckLuaFunction(L, stackPos);
        }

        public LuaTable CheckLuaTable(int stackPos)
        {            
            return ToLua.CheckLuaTable(L, stackPos);
        }

        public LuaThread CheckLuaThread(int stackPos)
        {            
            return ToLua.CheckLuaThread(L, stackPos);
        }

        //从堆栈读取一个值类型
        public T CheckValue<T>(int stackPos)
        {            
            return StackTraits<T>.Check(L, stackPos);
        }

        public object ToVariant(int stackPos)
        {
            return ToLua.ToVarObject(L, stackPos);
        }

        public void CollectRef(int reference, string name, bool isGCThread = false)
        {
            if (!isGCThread)
            {                
                Collect(reference, name, false);
            }
            else
            {
                lock (gcList)
                {
                    gcList.Add(new GCRef(reference, name));
                }
            }
        }

        //在委托调用中减掉一个LuaFunction, 此lua函数在委托中还会执行一次, 所以必须延迟删除，委托值类型表现之一
        public void DelayDispose(LuaBaseRef br)
        {
            if (br != null)
            {
                subList.Add(br);
            }
        }

        public int Collect()
        {
            int count = gcList.Count;

            if (count > 0)
            {
                lock (gcList)
                {
                    for (int i = 0; i < gcList.Count; i++)
                    {
                        int reference = gcList[i].reference;
                        string name = gcList[i].name;
                        Collect(reference, name, true);
                    }

                    gcList.Clear();
                    return count;
                }
            }

            for (int i = 0; i < subList.Count; i++)
            {
                subList[i].Dispose();
            }

            subList.Clear();
            translator.Collect();
            return 0;
        }

        public void RefreshDelegateMap()
        {
            List<long> list = new List<long>();
            var iter = delegateMap.GetEnumerator();

            while (iter.MoveNext())
            {
                if (!iter.Current.Value.IsAlive)
                {
                    list.Add(iter.Current.Key);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                delegateMap.Remove(list[i]);
            }
        }

        public object this[string fullPath]
        {
            get
            {
                int oldTop = LuaGetTop();
                int pos = fullPath.LastIndexOf('.');
                object obj = null;

                if (pos > 0)
                {
                    string tableName = fullPath.Substring(0, pos);

                    if (PushLuaTable(tableName))
                    {
                        string name = fullPath.Substring(pos + 1);
                        LuaPushString(name);
                        LuaRawGet(-2);
                        obj = ToVariant(-1);
                    }    
                    else
                    {
                        LuaSetTop(oldTop);
                        return null;
                    }
                }
                else
                {
                    LuaGetGlobal(fullPath);
                    obj = ToVariant(-1);
                }

                LuaSetTop(oldTop);
                return obj;
            }

            set
            {
                int oldTop = LuaGetTop();
                int pos = fullPath.LastIndexOf('.');

                if (pos > 0)
                {
                    string tableName = fullPath.Substring(0, pos);
                    IntPtr p = LuaFindTable(LuaIndexes.LUA_GLOBALSINDEX, tableName);

                    if (p == IntPtr.Zero)
                    {
                        string name = fullPath.Substring(pos + 1);
                        LuaPushString(name);
                        PushVariant(value);
                        LuaSetTable(-3);
                    }
                    else
                    {
                        LuaSetTop(oldTop);
                        int len = LuaDLL.tolua_strlen(p);
                        string str = LuaDLL.lua_ptrtostring(p, len);
                        throw new LuaException(string.Format("{0} not a Lua table", str));
                    }
                }
                else
                {
                    PushVariant(value);
                    LuaSetGlobal(fullPath);                    
                }

                LuaSetTop(oldTop);
            }
        }

        //慎用
        public void ReLoad(string moduleFileName)
        {
            LuaGetGlobal("package");
            LuaGetField(-1, "loaded");
            LuaPushString(moduleFileName);
            LuaGetTable(-2);                          

            if (!LuaIsNil(-1))
            {
                LuaPushString(moduleFileName);                        
                LuaPushNil();
                LuaSetTable(-4);                      
            }

            LuaPop(3);
            string require = string.Format("require '{0}'", moduleFileName);
            DoString(require, "ReLoad");
        }

        public int GetMetaReference(Type t)
        {
            int reference = -1;
            metaMap.TryGetValue(t, out reference);
            return reference;
        }

        public int GetMissMetaReference(Type t)
        {       
            int reference = -1;
            Type type = GetBaseType(t);

            while (type != null)
            {
                if (metaMap.TryGetValue(type, out reference))
                {
#if MISS_WARNING
                    if (!missSet.Contains(t))
                    {
                        missSet.Add(t);
                        Debugger.LogWarning("Type {0} not wrap to lua, push as {1}, the warning is only raised once", LuaMisc.GetTypeName(t), LuaMisc.GetTypeName(type));
                    }
#endif                    
                    return reference;              
                }

                type = GetBaseType(type);
            }            

            if (reference <= 0)
            {
                type = typeof(object);
                reference = LuaStatic.GetMetaReference(L, type);                
            }

#if MISS_WARNING
            if (!missSet.Contains(t))
            {
                missSet.Add(t);
                Debugger.LogWarning("Type {0} not wrap to lua, push as {1}, the warning is only raised once", LuaMisc.GetTypeName(t), LuaMisc.GetTypeName(type));
            }            
#endif

            return reference;
        }

        Type GetBaseType(Type t)
        {
            if (t.IsGenericType)
            {
                return GetSpecialGenericType(t);
            }

            return LuaMisc.GetExportBaseType(t);
        }

        Type GetSpecialGenericType(Type t)
        {
            Type generic = t.GetGenericTypeDefinition();

            if (genericSet.Contains(generic))
            {
                return t == generic ? t.BaseType : generic;
            }

            return t.BaseType;
        }

        void CloseBaseRef()
        {
            LuaUnRef(PackBounds);
            LuaUnRef(UnpackBounds);
            LuaUnRef(PackRay);
            LuaUnRef(UnpackRay);
            LuaUnRef(PackRaycastHit);
            LuaUnRef(PackTouch);   
        }
        
        public void Dispose()
        {
            if (IntPtr.Zero != L)
            {
                Collect();

                foreach (KeyValuePair<Type, int> kv in metaMap)
                {
                    LuaUnRef(kv.Value);
                }

                List<LuaBaseRef> list = new List<LuaBaseRef>();

                foreach (KeyValuePair<int, WeakReference> kv in funcRefMap)
                {
                    if (kv.Value.IsAlive)
                    {
                        list.Add((LuaBaseRef)kv.Value.Target);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Dispose(true);
                }

                CloseBaseRef();
                delegateMap.Clear();
                funcRefMap.Clear();
                funcMap.Clear();
                metaMap.Clear();                
                typeMap.Clear();
                enumMap.Clear();
                preLoadMap.Clear();
                genericSet.Clear();
                stateMap.Remove(L);
                LuaClose();
                translator.Dispose();
                translator = null;                    
#if MISS_WARNING
                missSet.Clear();
#endif
                OnDestroy();
                Debugger.Log("LuaState destroy");
            }

            if (mainState == this)
            {
                mainState = null;
            }

#if UNITY_EDITOR
            beStart = false;
#endif

            LuaFileUtils.Instance.Dispose();
            System.GC.SuppressFinalize(this);            
        }

        //public virtual void Dispose(bool dispose)
        //{
        //}

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);          
        }

        public override bool Equals(object o)
        {
            if (o == null) return L == IntPtr.Zero;
            LuaState state = o as LuaState;

            if (state == null || state.L != L)
            {
                return false;
            }

            return L != IntPtr.Zero;
        }

        public static bool operator == (LuaState a, LuaState b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            object l = a;
            object r = b;

            if (l == null && r != null)
            {
                return b.L == IntPtr.Zero;
            }

            if (l != null && r == null)
            {
                return a.L == IntPtr.Zero;
            }

            if (a.L != b.L)
            {
                return false;
            }

            return a.L != IntPtr.Zero;
        }

        public static bool operator != (LuaState a, LuaState b)
        {
            return !(a == b);
        }

        public void PrintTable(string name)
        {
            LuaTable table = GetTable(name);
            LuaDictTable dict = table.ToDictTable();
            table.Dispose();
            var iter2 = dict.GetEnumerator();

            while (iter2.MoveNext())
            {
                Debugger.Log("map item, k,v is {0}:{1}", iter2.Current.Key, iter2.Current.Value);
            }

            iter2.Dispose();
            dict.Dispose();
        }

        protected void Collect(int reference, string name, bool beThread)
        {
            if (beThread)
            {
                WeakReference weak = null;

                if (name != null)
                {
                    funcMap.TryGetValue(name, out weak);

                    if (weak != null && !weak.IsAlive)
                    {
                        funcMap.Remove(name);
                        weak = null;
                    }
                }
                
                funcRefMap.TryGetValue(reference, out weak);

                if (weak != null && !weak.IsAlive)
                {
                    ToLuaUnRef(reference);
                    funcRefMap.Remove(reference);
                    delegateMap.Remove(reference);

                    if (LogGC)
                    {
                        string str = name == null ? "null" : name;
                        Debugger.Log("collect lua reference name {0}, id {1} in thread", str, reference);
                    }
                }
            }
            else
            {
                if (name != null)
                {
                    WeakReference weak = null;
                    funcMap.TryGetValue(name, out weak);
                    
                    if (weak != null && weak.IsAlive)
                    {
                        LuaBaseRef lbr = (LuaBaseRef)weak.Target;

                        if (reference == lbr.GetReference())
                        {
                            funcMap.Remove(name);
                        }
                    }
                }

                ToLuaUnRef(reference);
                funcRefMap.Remove(reference);
                delegateMap.Remove(reference);

                if (LogGC)
                {
                    string str = name == null ? "null" : name;
                    Debugger.Log("collect lua reference name {0}, id {1} in main", str, reference);
                }
            }
        }

        protected void LuaLoadBuffer(byte[] buffer, string chunkName)
        {
            LuaDLL.tolua_pushtraceback(L);
            int oldTop = LuaGetTop();

            if (LuaLoadBuffer(buffer, buffer.Length, chunkName) == 0)
            {
                if (LuaPCall(0, LuaDLL.LUA_MULTRET, oldTop) == 0)
                {                    
                    LuaSetTop(oldTop - 1);
                    return;
                }
            }

            string err = LuaToString(-1);
            LuaSetTop(oldTop - 1);                        
            throw new LuaException(err, LuaException.GetLastError());
        }

        protected T LuaLoadBuffer<T>(byte[] buffer, string chunkName)
        {
            LuaDLL.tolua_pushtraceback(L);
            int oldTop = LuaGetTop();

            if (LuaLoadBuffer(buffer, buffer.Length, chunkName) == 0)
            {
                if (LuaPCall(0, LuaDLL.LUA_MULTRET, oldTop) == 0)
                {
                    T result = CheckValue<T>(oldTop + 1);
                    LuaSetTop(oldTop - 1);
                    return result;
                }
            }

            string err = LuaToString(-1);
            LuaSetTop(oldTop - 1);
            throw new LuaException(err, LuaException.GetLastError());
        }

        public bool BeginCall(string name, int top, bool beLogMiss)
        {            
            LuaDLL.tolua_pushtraceback(L);

            if (PushLuaFunction(name, false))
            {
                return true;
            }
            else
            {
                LuaDLL.lua_settop(L, top);
                if (beLogMiss)
                {
                    Debugger.Log("Lua function {0} not exists", name);
                }
                
                return false;
            }
        }

        public void Call(int nArgs, int errfunc, int top)
        {
            if (LuaDLL.lua_pcall(L, nArgs, LuaDLL.LUA_MULTRET, errfunc) != 0)
            {
                string error = LuaDLL.lua_tostring(L, -1);                
                throw new LuaException(error, LuaException.GetLastError());
            }
        }

        public void Call(string name, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {                
                if (BeginCall(name, top, beLogMiss))
                {
                    Call(0, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T>(string name, T arg1, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    Call(1, top + 1, top);                    
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T1, T2>(string name, T1 arg1, T2 arg2, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    Call(2, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T1, T2, T3>(string name, T1 arg1, T2 arg2, T3 arg3, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    Call(3, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T1, T2, T3, T4>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    Call(4, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T1, T2, T3, T4, T5>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    PushGeneric(arg5);
                    Call(5, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public void Call<T1, T2, T3, T4, T5, T6>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    PushGeneric(arg5);
                    PushGeneric(arg6);
                    Call(6, top + 1, top);
                    LuaDLL.lua_settop(L, top);
                }
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<R1>(string name, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    Call(0, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, R1>(string name, T1 arg1, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    Call(1, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, T2, R1>(string name, T1 arg1, T2 arg2, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    Call(2, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, T2, T3, R1>(string name, T1 arg1, T2 arg2, T3 arg3, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    Call(3, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, T2, T3, T4, R1>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    Call(4, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, T2, T3, T4, T5, R1>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    PushGeneric(arg5);
                    Call(5, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        public R1 Invoke<T1, T2, T3, T4, T5, T6, R1>(string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, bool beLogMiss)
        {
            int top = LuaDLL.lua_gettop(L);

            try
            {
                if (BeginCall(name, top, beLogMiss))
                {
                    PushGeneric(arg1);
                    PushGeneric(arg2);
                    PushGeneric(arg3);
                    PushGeneric(arg4);
                    PushGeneric(arg5);
                    PushGeneric(arg6);
                    Call(6, top + 1, top);
                    R1 ret1 = CheckValue<R1>(top + 2);
                    LuaDLL.lua_settop(L, top);
                    return ret1;
                }

                return default(R1);
            }
            catch (Exception e)
            {
                LuaDLL.lua_settop(L, top);
                throw e;
            }
        }

        void InitTypeTraits()
        {
            LuaMatchType _ck = new LuaMatchType();
            TypeTraits<sbyte>.Init(_ck.CheckNumber);
            TypeTraits<byte>.Init(_ck.CheckNumber);
            TypeTraits<short>.Init(_ck.CheckNumber);
            TypeTraits<ushort>.Init(_ck.CheckNumber);
            TypeTraits<char>.Init(_ck.CheckNumber);
            TypeTraits<int>.Init(_ck.CheckNumber);
            TypeTraits<uint>.Init(_ck.CheckNumber);
            TypeTraits<decimal>.Init(_ck.CheckNumber);
            TypeTraits<float>.Init(_ck.CheckNumber);
            TypeTraits<double>.Init(_ck.CheckNumber);
            TypeTraits<bool>.Init(_ck.CheckBool);
            TypeTraits<long>.Init(_ck.CheckLong);
            TypeTraits<ulong>.Init(_ck.CheckULong);
            TypeTraits<string>.Init(_ck.CheckString);

            TypeTraits<Nullable<sbyte>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<byte>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<short>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<ushort>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<char>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<int>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<uint>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<decimal>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<float>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<double>>.Init(_ck.CheckNullNumber);
            TypeTraits<Nullable<bool>>.Init(_ck.CheckNullBool);
            TypeTraits<Nullable<long>>.Init(_ck.CheckNullLong);
            TypeTraits<Nullable<ulong>>.Init(_ck.CheckNullULong);

            TypeTraits<byte[]>.Init(_ck.CheckByteArray);            
            TypeTraits<char[]>.Init(_ck.CheckCharArray);            
            TypeTraits<bool[]>.Init(_ck.CheckBoolArray);            
            TypeTraits<sbyte[]>.Init(_ck.CheckSByteArray);
            TypeTraits<short[]>.Init(_ck.CheckInt16Array);
            TypeTraits<ushort[]>.Init(_ck.CheckUInt16Array);
            TypeTraits<decimal[]>.Init(_ck.CheckDecimalArray);
            TypeTraits<float[]>.Init(_ck.CheckSingleArray);
            TypeTraits<double[]>.Init(_ck.CheckDoubleArray);
            TypeTraits<int[]>.Init(_ck.CheckInt32Array);
            TypeTraits<uint[]>.Init(_ck.CheckUInt32Array);
            TypeTraits<long[]>.Init(_ck.CheckInt64Array);
            TypeTraits<ulong[]>.Init(_ck.CheckUInt64Array);
            TypeTraits<string[]>.Init(_ck.CheckStringArray);

            TypeTraits<Vector3>.Init(_ck.CheckVec3);
            TypeTraits<Quaternion>.Init(_ck.CheckQuat);
            TypeTraits<Vector2>.Init(_ck.CheckVec2);
            TypeTraits<Color>.Init(_ck.CheckColor);
            TypeTraits<Vector4>.Init(_ck.CheckVec4);
            TypeTraits<Ray>.Init(_ck.CheckRay);
            TypeTraits<Bounds>.Init(_ck.CheckBounds);
            TypeTraits<Touch>.Init(_ck.CheckTouch);
            TypeTraits<LayerMask>.Init(_ck.CheckLayerMask);
            TypeTraits<RaycastHit>.Init(_ck.CheckRaycastHit);

            TypeTraits<Nullable<Vector3>>.Init(_ck.CheckNullVec3);
            TypeTraits<Nullable<Quaternion>>.Init(_ck.CheckNullQuat);
            TypeTraits<Nullable<Vector2>>.Init(_ck.CheckNullVec2);
            TypeTraits<Nullable<Color>>.Init(_ck.CheckNullColor);
            TypeTraits<Nullable<Vector4>>.Init(_ck.CheckNullVec4);
            TypeTraits<Nullable<Ray>>.Init(_ck.CheckNullRay);
            TypeTraits<Nullable<Bounds>>.Init(_ck.CheckNullBounds);
            TypeTraits<Nullable<Touch>>.Init(_ck.CheckNullTouch);
            TypeTraits<Nullable<LayerMask>>.Init(_ck.CheckNullLayerMask);
            TypeTraits<Nullable<RaycastHit>>.Init(_ck.CheckNullRaycastHit);

            TypeTraits<Vector3[]>.Init(_ck.CheckVec3Array);
            TypeTraits<Quaternion[]>.Init(_ck.CheckQuatArray);
            TypeTraits<Vector2[]>.Init(_ck.CheckVec2Array);
            TypeTraits<Color[]>.Init(_ck.CheckColorArray);
            TypeTraits<Vector4[]>.Init(_ck.CheckVec4Array);

            TypeTraits<IntPtr>.Init(_ck.CheckPtr);
            TypeTraits<UIntPtr>.Init(_ck.CheckPtr);
            TypeTraits<LuaFunction>.Init(_ck.CheckLuaFunc);
            TypeTraits<LuaTable>.Init(_ck.CheckLuaTable);
            TypeTraits<LuaThread>.Init(_ck.CheckLuaThread);
            TypeTraits<LuaBaseRef>.Init(_ck.CheckLuaBaseRef);

            TypeTraits<LuaByteBuffer>.Init(_ck.CheckByteBuffer);
            TypeTraits<EventObject>.Init(_ck.CheckEventObject);
            TypeTraits<IEnumerator>.Init(_ck.CheckEnumerator);
            TypeTraits<Type>.Init(_ck.CheckMonoType);
            TypeTraits<GameObject>.Init(_ck.CheckGameObject);
            TypeTraits<Transform>.Init(_ck.CheckTransform);
            TypeTraits<Type[]>.Init(_ck.CheckTypeArray);
            TypeTraits<object>.Init(_ck.CheckVariant);
            TypeTraits<object[]>.Init(_ck.CheckObjectArray);
        }

        void InitStackTraits()
        {
            LuaStackOp op = new LuaStackOp();
            StackTraits<sbyte>.Init(op.Push, op.CheckSByte, op.ToSByte);
            StackTraits<byte>.Init(op.Push, op.CheckByte, op.ToByte);
            StackTraits<short>.Init(op.Push, op.CheckInt16, op.ToInt16);
            StackTraits<ushort>.Init(op.Push, op.CheckUInt16, op.ToUInt16);
            StackTraits<char>.Init(op.Push, op.CheckChar, op.ToChar);
            StackTraits<int>.Init(op.Push, op.CheckInt32, op.ToInt32);
            StackTraits<uint>.Init(op.Push, op.CheckUInt32, op.ToUInt32);
            StackTraits<decimal>.Init(op.Push, op.CheckDecimal, op.ToDecimal);
            StackTraits<float>.Init(op.Push, op.CheckFloat, op.ToFloat);
            StackTraits<double>.Init(LuaDLL.lua_pushnumber, LuaDLL.luaL_checknumber, LuaDLL.lua_tonumber);
            StackTraits<bool>.Init(LuaDLL.lua_pushboolean, LuaDLL.luaL_checkboolean, LuaDLL.lua_toboolean);
            StackTraits<long>.Init(LuaDLL.tolua_pushint64, LuaDLL.tolua_checkint64, LuaDLL.tolua_toint64);
            StackTraits<ulong>.Init(LuaDLL.tolua_pushuint64, LuaDLL.tolua_checkuint64, LuaDLL.tolua_touint64);
            StackTraits<string>.Init(LuaDLL.lua_pushstring, ToLua.CheckString, ToLua.ToString);

            StackTraits<Nullable<sbyte>>.Init(op.Push, op.CheckNullSByte, op.ToNullSByte);
            StackTraits<Nullable<byte>>.Init(op.Push, op.CheckNullByte, op.ToNullByte);
            StackTraits<Nullable<short>>.Init(op.Push, op.CheckNullInt16, op.ToNullInt16);
            StackTraits<Nullable<ushort>>.Init(op.Push, op.CheckNullUInt16, op.ToNullUInt16);
            StackTraits<Nullable<char>>.Init(op.Push, op.CheckNullChar, op.ToNullChar);
            StackTraits<Nullable<int>>.Init(op.Push, op.CheckNullInt32, op.ToNullInt32);
            StackTraits<Nullable<uint>>.Init(op.Push, op.CheckNullUInt32, op.ToNullUInt32);
            StackTraits<Nullable<decimal>>.Init(op.Push, op.CheckNullDecimal, op.ToNullDecimal);
            StackTraits<Nullable<float>>.Init(op.Push, op.CheckNullFloat, op.ToNullFloat);
            StackTraits<Nullable<double>>.Init(op.Push, op.CheckNullNumber, op.ToNullNumber);
            StackTraits<Nullable<bool>>.Init(op.Push, op.CheckNullBool, op.ToNullBool);
            StackTraits<Nullable<long>>.Init(op.Push, op.CheckNullInt64, op.ToNullInt64);
            StackTraits<Nullable<ulong>>.Init(op.Push, op.CheckNullUInt64, op.ToNullUInt64);

            StackTraits<byte[]>.Init(ToLua.Push, ToLua.CheckByteBuffer, ToLua.ToByteBuffer);
            StackTraits<char[]>.Init(ToLua.Push, ToLua.CheckCharBuffer, ToLua.ToCharBuffer);
            StackTraits<bool[]>.Init(ToLua.Push, ToLua.CheckBoolArray, ToLua.ToBoolArray);
            StackTraits<sbyte[]>.Init(ToLua.Push, op.CheckSByteArray, op.ToSByteArray);
            StackTraits<short[]>.Init(ToLua.Push, op.CheckInt16Array, op.ToInt16Array);
            StackTraits<ushort[]>.Init(ToLua.Push, op.CheckUInt16Array, op.ToUInt16Array);
            StackTraits<decimal[]>.Init(ToLua.Push, op.CheckDecimalArray, op.ToDecimalArray);
            StackTraits<float[]>.Init(ToLua.Push, op.CheckFloatArray, op.ToFloatArray);
            StackTraits<double[]>.Init(ToLua.Push, op.CheckDoubleArray, op.ToDoubleArray);
            StackTraits<int[]>.Init(ToLua.Push, op.CheckInt32Array, op.ToInt32Array);
            StackTraits<uint[]>.Init(ToLua.Push, op.CheckUInt32Array, op.ToUInt32Array);
            StackTraits<long[]>.Init(ToLua.Push, op.CheckInt64Array, op.ToInt64Array);
            StackTraits<ulong[]>.Init(ToLua.Push, op.CheckUInt64Array, op.ToUInt64Array);
            StackTraits<string[]>.Init(ToLua.Push, ToLua.CheckStringArray, ToLua.ToStringArray);

            StackTraits<Vector3>.Init(ToLua.Push, ToLua.CheckVector3, ToLua.ToVector3);
            StackTraits<Quaternion>.Init(ToLua.Push, ToLua.CheckQuaternion, ToLua.ToQuaternion);
            StackTraits<Vector2>.Init(ToLua.Push, ToLua.CheckVector2, ToLua.ToVector2);
            StackTraits<Color>.Init(ToLua.Push, ToLua.CheckColor, ToLua.ToColor);
            StackTraits<Vector4>.Init(ToLua.Push, ToLua.CheckVector4, ToLua.ToVector4);
            StackTraits<Ray>.Init(ToLua.Push, ToLua.CheckRay, ToLua.ToRay);
            StackTraits<Touch>.Init(ToLua.Push, null, null);
            StackTraits<Bounds>.Init(ToLua.Push, ToLua.CheckBounds, ToLua.ToBounds);
            StackTraits<LayerMask>.Init(ToLua.PushLayerMask, ToLua.CheckLayerMask, ToLua.ToLayerMask);
            StackTraits<RaycastHit>.Init(ToLua.Push, null, null);

            StackTraits<Nullable<Vector3>>.Init(op.Push, op.CheckNullVec3, op.ToNullVec3);
            StackTraits<Nullable<Quaternion>>.Init(op.Push, op.CheckNullQuat, op.ToNullQuat);
            StackTraits<Nullable<Vector2>>.Init(op.Push, op.CheckNullVec2, op.ToNullVec2);
            StackTraits<Nullable<Color>>.Init(op.Push, op.CheckNullColor, op.ToNullColor);
            StackTraits<Nullable<Vector4>>.Init(op.Push, op.CheckNullVec4, op.ToNullVec4);
            StackTraits<Nullable<Ray>>.Init(op.Push, op.CheckNullRay, op.ToNullRay);
            StackTraits<Nullable<Touch>>.Init(op.Push, null, null);
            StackTraits<Nullable<Bounds>>.Init(op.Push, op.CheckNullBounds, op.ToNullBounds);
            StackTraits<Nullable<LayerMask>>.Init(op.Push, op.CheckNullLayerMask, op.ToNullLayerMask);
            StackTraits<Nullable<RaycastHit>>.Init(op.Push, null, null);

            StackTraits<Vector3[]>.Init(ToLua.Push, op.CheckVec3Array, op.ToVec3Array);
            StackTraits<Quaternion[]>.Init(ToLua.Push, op.CheckQuatArray, op.ToQuatArray);
            StackTraits<Vector2[]>.Init(ToLua.Push, op.CheckVec2Array, op.ToVec2Array);
            StackTraits<Color[]>.Init(ToLua.Push, op.CheckColorArray, op.ToColorArray);
            StackTraits<Vector4[]>.Init(ToLua.Push, op.CheckVec4Array, op.ToVec4Array);

            StackTraits<UIntPtr>.Init(op.Push, op.CheckUIntPtr, op.CheckUIntPtr); //"NYI"
            StackTraits<IntPtr>.Init(LuaDLL.lua_pushlightuserdata, ToLua.CheckIntPtr, ToLua.CheckIntPtr);
            StackTraits<LuaFunction>.Init(ToLua.Push, ToLua.CheckLuaFunction, ToLua.ToLuaFunction);
            StackTraits<LuaTable>.Init(ToLua.Push, ToLua.CheckLuaTable, ToLua.ToLuaTable);
            StackTraits<LuaThread>.Init(ToLua.Push, ToLua.CheckLuaThread, ToLua.ToLuaThread);
            StackTraits<LuaBaseRef>.Init(ToLua.Push, ToLua.CheckLuaBaseRef, ToLua.CheckLuaBaseRef);

            StackTraits<LuaByteBuffer>.Init(ToLua.Push, op.CheckLuaByteBuffer, op.ToLuaByteBuffer);
            StackTraits<EventObject>.Init(ToLua.Push, op.CheckEventObject, op.ToEventObject);
            StackTraits<IEnumerator>.Init(ToLua.Push, ToLua.CheckIter, op.ToIter);
            StackTraits<Type>.Init(ToLua.Push, ToLua.CheckMonoType, op.ToType);
            StackTraits<Type[]>.Init(ToLua.Push, op.CheckTypeArray, op.ToTypeArray);
            StackTraits<GameObject>.Init(op.Push, op.CheckGameObject, op.ToGameObject);
            StackTraits<Transform>.Init(op.Push, op.CheckTransform, op.ToTransform);
            StackTraits<object>.Init(ToLua.Push, ToLua.ToVarObject, ToLua.ToVarObject);
            StackTraits<object[]>.Init(ToLua.Push, ToLua.CheckObjectArray, ToLua.ToObjectArray);

            StackTraits<nil>.Init(ToLua.Push, null, null);
        }
    }
}