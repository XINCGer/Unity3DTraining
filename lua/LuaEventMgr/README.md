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
