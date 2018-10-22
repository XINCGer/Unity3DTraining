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

----------------------------------------
--
-- Utilities part 1
--
----------------------------------------

local function createProxy (metatable)
	return setmetatable({}, metatable)
end

local function shallowCopy (destTable, sourceTable)
	for k, v in pairs(sourceTable) do
		destTable[k] = v
	end
end

local function argError (iArg, funcName, needs, gets, errLevel)
	error(("bad argument #%d to '%s' (%s expected, got %s)")
		:format(iArg, funcName, needs, gets), errLevel + 1)
end

local function argListError (iArg, memberName, needs, gets, errLevel)
	if gets == nil then
		error(([[bad argument #%d to argument list of %s (%s)]])
			:format(iArg, memberName, needs), errLevel + 1)
	else
		error(([[bad argument #%d to argument list of %s (%s expected, got %s)]])
			:format(iArg, memberName, needs, gets), errLevel + 1)
	end
end

local function initValueError (fieldName, needs, gets, errLevel)
	if gets == nil then
		error(([[bad inital value to field '%s' (%s)]])
			:format(fieldName, needs), errLevel + 1)
	else
		error(([[bad inital value to field '%s' (%s expected, got %s)]])
			:format(fieldName, needs, gets), errLevel + 1)
	end
end