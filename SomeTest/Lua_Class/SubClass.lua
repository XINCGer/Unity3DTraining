require 'Class'

--声明了新的属性Z  
SubClass = {z = 0}  
--设置类型是Class  
setmetatable(SubClass, Class)  
--还是和类定义一样，表索引设定为自身  
SubClass.__index = SubClass  
--这里是构造方法
function SubClass:new(x,y,z)  
   local t = {}             --初始化对象自身  
   t = Class:new(x,y)       --将对象自身设定为父类，这个语句相当于其他语言的super 或者 base
   setmetatable(t, SubClass)    --将对象自身元表设定为SubClass类  
   t.z= z                   --新的属性初始化，如果没有将会按照声明=0  
   return t  
end  

--定义一个新的方法  
function SubClass:go()  
   self.x = self.x + 10  
end  

--重定义父类的方法，相当于override
function SubClass:test()  
     print(self.x,self.y,self.z)  
end  