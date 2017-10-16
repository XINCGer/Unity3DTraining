--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local rawget = rawget
local setmetatable = setmetatable
local type = type
local Vector3 = Vector3
local zero = Vector3.zero

local Bounds = 
{
	center = Vector3.zero,
	extents = Vector3.zero,
}

local get = tolua.initget(Bounds)

Bounds.__index = function(t,k)
	local var = rawget(Bounds, k)
	
	if var == nil then							
		var = rawget(get, k)
		
		if var ~= nil then
			return var(t)	
		end
	end
	
	return var
end

Bounds.__call = function(t, center, size)
	return setmetatable({center = center, extents = size * 0.5}, Bounds)		
end

function Bounds.New(center, size)	
	return setmetatable({center = center, extents = size * 0.5}, Bounds)		
end

function Bounds:Get()
	local size = self:GetSize()	
	return self.center, size
end

function Bounds:GetSize()
	return self.extents * 2
end

function Bounds:SetSize(value)
	self.extents = value * 0.5
end

function Bounds:GetMin()
	return self.center - self.extents
end

function Bounds:SetMin(value)
	self:SetMinMax(value, self:GetMax())
end

function Bounds:GetMax()
	return self.center + self.extents
end

function Bounds:SetMax(value)
	self:SetMinMax(self:GetMin(), value)
end

function Bounds:SetMinMax(min, max)
	self.extents = (max - min) * 0.5
	self.center = min + self.extents
end

function Bounds:Encapsulate(point)
	self:SetMinMax(Vector3.Min(self:GetMin(), point), Vector3.Max(self:GetMax(), point))
end

function Bounds:Expand(amount)	
	if type(amount) == "number" then
		amount = amount * 0.5
		self.extents:Add(Vector3.New(amount, amount, amount))
	else
		self.extents:Add(amount * 0.5)
	end
end

function Bounds:Intersects(bounds)
	local min = self:GetMin()
	local max = self:GetMax()
	
	local min2 = bounds:GetMin()
	local max2 = bounds:GetMax()
	
	return min.x <= max2.x and max.x >= min2.x and min.y <= max2.y and max.y >= min2.y and min.z <= max2.z and max.z >= min2.z
end    

function Bounds:Contains(p)
	local min = self:GetMin()
	local max = self:GetMax()
	
	if p.x < min.x or p.y < min.y or p.z < min.z or p.x > max.x or p.y > max.y or p.z > max.z then
		return false
	end
	
	return true
end

function Bounds:IntersectRay(ray)
	local tmin = -Mathf.Infinity
	local tmax = Mathf.Infinity
	
	local t0, t1, f
	local t = self:GetCenter () - ray:GetOrigin()
	local p = {t.x, t.y, t.z}
	t = self.extents
	local extent = {t.x, t.y, t.z}
	t = ray:GetDirection()
	local dir = {t.x, t.y, t.z}
  
	for i = 1, 3 do	
		f = 1 / dir[i]
		t0 = (p[i] + extent[i]) * f
		t1 = (p[i] - extent[i]) * f
			
		if t0 < t1 then			
			if t0 > tmin then tmin = t0 end				
			if t1 < tmax then tmax = t1 end				
			if tmin > tmax then return false end				
			if tmax < 0 then return false end        
		else			
			if t1 > tmin then tmin = t1 end				
			if t0 < tmax then tmax = t0 end				
			if tmin > tmax then return false end				
			if tmax < 0 then return false end
		end
	end
	
	return true, tmin
end

function Bounds:ClosestPoint(point)
	local t = point - self:GetCenter()
	local closest = {t.x, t.y, t.z}
	local et = self.extents
	local extent = {et.x, et.y, et.z}
	local distance = 0
	local delta
	
	for i = 1, 3 do	
		if  closest[i] < - extent[i] then		
			delta = closest[i] + extent[i]
			distance = distance + delta * delta
			closest[i] = -extent[i]
		elseif closest[i] > extent[i]  then
			delta = closest[i] - extent[i]
			distance = distance + delta * delta
			closest[i] = extent[i]
		end
	end
		
	if distance == 0 then	    
		return rkPoint, 0
	else	
		outPoint = closest + self:GetCenter()
		return outPoint, distance
	end
end

function Bounds:Destroy()
	self.center	= nil
	self.size	= nil
end

Bounds.__tostring = function(self)	
	return string.format("Center: %s, Extents %s", tostring(self.center), tostring(self.extents))
end

Bounds.__eq = function(a, b)
	return a.center == b.center and a.extents == b.extents
end

get.size = Bounds.GetSize
get.min = Bounds.GetMin
get.max = Bounds.GetMax

UnityEngine.Bounds = Bounds
setmetatable(Bounds, Bounds)
return Bounds
