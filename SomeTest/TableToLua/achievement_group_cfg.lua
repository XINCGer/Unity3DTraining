local config_predefine_pb = require 'Protol.config_predefine_pb'
local _fidx = {}
_fidx.id = 1
_fidx.name = 2
_fidx.achieve_children = 3
local _data = {
{100,[[神装打造]],{10001,10002,10003,10004}},
{200,[[伙伴养成]],{20001,20002}},
{201,[[魔力进阶]],{10001,10002}},
{202,[[神装打造]],{10001,10002}},
{203,[[伙伴养成]],{10001,10002}},
{204,[[魔力进阶]],{10001,10002}},
{205,[[神装打造]],{10001,10002}},
{206,[[伙伴养成]],{10001,10002}},
{207,[[魔力进阶]],{10001,10002}},}
local mt = {}
mt.__index = function(a,b)
	if _fidx[b] then
		return a[_fidx[b]]
	end
	return nil
end
mt.__newindex = function(t,k,v)
	error('do not edit config')
end
mt.__metatable = false
for _,v in ipairs(_data) do
	setmetatable(v,mt)
end
return _data