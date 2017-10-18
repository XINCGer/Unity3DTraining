function LuaFunction(name)
	CSFunctionTest.CSFunction(name)
end

function LuaAdd(number)
	print('lua'..number)
	CSFunctionTest.CSAdd(number)
end