## Lua版本的事件中心管理器  

### 介绍  
Lua版本的事件中心管理器，支持注册监听和派发事件，用于模块解耦、处理网络监听等，支持无参数派发事件和含参数派发事件。  
### 对外接口  
* Instance()：获取单例  
* RegisterEvent()：注册一个事件  
* UnRegisterEvent()：反注册一个事件  
* DispatchEvent()：派发事件  
* AddEventListener()：增加监听者  
* RemoveEventListener()：移除监听者  
### 使用方法
```lua
local function TestCallback_1()
    print("Callback_1")
end

local function TestCallback_2(param)
    print("Callback_2")
    print(param.id)
    print(param.pwd)
end

local EventMgr = EventMgr:Instance()
EventMgr:RegisterEvent(1, 1, TestCallback_1)
EventMgr:RegisterEvent(2, 1, TestCallback_2)
EventMgr:DispatchEvent(1, 1)
EventMgr:DispatchEvent(2, 1, { id = "abc", pwd = "123" })
```  
可以Main.lua中看到具体的使用代码  

### 博客教程  
>* [【Unity游戏开发】用C#和Lua实现Unity中的事件分发机制EventDispatcher](https://www.cnblogs.com/msxh/p/9539231.html)  

