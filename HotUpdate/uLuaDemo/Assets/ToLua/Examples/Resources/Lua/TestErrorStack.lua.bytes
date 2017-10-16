function Show()
    error('this is error')                
end

function ShowStack(go)
    TestStack.Test1(go)                        
end      

function Instantiate(obj)
    local go = UnityEngine.Object.Instantiate(obj)
	print(go.name)
end

function TestRay(ray)                
    return Vector3.zero
end

function PushLuaError()
    TestStack.PushLuaError()      	
end

function Test3()
    TestStack.Test3()          
end

function Test4()
    TestStack.Test4()          
end

function Test5()
    TestStack.Test5()          
end

function SendMsgError(go)
	go:SendMessage("OnSendMsg");
end

function resume(co, ...)
    local r, msg = nil
    local func = function(...)
         r, msg = coroutine.resume(co, ...)
        
        if not r then
            print('xxxxxxxxxxxxxxxxxxxxxx')
            error(msg)
        end
    end

    pcall(func, ...)
    return r, msg
end

function Test6()
    print('--------------------------')                
    --TestStack.Test6()                
    local co = coroutine.create(function()  coroutine.yield() print('hahahahaha')  TestStack.Test6(go) end)                                                                                        
    coroutine.resume(co)                
    local r, msg = coroutine.resume(co)                
    print('go error')
    print(msg)
    print('--------------------------')            
end     

function Test8()
	TestArgError('123') 
end

_event =
{
	name = 'event'
}

_event.__index = function(t, k)
	return rawget(_event, k)
end

setmetatable(_event, _event)

function _event:Add(func, obj)
	print('xxxxxxxxxxxxxxxxxxxxxxxxxx')
end

_event.__call = function()

end

testev = {}            
setmetatable(testev, _event)

function TestCo(...)
	local name = TestTableInCo(...)
	print("table.name is: "..name)
end

function TestCoTable()
	local co = coroutine.create(TestCo)
	local r, msg = coroutine.resume(co, testev)

	if not r then
		print("TestCoTable: "..msg)
	end    
end