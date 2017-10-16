--
-- strict.lua
-- checks uses of undeclared global variables
-- All global variables must be 'declared' through a regular assignment
-- (even assigning nil will do) in a main chunk before being used
-- anywhere or assigned to inside a function.
--
-- modified for better compatibility with LuaJIT, see:
-- http://www.freelists.org/post/luajit/strictlua-with-stripped-bytecode

local getinfo, error, rawset, rawget = debug.getinfo, error, rawset, rawget

local mt = getmetatable(_G)
if mt == nil then
  mt = {}
  setmetatable(_G, mt)
end

mt.__declared = {}

mt.__newindex = function (t, n, v)
  if not mt.__declared[n] then
    local info = getinfo(2, "S")
    if info and info.linedefined > 0 then
      error("assign to undeclared variable '"..n.."'", 2)
    end
    mt.__declared[n] = true
  end
  rawset(t, n, v)
end

mt.__index = function (t, n)
  if not mt.__declared[n] then
    local info = getinfo(2, "S")
    if info and info.linedefined > 0 then
      error("variable '"..n.."' is not declared", 2)
    end
  end
  return rawget(t, n)
end
