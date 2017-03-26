--[[
迭代器（iterator）是一种对象，它能够用来遍历标准模板库容器中的部分或全部元素
每个迭代器对象代表容器中的确定的地址
在Lua中迭代器是一种支持指针类型的结构，它可以遍历集合的每一个元素
]]--

array = {"Lua", "Tutorial"}

for key,value in ipairs(array) 
do
   print(key, value)
end

--使用一个简单的函数来实现迭代器，实现 数字 n 的平方
function square(iteratorMaxCount,currentNumber)
   if currentNumber<iteratorMaxCount
   then
      currentNumber = currentNumber+1
   return currentNumber, currentNumber*currentNumber
   end
end

for i,n in square,3,0
do
   print(i,n)
end

--迭代的状态包括被遍历的表（循环过程中不会改变的状态常量）和当前的索引下标（控制变量）
--ipairs和迭代函数都很简单，我们在Lua中可以这样实现
function iter (a, i)
    i = i + 1
    local v = a[i]
    if v then
       return i, v
    end
end
 
function ipairs (a)
    return iter, a, 0
end

array = {"Lua", "Tutorial"}

function elementIterator (collection)
   local index = 0
   local count = #collection
   -- 闭包函数
   return function ()
      index = index + 1
      if index <= count
      then
         --  返回迭代器的当前元素
         return collection[index]
      end
   end
end

for element in elementIterator(array)
do
   print(element)
end