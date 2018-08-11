require("EventMgr")

local function TestCallback_1()
    print("Callback_1")
end

local function TestCallback_2()
    print("Callback_2")
end

local EventMgr = EventMgr:Instance()
EventMgr:RegisterEvent(1, 1, TestCallback_1)
EventMgr:RegisterEvent(2, 1, TestCallback_2)
EventMgr:DispatchEvent(1, 1)
EventMgr:DispatchEvent(2, 1)