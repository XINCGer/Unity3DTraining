require("EventMgr")

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