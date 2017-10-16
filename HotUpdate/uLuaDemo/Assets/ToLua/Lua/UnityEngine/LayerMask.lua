--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local Layer = Layer
local rawget = rawget
local setmetatable = setmetatable

local LayerMask = {}

LayerMask.__index = function(t,k)
	return rawget(LayerMask, k)	
end

LayerMask.__call = function(t,v)
	return setmetatable({value = value or 0}, LayerMask)
end

function LayerMask.New(value)	
	return setmetatable({value = value or 0}, LayerMask)		
end

function LayerMask:Get()
	return self.value
end

function LayerMask.NameToLayer(name)
	return Layer[name]
end

function LayerMask.GetMask(...)
	local arg = {...}
	local value = 0	

	for i = 1, #arg do		
		local n = LayerMask.NameToLayer(arg[i])
		
		if n ~= nil then
			value = value + 2 ^ n				
		end
	end	
		
	return value
end

UnityEngine.LayerMask = LayerMask
setmetatable(LayerMask, LayerMask)
return LayerMask



