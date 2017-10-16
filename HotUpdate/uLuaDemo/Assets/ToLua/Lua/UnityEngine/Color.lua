--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------

local rawget = rawget
local setmetatable = setmetatable
local type = type
local Mathf = Mathf

local Color = {}
local get = tolua.initget(Color)

Color.__index = function(t,k)
	local var = rawget(Color, k)
		
	if var == nil then							
		var = rawget(get, k)
		
		if var ~= nil then
			return var(t)	
		end
	end
	
	return var
end

Color.__call = function(t, r, g, b, a)
	return setmetatable({r = r or 0, g = g or 0, b = b or 0, a = a or 1}, Color)   
end

function Color.New(r, g, b, a)
	return setmetatable({r = r or 0, g = g or 0, b = b or 0, a = a or 1}, Color)		
end

function Color:Set(r, g, b, a)
	self.r = r
	self.g = g
	self.b = b
	self.a = a or 1 
end

function Color:Get()
	return self.r, self.g, self.b, self.a
end

function Color:Equals(other)
	return self.r == other.r and self.g == other.g and self.b == other.b and self.a == other.a
end

function Color.Lerp(a, b, t)
	t = Mathf.Clamp01(t)
	return Color.New(a.r + t * (b.r - a.r), a.g + t * (b.g - a.g), a.b + t * (b.b - a.b), a.a + t * (b.a - a.a))
end

function Color.LerpUnclamped(a, b, t)
  return Color.New(a.r + t * (b.r - a.r), a.g + t * (b.g - a.g), a.b + t * (b.b - a.b), a.a + t * (b.a - a.a))
end

function Color.HSVToRGB(H, S, V, hdr)
  hdr = hdr and false or true  
  local white = Color.New(1,1,1,1)
  
  if S == 0 then    
    white.r = V
    white.g = V
    white.b = V
    return white
  end
  
  if V == 0 then    
    white.r = 0
    white.g = 0
    white.b = 0
    return white
  end
  
  white.r = 0
  white.g = 0
  white.b = 0;
  local num = S
  local num2 = V
  local f = H * 6;
  local num4 = Mathf.Floor(f)
  local num5 = f - num4
  local num6 = num2 * (1 - num)
  local num7 = num2 * (1 - (num * num5))
  local num8 = num2 * (1 - (num * (1 - num5)))
  local num9 = num4
  
  local flag = num9 + 1
  
  if flag == 0 then
    white.r = num2
    white.g = num6
    white.b = num7
  elseif flag == 1 then
    white.r = num2
    white.g = num8
    white.b = num6
  elseif flag == 2 then
    white.r = num7
    white.g = num2
    white.b = num6
  elseif flag == 3 then
    white.r = num6
    white.g = num2
    white.b = num8
  elseif flag == 4 then
    white.r = num6
    white.g = num7
    white.b = num2
  elseif flag == 5 then
    white.r = num8
    white.g = num6
    white.b = num2
  elseif flag == 6 then
    white.r = num2
    white.g = num6
    white.b = num7
  elseif flag == 7 then
    white.r = num2
    white.g = num8
    white.b = num6
  end
  
  if not hdr then    
    white.r = Mathf.Clamp(white.r, 0, 1)
    white.g = Mathf.Clamp(white.g, 0, 1)
    white.b = Mathf.Clamp(white.b, 0, 1)
  end
    
  return white
end

local function RGBToHSVHelper(offset, dominantcolor, colorone, colortwo)
  local V = dominantcolor
    
  if V ~= 0 then    
    local num = 0
        
    if colorone > colortwo then        
      num = colortwo
    else        
      num = colorone
    end
        
    local num2 = V - num
    local H = 0
    local S = 0
        
    if num2 ~= 0 then        
      S = num2 / V
      H = offset + (colorone - colortwo) / num2
    else        
      S = 0
      H = offset + (colorone - colortwo)
    end
        
    H = H / 6  
    if H < 0 then H = H + 1 end                
    return H, S, V
  end
  
  return 0, 0, V  
end

function Color.RGBToHSV(rgbColor)
    if rgbColor.b > rgbColor.g and rgbColor.b > rgbColor.r then    
        return RGBToHSVHelper(4, rgbColor.b, rgbColor.r, rgbColor.g)    
    elseif rgbColor.g > rgbColor.r then    
        return RGBToHSVHelper(2, rgbColor.g, rgbColor.b, rgbColor.r)
    else    
        return RGBToHSVHelper(0, rgbColor.r, rgbColor.g, rgbColor.b)
    end
end

function Color.GrayScale(a)
	return 0.299 * a.r + 0.587 * a.g + 0.114 * a.b
end

Color.__tostring = function(self)
	return string.format("RGBA(%f,%f,%f,%f)", self.r, self.g, self.b, self.a)
end

Color.__add = function(a, b)
	return Color.New(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a)
end

Color.__sub = function(a, b)	
	return Color.New(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a)
end

Color.__mul = function(a, b)
	if type(b) == "number" then
		return Color.New(a.r * b, a.g * b, a.b * b, a.a * b)
	elseif getmetatable(b) == Color then
		return Color.New(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a)
	end
end

Color.__div = function(a, d)
	return Color.New(a.r / d, a.g / d, a.b / d, a.a / d)
end

Color.__eq = function(a,b)
	return a.r == b.r and a.g == b.g and a.b == b.b and a.a == b.a
end

get.red 	= function() return Color.New(1,0,0,1) end
get.green	= function() return Color.New(0,1,0,1) end
get.blue	= function() return Color.New(0,0,1,1) end
get.white	= function() return Color.New(1,1,1,1) end
get.black	= function() return Color.New(0,0,0,1) end
get.yellow	= function() return Color.New(1, 0.9215686, 0.01568628, 1) end
get.cyan	= function() return Color.New(0,1,1,1) end
get.magenta	= function() return Color.New(1,0,1,1) end
get.gray	= function() return Color.New(0.5,0.5,0.5,1) end
get.clear	= function() return Color.New(0,0,0,0) end

get.gamma = function(c) 
  return Color.New(Mathf.LinearToGammaSpace(c.r), Mathf.LinearToGammaSpace(c.g), Mathf.LinearToGammaSpace(c.b), c.a)  
end

get.linear = function(c)
  return Color.New(Mathf.GammaToLinearSpace(c.r), Mathf.GammaToLinearSpace(c.g), Mathf.GammaToLinearSpace(c.b), c.a)
end

get.maxColorComponent = function(c)    
  return Mathf.Max(Mathf.Max(c.r, c.g), c.b)
end

get.grayscale = Color.GrayScale

UnityEngine.Color = Color
setmetatable(Color, Color)
return Color



