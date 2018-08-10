--类的声明，这里声明了类名还有属性，并且给出了属性的初始值
Class = {}
--设置元表的索引，想模拟类的话，这步操作很关键
Class.__index = Class
--构造方法，构造方法的名字是随便起的，习惯性命名为new()
function Class:new()
	 local self = {}  --初始化self，如果没有这句，那么类所建立的对象如果有一个改变，其他对象都会改变
	 setmetatable(self, Class)  --将self的元表设定为Class
	 return self  --返回自身
end

-- todo:可以新增一些基类方法