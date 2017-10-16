--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local zero = Vector2.zero
local rawget = rawget
local setmetatable = setmetatable

TouchPhase =
{
	Began = 0,
	Moved = 1,
	Stationary = 2,
	Ended = 3,
	Canceled = 4,
}

TouchBits = 
{
	DeltaPosition = 1,
	Position = 2,
	RawPosition = 4,
	ALL = 7,
}

local TouchPhase = TouchPhase
local TouchBits = TouchBits
local Touch = {}
local get = tolua.initget(Touch)

Touch.__index = function(t,k)
	local var = rawget(Touch, k)
	
	if var == nil then							
		var = rawget(get, k)
		
		if var ~= nil then
			return var(t)	
		end
	end
	
	return var
end

--c# 创建
function Touch.New(fingerId, position, rawPosition, deltaPosition, deltaTime, tapCount, phase)	
	return setmetatable({fingerId = fingerId or 0, position = position or zero, rawPosition = rawPosition or zero, deltaPosition = deltaPosition or zero, deltaTime = deltaTime or 0, tapCount = tapCount or 0, phase = phase or 0}, Touch)	
end

function Touch:Init(fingerId, position, rawPosition, deltaPosition, deltaTime, tapCount, phase)
	self.fingerId = fingerId
	self.position = position
	self.rawPosition = rawPosition
	self.deltaPosition = deltaPosition
	self.deltaTime = deltaTime
	self.tapCount = tapCount
	self.phase = phase	
end

function Touch:Destroy()
	self.position 		= nil
	self.rawPosition	= nil
	self.deltaPosition 	= nil	
end

function Touch.GetMask(...)
	local arg = {...}
	local value = 0	

	for i = 1, #arg do		
		local n = TouchBits[arg[i]] or 0
		
		if n ~= 0 then
			value = value + n				
		end
	end	
		
	if value == 0 then value = TouchBits["all"] end
		
	return value
end

UnityEngine.TouchPhase = TouchPhase
UnityEngine.Touch = Touch
setmetatable(Touch, Touch)
return Touch


