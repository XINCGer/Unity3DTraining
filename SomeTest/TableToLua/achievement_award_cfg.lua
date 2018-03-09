local config_predefine_pb = require 'Protol.config_predefine_pb'
local _fidx = {}
_fidx.id = 1
_fidx.reach_point = 2
_fidx.reward_id = 3
local _data = {
{1,100,10022301},
{2,200,10022301},
{3,300,10022301},
{4,400,10022301},
{5,500,10022301},
{6,600,10022301},
{7,700,10022301},
{8,800,10022301},
{9,900,10022301},
{10,1000,10022301},
{11,1100,10022301},
{12,1200,10022301},
{13,1300,10022301},
{14,1400,10022301},
{15,1500,10022301},
{16,1600,10022301},
{17,1700,10022301},
{18,1800,10022301},
{19,1900,10022301},
{20,2000,10022301},}
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