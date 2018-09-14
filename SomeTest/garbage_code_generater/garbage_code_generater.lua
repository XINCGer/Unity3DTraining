--[[
Copyright 2017 xerysherry
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall
be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
garbage_code_generater.lua: Generate Generate CSharp Code
Author:
	xerysherry
]]

local os = os
local math = math
local io = io

math.randomseed(os.time())

local charlist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_"
local charlist_length = string.len(charlist)

local header_charlist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_"
local header_charlist_length = string.len(header_charlist)

local function getChar(chars, charscount, i)
    local idx = i % charscount + 1
    return string.sub(chars, idx, idx);
end

local function getRandomChar()
    return getChar(charlist, charlist_length, math.random(1, charlist_length))
end

local function getRandomHChar()
    return getChar(header_charlist, header_charlist_length, math.random(1, charlist_length))
end

local function getRandomName(l)
    local r = getRandomHChar()
    for _=2, l do
        r = r .. getRandomChar()
    end
    return r
end

local base_type_and_default = {
    ["bool"] = "false",
    ["int"] = "0",
    ["float"] = "0.0f",
    ["double"] = "0.0",
    ["string"] = "null"
}

local base_type_randomfunction =
{
    ["bool"] = function(a, b, c)
        local r = math.random(1, 5)
        if r == 1 then
            return {a .. " = " .. b .. " && " .. c ..";", }
        elseif r == 2 then
            return {
                "if(" .. a .. ") ",
                "{",
                "    ".. b.. " = !" .. c .. ";",
                "}",
            }
        elseif r == 3 then
            return {
                "if(" .. a .. " && " .. c .. ") ",
                "{",
                "    ".. b.. " = !" .. b .. ";",
                "}",
            }
        elseif r == 4 then
            return {
                "if(" .. a .. " || " .. b .. ") ",
                "{",
                "    ".. b.. " = !" .. b .. ";",
                "}",
            }
        elseif r == 5 then
            return {a .. " = " .. b .. " || " .. c ..";", }
        else
            return {a .. " = " .. b .. " && " .. c ..";", }
        end
    end,
    ["int"] = function(a, b, c)
        local r = math.random(1, 7)
        if r==1 then
            return {a .. " = " .. b .. " + " .. c ..";", }
        elseif r==2 then
            return {a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
                a .. " = " .. math.random(1, 100000) ..";",
                b .. " = " .. math.random(1, 100000) ..";",
                c .. " = " .. math.random(1, 100000) ..";",
            }
        elseif r==6 then
            return {
                "for(int i=0;i<"..a..";++i)",
                "{",
                "	"..b .."+=1;",
                "   "..c .."+=" .. b..";",
                "}",
            }
        else
            return {
                b .. " = " .. a .. ";",
                c .. " = " .. a .. ";",
            }
        end
    end,
    ["float"] = function(a, b, c)
        local r = math.random(1, 6)
        if r==1 then
            return {a .. " = " .. b .. " + " .. c ..";", }
        elseif r==2 then
            return {a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
                a .. " = " .. math.random(1, 10000) ..".0f;",
                b .. " = " .. math.random(1, 10000) ..".0f;",
                c .. " = " .. math.random(1, 10000) ..".0f;",
            }
        else
            return {
                b .. " = " .. a .. ";",
                c .. " = " .. a .. ";",
            }
        end
    end,
    ["double"] = function(a, b, c)
        local r = math.random(1, 6)
        if r==1 then
            return {a .. " = " .. b .. " + " .. c ..";", }
        elseif r==2 then
            return {a .. " = " .. b .. " - " .. c ..";", }
        elseif r==3 then
            return {a .. " = " .. b .. " * " .. c ..";", }
        elseif r==4 then
            return {a .. " = " .. b .. " / " .. c ..";", }
        elseif r==5 then
            return {
                a .. " = " .. math.random(1, 10000) ..".0;",
                b .. " = " .. math.random(1, 10000) ..".0;",
                c .. " = " .. math.random(1, 10000) ..".0;",
            }
        else
            return {
                b .. " = " .. a .. ";",
                c .. " = " .. a .. ";",
            }
        end
    end,
    ["string"] = function(a, b, c)
        local r = math.random(1, 3)
        if r==1 then
            return {a .. " = " .. b .. " + " .. c ..";", }
        elseif r==2 then
            return {
                a .. " = string.Format("..c ..","..b..");",
            }
        else
            return {
                b .. " = " .. a .. ";",
                c .. " = " .. a .. ";",
            }
        end
    end,
}

local base_type_list = nil
local function getRandomType()
    if base_type_list == nil then
        base_type_list = {}
        for k, _ in pairs(base_type_and_default) do
            table.insert(base_type_list, k)
        end
    end
    local r = math.random(1, #base_type_list)
    local t = base_type_list[r]
    return t, base_type_and_default[t]
end
local function getRandomPublic()
    local r = math.random(1,3)
    if r==1 then
        return "public"
    elseif r==2 then
        return "private"
    else
        return ""
    end
end

local property_info =
{
    p = "",
    name = "default",
    t = "int",
    v = "0",
}
function PropertyGenerate(p, n, t, v)
    local ta = setmetatable({},
        {
            __index = property_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.t = t
    ta.v = v
    return ta
end

local attribute_info =
{
    p = "",
    name = "default",
    t = "int",
    property_info = nil,
}
function AttributeGenerate(p, n, t, pi)
    local ta = setmetatable({},
        {
            __index = attribute_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.property_info = pi
    if pi~=nil then
        ta.t = pi.t
    end
    return ta
end

local method_info =
{
    p = "",

    name = "default",
    --[[
        {{"type1", "name1"}, {"type2", "name2"}, ..}
    ]]
    params = nil,
    --[[
        {"type", "value"}
    ]]
    retn = nil,

    content = nil,
}
function MethodContentGenerate(ci)

    content = {}

    local function getRandomP(t)
        return t[math.random(1, #t)]
    end

    for i=1, 10 do
        local t = getRandomType()
        local m = ci.typemap[t]
        local f = base_type_randomfunction[t]
        if #m > 0 then
            local a = getRandomP(m)
            local c = f(a, getRandomP(m),getRandomP(m))
            for _, v in ipairs(c) do
                table.insert(content, v)
            end
        end
    end

    return content
end

function MethodGenerate(ci, p, n, r)
    local ta = setmetatable({},
        {
            __index = method_info,
        })

    ta.p = p or ta.p
    ta.name = n
    ta.retn = r
    ta.content = MethodContentGenerate(ci)
    return ta
end

local class_info =
{
    p = "",
    name = "default",
    implement = nil,

    properties = nil,
    attributes = nil,

    typemap = nil,
}

local classes = {}

local classnames = {}
function GetNextClassName(min, max)
    local r = math.random(min or 10, max or 10)
    while true do
        local n = getRandomName(r)
        if classnames[n] == nil then
            classnames[n] = true
            return n
        end
    end
end

--[[
- 类生成函数
- 参数
	mc: 函数数量
	pc: 变量数量
	ac: 属性数量
]]
function ClassGenerater(mc, pc, ac)
    local class = setmetatable({},
        {
            __index = class_info,
        })

    local usedname = {}
    function GetNextName(min, max)
        local r = math.random(min or 10, max or 10)
        while true do
            local n = getRandomName(r)
            if usedname[n] == nil then
                usedname[n] = true
                return n
            end
        end
    end

    class.name = GetNextClassName(10, 40)
    usedname[class.name] = true;
    --class.implement = "MonoBehaviour"

    class.properties = {}
    for i=1, pc or 20 do
        table.insert(class.properties,
            PropertyGenerate(
                getRandomPublic(),
                GetNextName(10, 40),
                getRandomType()
            ))
    end

    class.attributes = {}
    for i=1, ac or 20 do
        local t, v = getRandomType()
        local pi = nil
        if math.random(10) < 3 then
            pi = class.properties[math.random(#class.properties)]
        end
        table.insert(class.attributes,
            AttributeGenerate(
                "public",
                GetNextName(10, 40),
                t, pi
            ))
    end

    class.typemap = {}
    for _, v in ipairs(base_type_list) do
        class.typemap[v] = {}
    end
    for _, v in ipairs(class.properties) do
        table.insert(class.typemap[v.t], v.name)
    end
    for _, v in ipairs(class.attributes) do
        table.insert(class.typemap[v.t], v.name)
    end

    class.methods = {}
    for i=1, mc or 20 do
        local t, v = getRandomType()
        local pi = nil
        if math.random(10) < 3 then
            pi = class.properties[math.random(#class.properties)]
        end
        table.insert(class.methods,
            MethodGenerate(class,
                "public",
                GetNextName(10, 40)
            ))
    end

    return class
end

function GetClassSource(class)

    local r = ""
    local t = 0
    local s = function(str)
        for i=1, t do
            r = r.."    "
        end
        r = r..str.."\r\n"
    end
    local _property = function(p)
        s(p.p .. " " .. p.t .. " " .. p.name .. " = " .. p.v .. ";")
    end
    local _attribute = function(a)
        s(a.p .. " " .. a.t .. " " .. a.name)
        s "{"
        t = t +1

        if a.property_info then
            local p = a.property_info
            s("get { return " .. p.name .. "; }")
            s("set { " .. p.name .. " = value; }")
        else
            s("get;")
            s("set;")
        end

        t = t -1
        s "}"
    end
    local _method = function(m)
        local ty = "void"
        if m.retn ~= nil then
            ty = m.retn[1]
        end
        local p = "()"
        if m.params ~= nil then
            p = ""
            for _, v in ipairs(m.params) do
                if string.len(p) > 0 then
                    p = p .. "," .. v[1] .. " " .. v[2]
                else
                    p = v[1] .. " " .. v[2]
                end
            end
            p = "(" .. p .. ")"
        end

        s(m.p.." ".. ty .." ".. m.name .. p)
        s"{"
        t = t+1

        for _, v in ipairs(m.content) do
            s(v)
        end
        t = t-1
        s"}"
    end

    s(class.p .. " class "..class.name..((class.implement~=nil and (" : "..class.implement)) or ""))
    s"{"
    t = t+1

    if class.properties ~= nil then
        for _, p in ipairs(class.properties) do
            _property(p)
        end
    end
    if class.attributes ~= nil then
        for _, p in ipairs(class.attributes) do
            _attribute(p)
        end
    end
    if class.methods ~= nil then
        for _, m in ipairs(class.methods) do
            _method(m)
        end
    end
    t = t-1
    s"}"

    return r
end

local function precent_bar(v, l)
    local p = {".", "-", "="}
    local pl = #p
    local line = ""
    local per = v
    local s = per*l
    local n = math.floor(s)
    local ns = math.floor((s - n) * pl)+1
    for i=1, l do
        if i<n+1 then
            line = line .. p[pl]
        elseif i==n+1 then
            line = line .. p[ns]
        else
            line = line .. p[1]
        end
    end
    return line
end

local help =
[[C# Garbage Code Generater
	-h: Help
	-o: Output Dir, Default="garbate"
	-c: Generate Class Count, Default=400
	-mc: Method Count, Default=30
	-pc: Property Count, Default=30
	-ac: Attribute Count, Default=30
	-verbose: Print Every Generate Class Name
	-q: QuietMode, Print Nothing
]]
function main()
    local name = "garbage"
    local class_count = 400
    local method_count = 30
    local property_count = 30
    local attribute_count = 30
    local verbose = false
    local quiet = false

    if arg then
        local len = #arg
        for i=1, len do
            local v = arg[i]
            if v == "--h" or v == "--H" or
                    v == "-h" or v == "-H" or v == "?" then
                print(help)
                return
            elseif (v == "-c" or v == "--c") and i < len then
                i = i+1
                class_count = 0 + arg[i]
            elseif (v == "-o" or v == "--o") and i < len then
                i = i+1
                name = arg[i]
            elseif (v == "-mc" or v == "--mc") and i < len then
                i = i+1
                method_count = 0 + arg[i]
            elseif (v == "-pc" or v == "--pc") and i < len then
                i = i+1
                property_count = 0 + arg[i]
            elseif (v == "-ac" or v == "--ac") and i < len then
                i = i+1
                attribute_count = 0 + arg[i]
            elseif v == "-verbose" then
                verbose = true
            elseif v == "-q" or v == "-quiet" then
                quiet = true
            end
        end
    end

    if not quiet then
        print("output = " .. name)
        print("class count = " .. class_count)
        print("method count = " .. method_count)
        print("property count = " .. property_count)
        print("attribute count = " .. attribute_count)
    end

    local path = name .. "/"
    pcall(function()
        os.execute("mkdir "..name)
    end)

    for i=1, class_count do
        local class = ClassGenerater(method_count, property_count, attribute_count)
        os.remove("test.cs")
        local f = io.open(path .. class.name .. ".cs", "wb")
        f:write(GetClassSource(class))
        f:close()

        if not quiet then
            if verbose then
                print(class.name .. ".cs")
            else
                io.write("\013".. "|"..precent_bar(i/class_count, 60) .. "| " .. string.format("%5d/%5d", i, class_count))
            end
        end
    end
    if not quiet then
        print("\nFin!")
    end
end

main()