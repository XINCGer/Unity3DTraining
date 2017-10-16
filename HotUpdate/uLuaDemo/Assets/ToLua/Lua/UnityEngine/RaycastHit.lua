--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local rawget = rawget
local setmetatable = setmetatable

RaycastBits = 
{
	Collider = 1,
    Normal = 2,
    Point = 4,
    Rigidbody = 8,
    Transform = 16,
    ALL = 31,
}
	
local RaycastBits = RaycastBits
local RaycastHit = {}
local get = tolua.initget(RaycastHit)

RaycastHit.__index = function(t,k)
	local var = rawget(RaycastHit, k)
		
	if var == nil then							
		var = rawget(get, k)
		
		if var ~= nil then
			return var(t)	
		end
	end
	
	return var
end

--c# 创建
function RaycastHit.New(collider, distance, normal, point, rigidbody, transform)
	local hit = {collider = collider, distance = distance, normal = normal, point = point, rigidbody = rigidbody, transform = transform}
	setmetatable(hit, RaycastHit)
	return hit
end

function RaycastHit:Init(collider, distance, normal, point, rigidbody, transform)
	self.collider 	= collider
	self.distance 	= distance
	self.normal 	= normal
	self.point 		= point
	self.rigidbody 	= rigidbody
	self.transform 	= transform
end

function RaycastHit:Get()
	return self.collider, self.distance, self.normal, self.point, self.rigidbody, self.transform
end

function RaycastHit:Destroy()				
	self.collider 	= nil			
	self.rigidbody 	= nil					
	self.transform 	= nil		
end

function RaycastHit.GetMask(...)
	local arg = {...}
	local value = 0	

	for i = 1, #arg do		
		local n = RaycastBits[arg[i]] or 0
		
		if n ~= 0 then
			value = value + n				
		end
	end	
		
	if value == 0 then value = RaycastBits["all"] end
	return value
end

UnityEngine.RaycastHit = RaycastHit
setmetatable(RaycastHit, RaycastHit)
return RaycastHit