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

----------------------------------------
--
-- ColaPlus config
--
----------------------------------------

local config =
{
	--default config
	reflection = false,
	declare_checking = true,
	accessing_checking = true,
	calling_checking = true,
	reload = false,
}

--
-- load config
--
local loadedConfig
do
	-- load from module ColaPlus_config
	local currentModule = ...
	if currentModule ~= nil then
		local configModule = currentModule .. "_config"
		local bRet, result = pcall(require, configModule)
		if bRet then
			if type(result) ~= "table" then
				error("module ColaPlus_config should return a table, got: " .. type(result))
			end
			loadedConfig = result
		end
	end
end

if loadedConfig ~= nil then
	shallowCopy(config, loadedConfig)
end

--
-- feature config
--

local feature_reflection = config.reflection
local feature_declare_checking = config.declare_checking
local feature_accessing_checking = config.accessing_checking
local feature_calling_checking = config.calling_checking
local feature_reload = config.reload

--
-- derived feature config
--

local feature_member_info = feature_declare_checking or feature_accessing_checking


----------------------------------------
--
-- ColaPlus global variables
--
----------------------------------------

-- set flag that ColaPlus library is initializing
local bInitializingColaPlus = true

-- to determine value is ColaPlus object
local ColaPlusObjectMagic = {}

-- to determine value is ColaPlus type table
local ColaPlusTypeTableMagic = {}

-- to control function wrapper
local ColaPlusFunctionWrapperControlMagic = {}

-- mapping type name to type table
local typeNameToTableMap = {}

----------------------------------------
--
-- Utilities part 2
--
----------------------------------------

local basicTypeSet =
{
	["nil"] = true,
	["number"] = true,
	["string"] = true,
	["boolean"] = true,
	["table"] = true,
	["function"] = true,
	["thread"] = true,
	["userdata"] = true,

	["dynamic"] = true,		--any type (lua default style)
}

local primaryTypeSet =
{
	["nil"] = true,
	["number"] = true,
	["string"] = true,
	["boolean"] = true,
}

local nonNilPrimaryTypeSet =
{
	["number"] = true,
	["string"] = true,
	["boolean"] = true,
}

local function getTypeTableMeta (typeTable)
	if type(typeTable) ~= "table" then
		return nil
	end

	local meta = getmetatable(typeTable)

	--check magic
	if meta == nil or meta.magic ~= ColaPlusTypeTableMagic then
		return nil
	end

	return meta
end

local getTypeTableMetaNoCheck = getmetatable

local function getObjectMeta (obj)
	if type(obj) ~= "table" then
		return nil
	end

	--check magic
	local meta = getmetatable(obj)
	if meta == nil or meta.magic ~= ColaPlusObjectMagic then
		return nil
	end

	return meta
end

local getObjectMetaNoCheck = getmetatable

local function getObjectTypeTable (obj)
	return getObjectMetaNoCheck(obj).typeTable
end

local function getObjectTypeMeta (obj)
	return getTypeTableMetaNoCheck(getObjectTypeTable(obj))
end


local function checkTypeTable (typeTable, iArg, who, what, errLevel)
	if getTypeTableMeta(typeTable) == nil then
		argError(iArg, funcName, "type table", type(typeTable), errLevel+1)
	end
end

local function formatTypeName (typeValue)
	if typeValue == nil then
		return "<none>"
	elseif type(typeValue) == "string" then
		return '"' .. typeValue .. '"'
	else
		return tostring(typeValue)
	end
end

local function formatObjectTypeName (object)
	if ColaPlus.is(object, ColaPlus.Object) then
		return tostring(object:getTypeTable())
	else
		return '"' .. type(object) .. '"'
	end
end

local function checkValidArgType (typeValue, iParam, memberName, errLevel)
	if type(typeValue) == "string" then
		if not basicTypeSet[typeValue] then
			argListError(iParam, memberName, "vaild basic type name", '"'..typeValue..'"', errLevel+1)
		end
	else
		if getTypeTableMeta(typeValue) == nil then
			argListError(iParam, memberName, "type table", type(typeValue), errLevel+1)
		end
	end
end

local function isTypeCompatible (value, needType)
	if needType == "dynamic" then
		return true
	elseif type(needType) == "string" then
		if nonNilPrimaryTypeSet[needType] then
			return type(value) == needType
		else
			-- nilable type is compatible with nil
			return value == nil or type(value) == needType
		end
	else
		return value == nil or ColaPlus.is(value, needType)
	end
end


local function checkValueCompatible (value, needType, format, who, errorLevel)
	if not isTypeCompatible(value, needType) then
		local what = format:format(who)
		error(([[bad %s (%s expected, got %s)]])
			:format(what, formatTypeName(needType), formatObjectTypeName(value)), errorLevel + 1)
	end
end

local function isTypeTableInterface (typeTable)
	return getTypeTableMetaNoCheck(typeTable).isInterface
end

----------------------------------------
--
-- type creation
--
----------------------------------------

local function inheritMemberInfo (derivedMemberInfo, baseMemberInfo)
	for name, info in pairs(baseMemberInfo) do
		if info.inheritable then
			derivedMemberInfo[name] = info
		end
	end
end

-- copy base members to derived type meta
local function inheritMembers (derivedMeta, baseTypeTable)
	-- inherit from baseTypeTable
	local baseMeta = getTypeTableMetaNoCheck(baseTypeTable)

	shallowCopy(derivedMeta.fields, baseMeta.fields)
	shallowCopy(derivedMeta.constructors, baseMeta.constructors)
	shallowCopy(derivedMeta.methods, baseMeta.methods)
	shallowCopy(derivedMeta.abstractMethods, baseMeta.abstractMethods)
	shallowCopy(derivedMeta.staticMembers, baseMeta.staticMembers)
	shallowCopy(derivedMeta.compatibleTypes, baseMeta.compatibleTypes)

	if feature_member_info then
		inheritMemberInfo(derivedMeta.memberInfoMap, baseMeta.memberInfoMap)
	end
end

-- param paramList: { param1, param2, ..., "=>", ret1, ret2, ... }
-- return: { [n] = paramN, [-n] = retN }
local function checkMethodParamAndMakeList (paramCount, paramList, methodName, errorLevel)
	local result = {}

	local bReturnPart = false
	local iSeperator = -1
	local bGotParamValist = false
	local bGotReturnValist = false

	for i = 1, paramCount do
		local inParam = paramList[i]
		if inParam == "=>" then
			bReturnPart = true
			iSeperator = i
		elseif inParam == "varlist" then
			if bReturnPart then
				bGotReturnValist = true
				result[-(i-iSeperator)] = inParam	-- last return value type
			else
				bGotParamValist = true
				result[i] = inParam					-- last param value type
			end
		else
			checkValidArgType(inParam, i, methodName, errorLevel+1)
			if bReturnPart then
				if bGotReturnValist then
					argListError(i, methodName, '"varlist" must be the last return value type', nil, errorLevel + 1)
				end
				result[-(i-iSeperator)] = inParam	-- a return value type
			else
				if bGotParamValist then
					argListError(i, methodName, '"varlist" must be the last param value type', nil, errorLevel + 1)
				end
				result[i] = inParam					-- a param value type
			end
		end
	end

	return result
end

-- return diff index, or nil if equal
local function compareSignature (signatureLeft, signatureRight)
	local iParam = 1
	repeat
		local paramLeft = signatureLeft[iParam]
		local paramRight = signatureRight[iParam]

		if paramLeft ~= paramRight then
			return iParam
		end

		iParam = iParam + 1
	until paramLeft == nil

	local iReturn = 1
	repeat
		local returnLeft = signatureLeft[-iReturn]
		local returnRight = signatureRight[-iReturn]

		if returnLeft ~= returnRight then
			return -iReturn
		end

		iReturn = iReturn + 1
	until returnLeft == nil

	return nil
end

local function checkMethodOverrideSignature (signatureBase, signatureOverride, methodType, methodName, errLevel)
	local iDiff = compareSignature(signatureBase, signatureOverride)
	if iDiff ~= nil then
		local what
		local iWhat
		if iDiff > 0 then
			what = "param"
			iWhat = iDiff
		else
			what = "return value"
			iWhat = -iDiff
		end

		error(([[bad %s type #%d to %s '%s' (%s expected, got %s)]])
			:format(what, iWhat, methodType, methodName,
				formatTypeName(signatureBase[iDiff]),
				formatTypeName(signatureOverride[iDiff])),
			errLevel+1)
	end
end

local forwardDeclaredType = {}

local function createForwardDeclareType (typeName, errorLevel)
	if typeName ~= nil then
		if type(typeName) ~= "string" then
			error("invalid type name (string expected, got " .. type(typeName) .. ")", errorLevel+1)
		end

		local typeTable = typeNameToTableMap[typeName]
		if typeTable ~= nil then
			return typeTable
		end
	end

	local typeTable = forwardDeclaredType[typeName]
	if typeTable ~= nil then
		return typeTable
	end


	local typeString = (typeName or "anonymousType") .. "(forward declare)"
	local typeMeta =
	{
		magic = ColaPlusTypeTableMagic;
		isForwardDeclared = true;
		typeName = typeName;
		__tostring = function (_)
			return typeString
		end;
	}

	typeTable = {}
	setmetatable(typeTable, typeMeta)

	if typeName ~= nil then
		forwardDeclaredType[typeName] = typeTable
	end

	return typeTable
end

local function isTypeTableForwardDeclared (typeTable)
	return getTypeTableMetaNoCheck(typeTable).isForwardDeclared
end

-- if forwardDeclare is given, it's name should equal to typeName
local function createEmptyType (typeName, forwardDeclare, errorLevel)
	local resultTable
	if forwardDeclare ~= nil then
		resultTable = forwardDeclare
	else
		if typeName == nil then
			resultTable = {}
		else
			resultTable = forwardDeclaredType[typeName] or {}
		end
	end

	if typeName ~= nil then
		forwardDeclaredType[typeName] = nil		-- fetch the forward declared type table
	end

	return setmetatable(resultTable, nil)
end

-- check ..., return ...
-- needTypes.selfType = type of self when not static
-- needTypes.format: format(who), needTypes.format: format(i, who)
local function checkValuesCompatible (needTypes, errLevel, ...)
	local nGets = select("#", ...)
	local nNeeds = #needTypes
	local bVarlist = needTypes.bVarlist

	if nGets < nNeeds then
		local what = needTypes.format:format(needTypes.who)
		error(([[too little %s (%d expected, got %d)]])
			:format(what, nNeeds, nGets), errLevel+1)
	end

	if not bVarlist and nGets > nNeeds then
		local what = needTypes.format:format(needTypes.who)
		error(([[too many %s (%d expected, got %d)]])
			:format(what, nNeeds, nGets), errLevel+1)
	end

	local selfType = needTypes.selfType
	if selfType ~= nil then
		local self = ...
		if self == nil then
			local what = needTypes.ithFormat:format(1, needTypes.who)
			error(([[bad %s (%s expected, got %s)]])
				:format(what, formatTypeName(selfType), formatObjectTypeName(self)), errLevel + 1)
		end
	end
	for i = 1, nNeeds do
		local getValue = select(i, ...)
		local needType = needTypes[i]

		if needType == "varlist" then	--"varlist" matches anything
			return
		end

		if not isTypeCompatible(getValue, needType) then
			local what = needTypes.ithFormat:format(i, needTypes.who)
			error(([[bad %s (%s expected, got %s)]])
				:format(what, formatTypeName(needType), formatObjectTypeName(getValue)), errLevel + 1)
		end
	end

	return ...
end

local function createEmptyFunctionWrapper ()
	local func
	local methodName
	local tableType
	local bCheckType

	local arguments
	local returns

	return function (...)
		local magic, op = ...
		if magic == ColaPlusFunctionWrapperControlMagic then	-- control command
			if op == "set" then
				local _, methodInfo
				_, _, func, methodName, tableType, methodInfo, bCheckType = ...

				local bInstanceMethod =
					methodInfo.style:find("s", 1, true) == nil
					and methodInfo.style:find("f", 1, true) == nil

				local needTypes = methodInfo.valueType
				arguments =
				{
					format="arguments to '%s' in " .. tostring(tableType),
					ithFormat="argument #%d to '%s' in " .. tostring(tableType),
					who = methodName,
				}
				returns =
				{
					format="return values from '%s' in " .. tostring(tableType),
					ithFormat="return value #%d from '%s' in " .. tostring(tableType),
					who = methodName,
				}
				do
					if bInstanceMethod then
						arguments[#arguments + 1] = tableType
						arguments.selfType = tableType
					end
					local iArg = 1
					while needTypes[iArg] do
						local needType = needTypes[iArg]
						if needType == "varlist" then
							arguments.bVarlist = true
						else
							arguments[#arguments + 1] = needType
						end
						iArg = iArg + 1
					end
				end
				do
					local iRet = 1
					while needTypes[-iRet] do
						local needType = needTypes[-iRet]
						if needType == "varlist" then
							returns.bVarlist = true
						else
							returns[iRet] = needTypes[-iRet]
						end
						iRet = iRet + 1
					end
				end
			end

			return
		end

		-- function call
		if bCheckType then
			return checkValuesCompatible(returns, 1, func(checkValuesCompatible(arguments, 2, ...)))
		else
			return func(...)
		end
	end
end

local function setFunctionWrapper (wrapper, func, methodName, tableType, methodInfo, bCheckType)
	wrapper(ColaPlusFunctionWrapperControlMagic, "set", func, methodName, tableType, methodInfo, bCheckType)
end

local function createFunctionWrapper (func, methodName, tableType, methodInfo, bCheckType)
	local wrapper = createEmptyFunctionWrapper()
	setFunctionWrapper(wrapper, func, methodName, tableType, methodInfo, bCheckType)

	return wrapper
end

-- create type with given base type and type name
--	base type could be nil, which means bCreatingClass Object
local function createType (baseTypeTable, typeNameOrForwardDeclare, isInterface)
	--
	-- checking parameters
	--

	local theForwardDeclare
	local typeName

	if type(typeNameOrForwardDeclare) == "table" then
		theForwardDeclare = typeNameOrForwardDeclare

		local forwardDeclareMeta = getTypeTableMeta(theForwardDeclare)

		if forwardDeclareMeta == nil then	-- not type table
			error("invalid forward declare (type table expected, got " .. type(typeName) .. ")", 2)
		end
		if not forwardDeclareMeta.isForwardDeclared then
			error("invalid forward declare (type is already defined)", 2)
		end
		if forwardDeclareMeta.typeName ~= nil then
			error('invalid forward declare (anonymous expected, got name "' .. forwardDeclareMeta.typeName .. '")', 2)
		end
	else
		typeName = typeNameOrForwardDeclare

		--check typeName
		if typeName ~= nil then
			if type(typeName) ~= "string" then
				error("invalid type name (string expected, got " .. type(typeName) .. ")", 2)
			end
			if typeNameToTableMap[typeName] ~= nil then
				error('type with name "' .. typeName .. '" already exist', 2)
			end
		end
	end

	--checking base type
	if baseTypeTable ~= nil then
		local baseMeta = getTypeTableMeta(baseTypeTable)
		if baseMeta == nil then
			if not bInitializingColaPlus then		--when Type extends Object, baseMeta is nil
				error ("Param 1 expect a ColaPlus type table, got: " .. type(baseTypeTable), 2)
			end
		else
			if baseMeta.isForwardDeclared then
				error("forward declared type " .. tostring(baseTypeTable) .. " can not be inherited", 2)
			end

			local isBaseInterface = baseMeta.isInterface
			if isBaseInterface then		-- both explicit interface declaration and extending an interface make the type an interface
				isInterface = true
			end

			if baseTypeTable ~= ColaPlus.Object and isBaseInterface ~= isInterface then
				if isInterface then
					error("interface can not inherit from non-interface type", 2)
				else
					error("non-interface can not inherit from interface type", 2)
				end
			end
		end
	end

	-- set flag that the creation of type has not finished
	local bCreatingClass = true

	-- the created type
	local theType = createEmptyType(typeName, theForwardDeclare, 2)

	-- metatable of the type
	local typeMeta = {}

	--
	-- setup typeMeta
	--
	do
		typeMeta.magic = ColaPlusTypeTableMagic

		--
		-- members
		--

		-- fields
		typeMeta.fields = {}
		-- constructors
		typeMeta.constructors = {}
		-- non-static methods
		typeMeta.abstractMethods = {}
		-- abstract non-static methods
		typeMeta.methods = {}
		-- static methods, static and constant fields
		typeMeta.staticMembers = {}

		-- members info map (for checking)
		if feature_member_info then
			typeMeta.memberInfoMap = {}
		end

		--
		-- extra information
		--

		typeMeta.isInterface = isInterface
		typeMeta.baseTypeTable = baseTypeTable
		typeMeta.typeName = typeName

		local typeString = (typeName or "anonymousType") .. "(" .. tostring(theType) .. ")"
		typeMeta.__tostring = function (_)
			return typeString
		end

		-- build type info (for convertion)
		typeMeta.typeInfo = nil	--will be created when call obj:getType()
		typeMeta.compatibleTypes = { [theType] = true }	-- base type will be add by inheritMembers()
		typeMeta.implementInterfaces = {}	-- only directly implemented interfaces
	end

	-- inherit from baseTypeTable
	if baseTypeTable ~= nil then
		inheritMembers(typeMeta, baseTypeTable)
	end

	--
	-- Add interface
	--

	--[[
		Special operator Implement
		declare that this type implements an interface
			param interfaceTypeTable: type table of interface
			return type table itself
		e.g: MyClass.Implement(IEquatable).Implement(IComparable)
	]]
	function theType.Implement (interfaceTypeTable)
		typeMeta.compatibleTypes[interfaceTypeTable] = true
		typeMeta.implementInterfaces[interfaceTypeTable] = true
		return theType
	end

	--
	-- member declare
	--

	-- style is a string, each char stands for a flag
	--	'c': constant
	local function FieldInternal (style, fieldType)
		if isInterface then
			error("interface can not have field", 2)
		end
		if not bCreatingClass then
			error("bad field defining (type " .. tostring(theType) .. " is already committed)", 2)
		end

		local bConstant = (style == "c")

		return createProxy
		{
			__index = function (_, fieldName)
				error("You need to give field a default value", 2)
			end;

			__newindex = function (_, fieldName, initValue)
				if type(fieldName) ~= "string" then
					error("Field name should be string, got: "..type(fieldName), 2)
				end

				local memberInfo
				if feature_member_info then
					local memberInfoMap = typeMeta.memberInfoMap

					-- checking name conflict
					if memberInfoMap[fieldName] ~= nil then
						error('member with name "' .. fieldName .. '" already exists', 2)
					end

					checkValidArgType(fieldType, 1, fieldName, 2)
					-- record member info
					memberInfo =
					{
						memberType = "field",
						style = style,
						typeTable = theType,
						inheritable = true,
						valueType = fieldType
					}
				end

				if bConstant then
					checkValueCompatible(initValue, fieldType, [[initial value to field '%s']], fieldName, 2)
					typeMeta.staticMembers[fieldName] = initValue
				else
					local initValueType = type(initValue)
					if not primaryTypeSet[initValueType] and initValueType ~= "function" then
						initValueError(fieldName, "number, boolean, string, nil or function", initValueType, 2)
					end

					if type(initValue) ~= "function" then
						checkValueCompatible(initValue, fieldType, [[initial value to field '%s']], fieldName, 2)
					end
					typeMeta.fields[fieldName] = initValue		--... checking whether table, userdata
				end

				-- register member info only when succeeded
				if feature_member_info then
					local memberInfoMap = typeMeta.memberInfoMap
					memberInfoMap[fieldName] = memberInfo
				end
			end;
		}
	end

	--[[
		Special operator Field
		Declare a field
		Initial value of the field can only be number, boolean, string, nil or function
		If initial value is a function, it will be called when instantiate object. The return value will be assigned to field
			param fieldType: type of field
		e.g:
			Field("number").m_fieldName_1 = 1
			Field("table").m_fieldName_2 = function () return {"new table"} end
	]]
	local function Field (fieldType)
		return FieldInternal("", fieldType)
	end

	--[[
		Special operator ConstField
		declare a static field
			param fieldType: type of field
		e.g: Field("number").m_fieldName = 1
	]]
	local function ConstField (fieldType)
		return FieldInternal("c", fieldType)
	end

	-- style is a string, each char stands for a flag
	--	's': static, 'v': virtual, 'o': override, 'f': final
	local function MethodInternal (style, ...)
		if not bCreatingClass then
			error("bad method defining (type " .. tostring(theType) .. " is already committed)", 2)
		end

		assert(#style <= 1)

		local bStatic = (style == "s")
		local bVirtual = (style == "v")
		local bOverride = (style == "o")
		local bFinal = (style == "f")
		local bConstructor = (style == "c")

		local paramCount
		local paramList
		if feature_member_info then
			paramCount = select("#", ...)
			paramList = {...}
		end

		return createProxy
		{
			__index = function (_, methodName)
				error("You need to give method a function body", 2)
			end;

			__newindex = function (_, methodName, functionBody)
				if type(methodName) ~= "string" then
					error("Method name should be string, got: "..type(methodName), 2)
				end
				if type(functionBody) ~= "function" then
					error("Need function body, got: "..type(functionBody), 2)
				end

				if feature_member_info then
					local memberInfoMap = typeMeta.memberInfoMap

					local baseMemberInfo

					-- checking name conflict
					if not bConstructor and memberInfoMap[methodName] ~= nil then
						local oldInfo = memberInfoMap[methodName]
						if bOverride and oldInfo.memberType == "method"	--only "override" method can override others
								and oldInfo.typeTable ~= theType then	--can only override once in one type
							if oldInfo.style == "v" or oldInfo.style == "o" then
								baseMemberInfo = oldInfo	-- will check signature later
							else
								error('Can not override non-virtual method "' .. methodName .. '"', 2)
							end
						else
							error('member with name "' .. methodName .. '" already exists', 2)
						end
					end

					-- checking constructor
					if bConstructor and paramCount ~= 0 then
						error([[constructor can not have extra param and return value]], 2)
					end

					-- checking override
					if bOverride and baseMemberInfo == nil then
						error('overrided method with name "' .. methodName .. '" not exists', 2)
					end

					local memberInfo =
					{
						memberType = "method",
						style = style,
						typeTable = theType,
						inheritable = not bFinal,
						valueType = checkMethodParamAndMakeList(paramCount, paramList, methodName, 2)
					}

					-- record member info
					if not bConstructor then
						if baseMemberInfo ~= nil then
							checkMethodOverrideSignature(baseMemberInfo.valueType, memberInfo.valueType, "override method", methodName, 2)
						end

						memberInfoMap[methodName] = memberInfo
					end

					if feature_calling_checking then
						functionBody = createFunctionWrapper(functionBody, methodName, theType, memberInfo, true)
					end
				end

				if bConstructor then
					if isInterface then
						error("Interface can not have constructor", 2)
					end

					typeMeta.constructors[#typeMeta.constructors+1] = functionBody
				elseif bStatic or bFinal then
					if isInterface then
						error("Interface can not have static method", 2)
					end

					typeMeta.staticMembers[methodName] = functionBody
				else
					if isInterface then
						typeMeta.abstractMethods[methodName] = functionBody
					else
						typeMeta.methods[methodName] = functionBody
					end
				end
			end;
		}
	end


	--[[
		Special operation Method
		declare a method
			params: params types and return value types, params types and return types are seperated by "=>"
		e.g:
			-- declare method with one number param and no return value
			Method("number").foo = function (self, numberParam) end
			-- declare method with one number param, one string param, one boolean return value, and one string return value
			Method("number", "string", "=>", "boolean", "string").foo = function (self, numberParam) end
	]]
	local function Method (...)
		return MethodInternal("", ...)
	end

	--[[
		Special operation VirtualMethod
		declare a virtual method
			a virtual method is similar to normal method, but can be overrided
	]]
	local function VirtualMethod (...)
		return MethodInternal("v", ...)
	end

	--[[
		Special operation OverrideMethod
		declare a override method
			Virtual method of base type can be overrided
	]]
	local function OverrideMethod (...)
		return MethodInternal("o", ...)
	end

	--[[
		Special operation StaticMethod
		declare a static method
			params: params types and return value types, params types and return types are seperated by "=>"
		e.g:
			-- declare method with one number param and no return value
			StaticMethod("number").foo = function (numberParam) end
			-- declare method with one number param, one string param, one boolean return value, and one string return value
			StaticMethod("number", "string", "=>", "boolean", "string").foo = function (numberParam) end
	]]
	local function StaticMethod (...)
		return MethodInternal("s", ...)
	end

	--[[
		Special operation FinalMethod
		declare a final method. Final method is static, and is not inheritable
			params: params types and return value types, params types and return types are seperated by "=>"
		e.g:
			-- declare method with one number param and no return value
			StaticMethod("number").foo = function (numberParam) end
			-- declare method with one number param, one string param, one boolean return value, and one string return value
			StaticMethod("number", "string", "=>", "boolean", "string").foo = function (numberParam) end
	]]
	local function FinalMethod (...)
		return MethodInternal("f", ...)
	end

	--[[
		Special operation ConstructorMethod
		declare a constructor. Constructor takes the object instance as the only parameter, and returns no value
		Constructor of type and all of its base types will be invoked when create object instance
		Constructor of base type will be invoked firstly
		Name of constructor is insignificant
		e.g:
			Constructor(MyClass).init = function (self) end
	]]
	local function ConstructorMethod (...)
		return MethodInternal("c", ...)
	end

	local define =
	{
		field = Field,
		const = ConstField,
		method = Method,
		virtual = VirtualMethod,
		override = OverrideMethod,
		static = StaticMethod,
		final = FinalMethod,
		constructor = ConstructorMethod,
	}

	typeMeta.define = define

	--[[
		Special operation define
		get member declaring operators
		e.g:
			Class.define.field("number").m_fieldName = 1
	]]
	theType.define = define

	--
	-- finish class creation
	--

	local function commit ()
		bCreatingClass = false

		--
		-- remove special operators that are only used in class creation
		--
		do
			theType.Implement = nil
			theType.define = nil
			theType.Commit = nil
		end

		--
		-- check whether interfaces are properly implemented
		--

		if not isInterface and feature_declare_checking then
			local selfMembers = typeMeta.memberInfoMap

			for interface, _ in pairs(typeMeta.implementInterfaces) do
				local interfaceMeta = getTypeTableMetaNoCheck(interface)
				for methodName, methodInfo in pairs(interfaceMeta.memberInfoMap) do
					assert(methodInfo.memberType == "method")

					local selfMemberInfo = selfMembers[methodName]
					if selfMemberInfo == nil or selfMemberInfo.memberType ~= "method" then
						error(("method '%s' in interface '%s' not implemented"):format(methodName, tostring(interface)), 2)
					end

					checkMethodOverrideSignature(methodInfo.valueType, selfMemberInfo.valueType, "method implemetation", methodName, 2)
				end
			end
		end

		--
		-- add members (including instance members and static members)
		--

		if feature_accessing_checking then
			local memberInfoMap = typeMeta.memberInfoMap

			-- return member info
			local function checkMemberInfo (memberName, bAccessInstanceMember, errorLevel)
				local memberInfo = memberInfoMap[memberName]
				if memberInfo == nil then
					error('member "' .. memberName .. '" not found in ' .. tostring(theType), errorLevel+1)
				end

				local style = memberInfo.style
				local bIsInstanceMember = (style ~= "s" and style ~= "c" and style ~= "f")

				if bAccessInstanceMember then
					if not bIsInstanceMember then
						error('can not access non-instance member "' .. memberName .. '" in ' .. tostring(theType), errorLevel+1)
					end
				else
					if bIsInstanceMember and memberInfo.memberType ~= "method" then
						error('can not access instance member "' .. memberName .. '" in ' .. tostring(theType), errorLevel+1)
					end
				end
				return memberInfo
			end

			local staticMembers = typeMeta.staticMembers
			local methods = typeMeta.methods

			function typeMeta.__index (_, memberName)
				local memberInfo = checkMemberInfo(memberName, false, 2)
				-- value in methods can only be function
				return methods[memberName] or staticMembers[memberName]
			end

			function typeMeta.__newindex (_, memberName, newValue)
					error('can not assign to static member "' .. memberName .. '"', 2)
			end

			function typeMeta.__tryget (_, memberName)
				local method = methods[memberName]
				if method ~= nil then
					return method
				else
					return staticMembers[memberName]
				end
			end

			-- add constructor
			--	note: interface does not have constructor
			function typeMeta.__call (_, ...)
				if isInterface then
					error("interface " .. tostring(theType) .. " can not instantiate", 2)
				end

				if select("#", ...) ~= 0 then
					error(([[bad argument to construct of %s (no argument expected, got %d)]])
						:format(tostring(theType), select("#", ...)), 2)
				end

				-- metatable of class instances
				local objMeta =
				{
					-- for getTypeTable() in Object
					typeTable = theType,
					magic = ColaPlusObjectMagic,
				}

				local memberInfoMap = typeMeta.memberInfoMap

				local objMembers = setmetatable({}, {__index = typeMeta.methods})

				function objMeta.__index (_, memberName)
					checkMemberInfo(memberName, true, 2)
					return objMembers[memberName]
				end

				function objMeta.__tostring (t)
					return t:toString()
				end

				local function assignInstanceMember (memberName, newValue, errorLevel)
					local memberInfo = checkMemberInfo(memberName, true, errorLevel+1)
					if memberInfo.memberType ~= "field" then
						error('can not assign to non-field member "' .. memberName .. '"', errorLevel+1)
					end

					if memberInfo.style:find("c", 1, true) ~= nil then
						error('can not assign to constant field "' .. memberName .. '"', errorLevel+1)
					end

					checkValueCompatible(newValue, memberInfo.valueType, [[assginment to field '%s']], memberName, errorLevel+1)

					objMembers[memberName] = newValue
				end

				local function initInstanceMembers (errorLevel)
					for name, initValue in pairs(typeMeta.fields) do
						local realInitValue = initValue
						-- if init value is a function should call it
						if type(initValue) == "function" then
							local bSucc, ret = pcall(initValue)
							if not bSucc then
								error('failed to get initial value for member "' .. name .. '" :\n  ' .. ret, level)
							end
							initValue = ret
						end

						assignInstanceMember(name, initValue, errorLevel + 1)
					end
				end

				local function invokeConstructors (obj, errorLevel)
					local constructors = typeMeta.constructors
					for i = 1, #constructors do
						local bSucc, ret = pcall(constructors[i], obj)
						if not bSucc then
							error('failed to call constructor level #' .. i .. '" :\n  ' .. ret, level)
						end
					end
				end

				function objMeta.__newindex (_, memberName, newValue)
					assignInstanceMember(memberName, newValue, 2)
				end

				function objMeta.__tryget (_, memberName)
					local memberInfo = memberInfoMap[memberName]
					if memberInfo then
						return objMembers[memberName]
					else
						return nil
					end
				end

				local obj = {}
				setmetatable(obj, objMeta)
				initInstanceMembers(2)
				invokeConstructors(obj, 2)
				return obj
			end
		else
			local function initInstanceMembers (obj, fields)
				local type = type
				for name, initValue in pairs(fields) do
					-- if init value is a function should call it
					if type(initValue) == "function" then
						obj[name] = initValue()
					else
						obj[name] = initValue
					end
				end
			end

			local function invokeConstructors (obj, constructors)
				for i = 1, #constructors do
					constructors[i](obj)
				end
			end

			-- copy typeMeta.staticMembers to type table
			shallowCopy(theType, typeMeta.staticMembers)
			shallowCopy(theType, typeMeta.methods)

			-- metatable of class instances
			local objMeta =
			{
				-- for getTypeTable() in Object
				typeTable = theType,
				magic = ColaPlusObjectMagic,
				__index = typeMeta.methods
			}

			function objMeta.__tostring (t)
				return t:toString()
			end

			-- add constructor
			function typeMeta.__call(_, ...)
				local obj = {}
				initInstanceMembers(obj, typeMeta.fields)
				setmetatable(obj, objMeta)
				invokeConstructors(obj, typeMeta.constructors)
				return obj
			end
		end

		-- metatable content changed, refresh it
		return setmetatable(theType, typeMeta)
	end
	typeMeta.commit = commit

	--[[
		Special operation Commit
		finish class creation, new members can not be added any more after commit
	]]
	theType.Commit = commit

	--
	-- register type
	--
	if typeName ~= nil then
		typeNameToTableMap[typeName] = theType
	end

	return setmetatable(theType, typeMeta)
end


-- utility function to get type info from type table
local function typeTableToTypeInfo (typeTable)
	local typeMeta = getTypeTableMetaNoCheck(typeTable)
	local typeInfo = typeMeta.typeInfo
	if typeInfo == nil then
		-- create type info if absent
		typeInfo = ColaPlus.Type.fromTypeTable(typeTable)
		typeMeta.typeInfo = typeInfo
	end
	return typeInfo
end

local function isCompatibleTypeTable (obj, typeTable)
	return getObjectTypeMeta(obj).compatibleTypes[typeTable] ~= nil
end

--[[
	Forward declare a class
		param typeName: type name of the class
		return an empty class, which can be used as parameter value type
]]
function ColaPlus.ForwardDeclare (typeName)
	return createForwardDeclareType(typeName, 1)
end

--
-- Create the ultimate base type of all classes: Object, and the type of type info: Type
--

do
	local Type = ColaPlus.ForwardDeclare("System.Type")

	local function getObjectTypeTable (obj)
		return getObjectMetaNoCheck(obj).typeTable
	end

	local function getObjectTypeMeta (obj)
		return getTypeTableMetaNoCheck(getObjectTypeTable(obj))
	end

	local Object = createType(nil, "System.Object", false)
	local def = Object.define

	--[[
		get type of this object
			The result value can be used as base class, but it is not a ColaPlus object
			To get ColaPlus object that represent the type, use getType
	]]
	def.method("=>", "table").getTypeTable = function (self)
		return getObjectTypeTable(self)
	end

	--[[
		get type info of this object
	]]
	def.method("=>", Type).getType = function (self)
		local typeTable = getObjectTypeTable(self)
		return typeTableToTypeInfo(typeTable)
	end

	def.method("table", "=>", "boolean").is = function (self, typeTable)
		return isCompatibleTypeTable(self, typeTable)
	end

	def.method("table", "=>", Object).as = function (self, typeTable)
		if self:is(typeTable) then
			return self
		else
			return nil
		end
	end

	def.method("table", "=>", Object).cast = function (self, theType)
		if self:is(theType) then
			return self
		else
			error("bad cast from " .. tostring(self:getTypeTable()) .. " to " .. tostring(theType), 2)
		end
	end

	def.virtual("=>", "string").toString = function (self)
		return ("%s#%d"):format(self:getType():getName(), ColaPlus.tableid(self))
	end

	--[[
		get member value while bypassing accessing checking
	]]
	def.method("string", "=>", "dynamic").tryget = function (self, memberName)
		if not feature_accessing_checking then
			return self[memberName]
		else
			return getmetatable(self).__tryget(self, memberName)
		end
	end

	Object.Commit()

	-- register Object into ColaPlus
	ColaPlus.Object = Object
end

--
-- Create the type of type info: Type
--

do
	local Type = createType(ColaPlus.Object, "System.Type", false)
	local def = Type.define

	--[[
		get type table that the type stands for
	]]
	def.method("=>", "table").toTypeTable = function (self)
		return self.m_typeTable
	end

	def.method("=>", "string").getName = function (self)
		return getTypeTableMetaNoCheck(self.m_typeTable).typeName or "anonymousType"
	end

	def.method("=>", "table").getBaseTypeTable = function (self)
		return getTypeTableMetaNoCheck(self.m_typeTable).baseTypeTable
	end

	def.method("=>", Type).getBaseType = function (self)
		local baseTypeTable = self:getBaseTypeTable()
		if baseTypeTable == nil then	-- has no base type
			return nil
		else
			return typeTableToTypeInfo(baseTypeTable)
		end
	end

	def.override("=>", "string").toString = function (self)
		return self:getName()
	end

	def.field("table").m_typeTable = nil

	def.final("table", "=>", Type).fromTypeTable = function (typeTable)
		local typeInfo = Type()
		typeInfo.m_typeTable = typeTable
		return typeInfo
	end

	Type.Commit()

	-- register Type into ColaPlus
	ColaPlus.Type = Type
end

--[[
	Create a class with Object to be it's base type
		param typeNameOrForwardDeclare:
			when is a string: name of the created class used for reflection
				typeName should be look like: "Namespace.Subspace.Subspace2.....SubspaceN.ClassShortName"
				typeName also can have no namespace: "ClassName"
			when is a table: anonymous forward declared type (return value by ColaPlus.ForwardDeclare)
		return the created type table
]]
function ColaPlus.Class (typeNameOrForwardDeclare)
	return createType(ColaPlus.Object, typeNameOrForwardDeclare, false)
end

--[[
	Create a class with baseTypeTable to be it's base type
		param baseTypeTable: base type of the created class
		param typeNameOrForwardDeclare:
			when is a string: name of the created class used for reflection
				typeName should be look like: "Namespace.Subspace.Subspace2.....SubspaceN.ClassShortName"
				typeName also can have no namespace: "ClassName"
			when is a table: anonymous forward declared type (return value by ColaPlus.ForwardDeclare)
		return the created type table
]]
function ColaPlus.Extend (baseTypeTable, typeNameOrForwardDeclare)
	if not getTypeTableMeta(baseTypeTable) then
		argError(1, "Extend", "type table", type(baseTypeTable), 2)
	end
	return createType(baseTypeTable, typeNameOrForwardDeclare, false)
end

--[[
	Create a interface with Object to be it's base type
		return the created interface type table
]]
function ColaPlus.Interface (typeNameOrForwardDeclare)
	return createType(ColaPlus.Object, typeNameOrForwardDeclare, true)
end

--[[
	Create a interface with baseTypeTable to be it's base type, baseTypeTable must be an interface
		return the created interface type table
]]
function ColaPlus.ExtendInterface (baseTypeTable, typeNameOrForwardDeclare)
	return createType(baseTypeTable, typeNameOrForwardDeclare, true)
end

----------------------------------------
--
-- Type checking and convertion
--
----------------------------------------

--[[
	get type from given type table
		param typeTable: a type table
]]
function ColaPlus.typeof (typeTable)
	if getTypeTableMeta(typeTable) == nil then
		argError(1, "typeof", "type table", type(typeTable), 2)
	end
	return typeTableToTypeInfo(typeTable)
end

--[[
	check whether given value is a type table
		param typeTable: the type table to check
]]
function ColaPlus.isTypeTable (typeTable)
	return getTypeTableMeta(typeTable) ~= nil
end

--[[
	check whether given object is instance of certain type
		param obj: the object to check
		param typeTable: type table of the type
]]
function ColaPlus.is (obj, typeTable)
	if getObjectMeta(obj) == nil then
		return false
	end

	return isCompatibleTypeTable(obj, typeTable)
end

--[[
	get value while bypassing access checking
		param key: key of the value
]]
function ColaPlus.tryget (obj, key)
	if not feature_accessing_checking then
		return obj[key]
	end

	if type(obj) ~= "table" then
		argError(1, "tryget", "table", type(obj), 2)
	end

	local typeTableMeta = getTypeTableMeta(obj)
	if typeTableMeta then
		return typeTableMeta.__tryget(obj, key)
	end

	local objMeta = getObjectMeta(obj)
	if objMeta then
		return objMeta.__tryget(obj, key)
	end

	return obj[key]
end

----------------------------------------
--
-- Misc tools
--
----------------------------------------

local l_tableidmap = {}
local l_tableidUnique = 0
setmetatable(l_tableidmap, {__mode="kv"})

function ColaPlus.tableid (t)
	if type(t) ~= "table" then
		argError(1, "tableid", "table", type(t), 2)
	end

	local curid = l_tableidmap[t]
	if curid then
		return curid
	end

	local newid = l_tableidUnique + 1
	l_tableidmap[t] = newid
	l_tableidUnique = newid

	return newid
end

----------------------------------------
--
-- Finish
--
----------------------------------------

bInitializingColaPlus = false
return ColaPlus
