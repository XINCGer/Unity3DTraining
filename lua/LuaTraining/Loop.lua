-- While
a=10
while( a < 20 )
do
   print("a Value:", a)
   a = a+1
end

-- 数值for循环，var从exp1变化到exp2
-- 每次变化以exp3为步长递增var，并执行一次"执行体"。exp3是可选的，如果不指定，默认为1
for i=1,f(x) do
    print(i)
end
 
for i=10,1,-1 do
    print(i)
end

-- for的三个表达式在循环开始前一次性求值
-- 以后不再进行求值。比如上面的f(x)只会在循环开始前执行一次，其结果用在后面的循环中
function f(x)  
    print("function")  
    return x*2   
end  
for i=1,f(5) do print(i)  
end 

-- 泛型for循环通过一个迭代器函数来遍历所有值，类似java中的foreach语句
-- 
--打印数组a的所有值  
for i,v in ipairs(a) 
	do print(v) 
end  
-- i是数组索引值，v是对应索引的数组元素值。ipairs是Lua提供的一个迭代器函数，用来迭代数组

-- repeat until
--[ 变量定义 --]
a = 10
--[ 执行循环 --]
repeat
   print("a Value:", a)
   a = a + 1
until( a > 15 )

-- break
--[ 定义变量 --]
a = 10

--[ while 循环 --]
while( a < 20 )
do
   print("a 的值为:", a)
   a=a+1
   if( a > 15)
   then
      --[ 使用 break 语句终止循环 --]
      break
   end
end