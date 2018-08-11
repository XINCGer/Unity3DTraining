require("Class")
local bit = require "bit"

EventMgr = {
    --实例对象
    _instance = nil,
    --观察值列表
    _listeners = nil
}
EventMgr.__index = EventMgr
setmetatable(EventMgr, Class)

-- 构造器
function EventMgr:new()
    local t = {}
    t = Class:new()
    setmetatable(t, EventMgr)
    return t
end

-- 获取单例接口
function EventMgr:Instance()
    if EventMgr._instance == nil then
        EventMgr._instance = EventMgr:new()
        EventMgr._listeners = {}
    end
    return EventMgr._instance
end

function EventMgr:RegisterEvent(moduleId, eventId, func)
    local key = bit.lshift(moduleId, 16) + eventId
    self:AddEventListener(key, func, nil)
end

function EventMgr:UnRegisterEvent(moduleId, eventId, func)
    local key = bit.lshift(moduleId, 16) + eventId
    self:RemoveEventListener(key, func)
end

function EventMgr:DispatchEvent(moduleId, eventId, param)
    local key = bit.lshift(moduleId, 16) + eventId
    local listeners = self._listeners[key]
    if nil == listeners then
        return
    end
    for _, v in ipairs(listeners) do
        if v.p then
            v.f(v.p, param)
        else
            v.f(param)
        end
    end
end

function EventMgr:AddEventListener(eventId, func, param)
    local listeners = self._listeners[eventId]
    -- 获取key对应的监听者列表，结构为{func,para}，如果没有就新建
    if listeners == nil then
        listeners = {}
        self._listeners[eventId] = listeners -- 保存监听者
    end
    --过滤掉已经注册过的消息，防止重复注册
    for _, v in pairs(listeners) do
        if (v and v.f == func) then
            return
        end
    end
    --if func == nil then
    --    print("func is nil!")
    --end
    --加入监听者的回调和参数
    table.insert(listeners, { f = func, p = param })
end

function EventMgr:RemoveEventListener(eventId, func)
    local listeners = self._listeners[eventId]
    if nil == listeners then
        return
    end
    for k, v in pairs(listeners) do
        if (v and v.f == func) then
            table.remove(listeners, k)
            return
        end
    end
end