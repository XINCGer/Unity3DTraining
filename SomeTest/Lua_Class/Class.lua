--类的声明，这里声明了类名还有属性，并且给出了属性的初始值
Class = {x=0,y=0}
--设置元表的索引，想模拟类的话，这步操作很关键
Class.__index = Class
--构造方法，构造方法的名字是随便起的，习惯性命名为new()
function Class:new(x,y)
	 local self = {}  --初始化self，如果没有这句，那么类所建立的对象如果有一个改变，其他对象都会改变
	 setmetatable(self, Class)  --将self的元表设定为Class
	 self.x = x   --属性值初始化
	 self.y = y
	 return self  --返回自身
end

--这里定义类的其他方法
function Class:test()
 print(self.x,self.y)
end

function Class:plus()
 self.x = self.x + 1
 self.y = self.y + 1
end