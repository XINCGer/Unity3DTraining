--[[
	Colaplus: Lua高级面向对象开发插件
]]

local ColaPlus = {}

----------------------------------------
--
-- external functions
--
----------------------------------------

local rawget = rawget
local type = type
local pairs = pairs
local setmetatable = setmetatable
local getmetatable = getmetatable
local tostring = tostring
local error = error
local select = select
local assert = assert
local require = require
local pcall = pcall
local debug = debug

-- debug
local _G = _G
--~ local _error = error
--~ local function error (what)
--~ 	return _error(what, 2)
--~ end

local _ENV = nil	-- help to do spelling checking, compatible with lua 5.1