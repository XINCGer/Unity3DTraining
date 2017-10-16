## 例子1
展示了最小的tolua#环境，以及执行一段lua代码操作代码如下：
``` csharp
        LuaState lua = new LuaState();
        lua.Start();
        string hello =
            @"                
                print('hello tolua#')                                  
            ";
        
        lua.DoString(hello, "HelloWorld.cs");
        lua.CheckTop();
        lua.Dispose();
        lua = null;
``` 
LuaState封装了对lua 主要数据结构 lua_State 指针的各种堆栈操作。<br>
一般对于客户端，推荐只创建一个LuaState对象。如果要使用多State需要在Unity中设置全局宏 MULTI_STATE<br>
* LuaState.Start 需要在tolua代码加载到内存后调用。如果使用assetbunblde加载lua文件，调用Start()之前assetbundle必须加载好<br>
* LuaState.DoString 执行一段lua代码,除了例子,比较少用这种方式加载代码,无法避免代码重复加载覆盖等,需调用者自己保证。第二个参数用于调试信息,或者error消息(用于提示出错代码所在文件名称)<br>
* LuaState.CheckTop 检查是否堆栈是否平衡，一般放于update中，c#中任何使用lua堆栈操作，都需要调用者自己平衡堆栈（参考LuaFunction以及LuaTable代码）, 当CheckTop出现警告时其实早已经离开了堆栈操作范围，这是需自行review代码。<br>
* LuaState.Dispose 释放LuaState 以及其资源。<br>
> **注意:** 此例子无法发布到手机

## 例子2
展示了dofile跟require的区别, 代码如下:
``` csharp
    LuaState lua = null;

    void Start () 
    {      
        lua = new LuaState();                
        lua.Start();        
        //如果移动了ToLua目录，需要自己手动，这里只是例子就不做配置了
        string fullPath = Application.dataPath + "/ToLua/Examples/02_ScriptsFromFile";
        lua.AddSearchPath(fullPath);        
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 120, 45), "DoFile"))
        {
            lua.DoFile("ScriptsFromFile.lua");                        
        }
        else if (GUI.Button(new Rect(50, 150, 120, 45), "Require"))
        {            
            lua.Require("ScriptsFromFile");            
        }

        lua.Collect();
        lua.CheckTop();
    }

    void OnApplicationQuit()
    {
        lua.Dispose();
        lua = null;
    }
``` 
tolua#DoFile函数,跟lua保持一致行为,能多次执行一个文件。tolua#加入了新的Require函数,无论c#和lua谁先require一个lua文件, 都能保证加载唯一性<br>
* LuaState.AddSearchPath 增加搜索目录, 这样DoFile跟Require函数可以只用文件名,无需写全路径<br>
* LuaState.DoFile 加载一个lua文件, 注意dofile需要扩展名, 可反复执行, 后面的变量会覆盖之前的DoFile加载的变量<br>
* LuaState.Require 同lua require(modname)操作, 加载指定模块并且把结果写入到package.loaded中,如果modname存在, 则直接返回package.loaded[modname]<br>
* LuaState.Collect 垃圾回收, 对于被自动gc的LuaFunction, LuaTable, 以及委托减掉的LuaFunction, 延迟删除的Object之类。等等需要延迟处理的回收, 都在这里自动执行<br>

> **注意:** 虽然有文件加载,但此例子无法发布到手机, 如果ToLua目录不在/Assets目录下, 需要修改代码中的目录位置<br>

## 例子3 LuaFunction
展示了如何调用lua的函数, 主要代码如下:
``` csharp
    private string script =
        @"  function luaFunc(num)                        
                return num + 1
            end

            test = {}
            test.luaFunc = luaFunc
        ";

    LuaFunction luaFunc = null;
    LuaState lua = null;
	
    void Start () 
    {
        new LuaResLoader();
        lua = new LuaState();
        lua.Start();
        DelegateFactory.Init();
        lua.DoString(script);

        //Get the function object
        luaFunc = lua.GetFunction("test.luaFunc");

        if (func != null)
        {
            int num = luaFunc.Invoke<int, int>(123456);
            Debugger.Log("generic call return: {0}", num);

            num = CallFunc();
            Debugger.Log("expansion call return: {0}", num);

            Func<int, int> Func = luaFunc.ToDelegate<Func<int, int>>();
            num = Func(123456);
            Debugger.Log("Delegate call return: {0}", num);
            
            num = lua.Invoke<int, int>("test.luaFunc", 123456, true);
            Debugger.Log("luastate call return: {0}", num);
        }
                
        lua.CheckTop();
	}

    void OnDestroy()
    {
        if (luaFunc != null)
        {
            luaFunc.Dispose();
            luaFunc = null;
        }

        lua.Dispose();
        lua = null;
    }

    int CallFunc()
    {        
        luaFunc.BeginPCall();                
        luaFunc.Push(123456);
        luaFunc.PCall();        
        int num = (int)luaFunc.CheckNumber();                    
        luaFunc.EndPCall();
        return num;                
    }
``` 
tolua# 简化了lua函数的操作，通过LuaFunction封装(并缓存)一个lua函数，并提供各种操作, 建议频繁调用函数使用无GC方式。<br>
* LuaState.GetLuaFunction 获取并缓存一个lua函数, 此函数支持串式操作, 如"test.luaFunc"代表test表中的luaFunc函数。<br>
* LuaState.Invoke 临时调用一个lua function并返回一个值，这个操作并不缓存lua function，适合频率非常低的函数调用。<br>
* LuaFunction.Call() 不需要返回值的函数调用操作<br>
* LuaFunction.Invoke() 有一个返回值的函数调用操作 <br>
* LuaFunction.BeginPCall() 开始函数调用 <br>
* LuaFunction.Push() 压入函数调用需要的参数，通过众多的重载函数来解决参数转换gc问题 <br>
* LuaFunction.PCall() 调用lua函数 <br>
* LuaFunction.CheckNumber() 提取函数返回值, 并检查返回值为lua number类型 <br>
* LuaFunction.EndPCall() 结束lua函数调用, 清楚函数调用造成的堆栈变化 <br>
* LuaFunction.Dispose() 释放LuaFunction, 递减引用计数，如果引用计数为0, 则从_R表删除该函数 <br>

> **注意:** 无论Call还是PCall只相当于lua中的函数'.'调用。<br>
请注意':'这种语法糖 self:call(...) == self.call(self, ...） <br>
c# 中需要按后面方式调用, 即必须主动传入第一个参数self <br>

## 例子4
展示了如何访问lua中变量，table的操作
``` csharp
    private string script =
        @"
            print('Objs2Spawn is: '..Objs2Spawn)
            var2read = 42
            varTable = {1,2,3,4,5}
            varTable.default = 1
            varTable.map = {}
            varTable.map.name = 'map'
            
            meta = {name = 'meta'}
            setmetatable(varTable, meta)
            
            function TestFunc(strs)
                print('get func by variable')
            end
        ";

    void Start () 
    {
        new LuaResLoader();
        LuaState lua = new LuaState();
        lua.Start();
        lua["Objs2Spawn"] = 5;
        lua.DoString(script);

        //通过LuaState访问
        Debugger.Log("Read var from lua: {0}", lua["var2read"]);
        Debugger.Log("Read table var from lua: {0}", lua["varTable.default"]);  //LuaState 拆串式table

        LuaFunction func = lua["TestFunc"] as LuaFunction;
        func.Call();
        func.Dispose();

        //cache成LuaTable进行访问
        LuaTable table = lua.GetTable("varTable");
        Debugger.Log("Read varTable from lua, default: {0} name: {1}", table["default"], table["map.name"]);
        table["map.name"] = "new";  //table 字符串只能是key
        Debugger.Log("Modify varTable name: {0}", table["map.name"]);

        table.AddTable("newmap");
        LuaTable table1 = (LuaTable)table["newmap"];
        table1["name"] = "table1";
        Debugger.Log("varTable.newmap name: {0}", table1["name"]);
        table1.Dispose();

        table1 = table.GetMetaTable();

        if (table1 != null)
        {
            Debugger.Log("varTable metatable name: {0}", table1["name"]);
        }

        object[] list = table.ToArray();

        for (int i = 0; i < list.Length; i++)
        {
            Debugger.Log("varTable[{0}], is {1}", i, list[i]);
        }

        table.Dispose();                        
        lua.CheckTop();
        lua.Dispose();
    }
``` 
* luaState["Objs2Spawn"] LuaState通过重载this操作符，访问lua _G表中的变量Objs2Spawn <br>
* LuaState.GetTable 从lua中获取一个lua table, 可以串式访问比如lua.GetTable("varTable.map.name") 等于 varTable->map->name<br>
* LuaTable 支持this操作符，但此this不支持串式访问。比如table["map.name"] "map.name" 只是一个key，不是table->map->name <br>
* LuaTable.GetMetaTable() 可以获取当前table的metatable <br>
* LuaTable.ToArray() 获取数组表中的所有对象存入到object[]表中 <br>
* LuaTable.AddTable(name) 在当前的table表中添加一个名字为name的表 <br>
* LuaTable.GetTable(key) 获取t[key]值到c#, 类似于 lua_gettable <br>
* LuaTable.SetTable(key, value) 等价于t[k] = v的操作, 类似于lua_settable <br>
* LuaTable.RawGet(key) 获取t[key]值到c#, 类似于 lua_rawget <br>
* LuaTable.RawSet(key, value) 等价于t[k] = v的操作, 类似于lua_rawset <br>

## 例子5 协同一
展示了如何使用lua协同, lua 代码如下：
``` lua
        function fib(n)
            local a, b = 0, 1
            while n > 0 do
                a, b = b, a + b
                n = n - 1
            end

            return a
        end

        function CoFunc()
            print('Coroutine started')    
            for i = 1, 10, 1 do
                print(fib(i))                    
                coroutine.wait(0.1)                     
            end 
            print("current frameCount: "..Time.frameCount)
            coroutine.step()
            print("yield frameCount: "..Time.frameCount)

            local www = UnityEngine.WWW("http://www.baidu.com")
            coroutine.www(www)
            local s = tolua.tolstring(www.bytes)
            print(s:sub(1, 128))
            print('Coroutine ended')
        end

        function TestCortinue() 
            coroutine.start(CoFunc)
        end

        local coDelay = nil

        function Delay()
            local c = 1

            while true do
                coroutine.wait(1) 
                print("Count: "..c)
                c = c + 1
            end
        end

        function StartDelay()
            coDelay = coroutine.start(Delay)
        end

        function StopDelay()
            coroutine.stop(coDelay)
        end
```
c#代码如下:
``` csharp
        new LuaResLoader();
        lua  = new LuaState();
        lua.Start();
        LuaBinder.Bind(lua);
        DelegateFactory.Init();         
        looper = gameObject.AddComponent<LuaLooper>();
        looper.luaState = lua;

        lua.DoString(luaFile.text, "TestLuaCoroutine.lua");
        LuaFunction f = lua.GetFunction("TestCortinue");
        f.Call();
        f.Dispose();
        f = null;  
```         
* 必须启动LuaLooper驱动协同，这里将一个lua的半双工协同装换为类似unity的全双工协同 <br>
* fib函数负责计算一个斐那波契n  <br>
* coroutine.start 启动一个lua协同  <br>
* coroutine.wait 协同中等待一段时间，单位:秒  <br>
* coroutine.step 协同中等待一帧.  <br>
* coroutine.www 等待一个WWW完成. <br>
* tolua.tolstring 转换byte数组为lua字符串缓冲 <br>
* coroutine.stop 停止一个正在lua将要执行的协同 <br>