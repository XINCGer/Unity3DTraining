--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
local math = math
local floor = math.floor
local abs = math.abs
local Mathf = Mathf

Mathf.Deg2Rad = math.rad(1)
Mathf.Epsilon = 1.4013e-45
Mathf.Infinity = math.huge
Mathf.NegativeInfinity = -math.huge
Mathf.PI = math.pi
Mathf.Rad2Deg = math.deg(1)
		
Mathf.Abs = math.abs
Mathf.Acos = math.acos
Mathf.Asin = math.asin
Mathf.Atan = math.atan
Mathf.Atan2 = math.atan2
Mathf.Ceil = math.ceil
Mathf.Cos = math.cos
Mathf.Exp = math.exp
Mathf.Floor = math.floor
Mathf.Log = math.log
Mathf.Log10 = math.log10
Mathf.Max = math.max
Mathf.Min = math.min
Mathf.Pow = math.pow
Mathf.Sin = math.sin
Mathf.Sqrt = math.sqrt
Mathf.Tan = math.tan
Mathf.Deg = math.deg
Mathf.Rad = math.rad
Mathf.Random = math.random

function Mathf.Approximately(a, b)
	return abs(b - a) < math.max(1e-6 * math.max(abs(a), abs(b)), 1.121039e-44)
end

function Mathf.Clamp(value, min, max)
	if value < min then
		value = min
	elseif value > max then
		value = max    
	end
	
	return value
end

function Mathf.Clamp01(value)
	if value < 0 then
		return 0
	elseif value > 1 then
		return 1   
	end
	
	return value
end

function Mathf.DeltaAngle(current, target)    
	local num = Mathf.Repeat(target - current, 360)

	if num > 180 then
		num = num - 360
	end

	return num
end 

function Mathf.Gamma(value, absmax, gamma) 
	local flag = false
	
    if value < 0 then    
        flag = true
    end
	
    local num = abs(value)
	
    if num > absmax then    
        return (not flag) and num or -num
    end
	
    local num2 = math.pow(num / absmax, gamma) * absmax
    return (not flag) and num2 or -num2
end

function Mathf.InverseLerp(from, to, value)
	if from < to then      
		if value < from then 
			return 0
		end

		if value > to then      
			return 1
		end

		value = value - from
		value = value/(to - from)
		return value
	end

	if from <= to then
		return 0
	end

	if value < to then
		return 1
	end

	if value > from then
        return 0
	end

	return 1 - ((value - to) / (from - to))
end

function Mathf.Lerp(from, to, t)
	return from + (to - from) * Mathf.Clamp01(t)
end

function Mathf.LerpAngle(a, b, t)
	local num = Mathf.Repeat(b - a, 360)

	if num > 180 then
		num = num - 360
	end

	return a + num * Mathf.Clamp01(t)
end

function Mathf.LerpUnclamped(a, b, t)
    return a + (b - a) * t;
end

function Mathf.MoveTowards(current, target, maxDelta)
	if abs(target - current) <= maxDelta then
		return target
	end

	return current + Mathf.Sign(target - current) * maxDelta
end

function Mathf.MoveTowardsAngle(current, target, maxDelta)
	target = current + Mathf.DeltaAngle(current, target)
	return Mathf.MoveTowards(current, target, maxDelta)
end

function Mathf.PingPong(t, length)
    t = Mathf.Repeat(t, length * 2)
    return length - abs(t - length)
end

function Mathf.Repeat(t, length)    
	return t - (floor(t / length) * length)
end  

function Mathf.Round(num)
	return floor(num + 0.5)
end

function Mathf.Sign(num)  
	if num > 0 then
		num = 1
	elseif num < 0 then
		num = -1
	else 
		num = 0
	end

	return num
end

function Mathf.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime)
	maxSpeed = maxSpeed or Mathf.Infinity
	deltaTime = deltaTime or Time.deltaTime
    smoothTime = Mathf.Max(0.0001, smoothTime)
    local num = 2 / smoothTime
    local num2 = num * deltaTime
    local num3 = 1 / (1 + num2 + 0.48 * num2 * num2 + 0.235 * num2 * num2 * num2)
    local num4 = current - target
    local num5 = target
    local max = maxSpeed * smoothTime
    num4 = Mathf.Clamp(num4, -max, max)
    target = current - num4
    local num7 = (currentVelocity + (num * num4)) * deltaTime
    currentVelocity = (currentVelocity - num * num7) * num3
    local num8 = target + (num4 + num7) * num3
	
    if (num5 > current) == (num8 > num5)  then    
        num8 = num5
        currentVelocity = (num8 - num5) / deltaTime		
    end
	
    return num8,currentVelocity
end

function Mathf.SmoothDampAngle(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime)
	deltaTime = deltaTime or Time.deltaTime
	maxSpeed = maxSpeed or Mathf.Infinity	
	target = current + Mathf.DeltaAngle(current, target)
    return Mathf.SmoothDamp(current, target, currentVelocity, smoothTime, maxSpeed, deltaTime)
end


function Mathf.SmoothStep(from, to, t)
    t = Mathf.Clamp01(t)
    t = -2 * t * t * t + 3 * t * t
    return to * t + from * (1 - t)
end

function Mathf.HorizontalAngle(dir) 
	return math.deg(math.atan2(dir.x, dir.z))
end

function Mathf.IsNan(number)
	return not (number == number)
end

UnityEngine.Mathf = Mathf
return Mathf