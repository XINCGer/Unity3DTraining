require("Class")
local bit = require "bit"

EventMgr = {
    --实例对象
    _instance = nil
}
EventMgr.__index = EventMgr
setmetatable(EventMgr,Class)

-- 构造器
function EventMgr:new()
    local t = {}
    t = Class:new()
    setmetatable(t,EventMgr)
    return t
end

-- 获取单例接口
function EventMgr:Instance()
    if EventMgr._instance == nil then
        EventMgr._instance = EventMgr:new()
    end
    return EventMgr._instance
end

function EventMgr:RegisterEvent(moduleId,eventId,func)

end

function EventMgr:UnRegisterEvent(moduleId,eventId,func)

end

function EventMgr:DispatchEvent(moduleId, eventId, param)

end

function EventMgr:AddEventListener(eventId, func, param)

end

function EventMgr:RemoveEventListener(eventId, func)

end

function EventMgr:Test()
    print("Test")
end