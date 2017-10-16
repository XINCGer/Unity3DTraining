--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local setmetatable = setmetatable

local _slot = {}
setmetatable(_slot, _slot)	

_slot.__call = function(self, ...)			
	if nil == self.obj then
		return self.func(...)			
	else		
		return self.func(self.obj, ...)			
	end
end

_slot.__eq = function (lhs, rhs)
	return lhs.func == rhs.func and lhs.obj == rhs.obj
end

--可用于 Timer 定时器回调函数. 例如Timer.New(slot(self.func, self))
function slot(func, obj)	
	return setmetatable({func = func, obj = obj}, _slot)			
end