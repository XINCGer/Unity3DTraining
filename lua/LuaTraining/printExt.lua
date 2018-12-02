local raw_print = print

local function printTable(t)
	if type(t) == "table" then
		for _,v in pairs(t) do
			if type(v) == "table" then
				printTable(v)
			else
				raw_print(v)
			end
		end
	end
end

local function printExt(...)
	local args = { ... }
	for _,v in pairs(args) do
		if type(v) == "table" then
			printTable(v)
		else
			raw_print(v)
		end
	end
end

rawset(_G,"print",printExt)

print(1,2,3,{123,456,{"abc"},nil})