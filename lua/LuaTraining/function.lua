--以下实例定义了函数 max()，参数为 num1, num2，用于比较两值的大小，并返回最大值
function max(num1, num2)

   if (num1 > num2) then
      result = num1;
   else
      result = num2;
   end

   return result; 
end
-- 调用函数
print("Max is: ",max(10,4))
print("Min is: ",max(5,6))

--Lua 中我们可以将函数作为参数传递给函数
myprint = function(param)
   print("This is print fucntion -   ##",param,"##")
end

function add(num1,num2,functionPrint)
   result = num1 + num2
   -- 调用传递的函数参数
   functionPrint(result)
end
myprint(10)
-- myprint 函数作为参数传递
add(2,5,myprint)

--Lua函数可以返回多个结果值，比如string.find
--其返回匹配串"开始和结束的下标"（如果不存在匹配串返回nil）
s, e = string.find("www.runoob.com", "runoob") 
print(s, e)  -->5 10

--Lua函数中，在return后列出要返回的值得列表即可返回多值
function maximum (a)
    local mi = 1             -- 最大值索引
    local m = a[mi]          -- 最大值
    for i,val in ipairs(a) do
       if val > m then
           mi = i
           m = val
       end
    end
    return m, mi
end

print(maximum({8,10,23,12,5}))

--[[
Lua函数可以接受可变数目的参数，和C语言类似在函数参数列表中使用三点（...) 表示函数有可变的参数
Lua将函数的参数放在一个叫arg的表中，#arg 表示传入参数的个数
]]--

function average(...)
   result = 0
   local arg={...}
   for i,v in ipairs(arg) do
      result = result + v
   end
   print("Total args count : " .. #arg)
   return result/#arg
end

print("Average is: ",average(10,5,3,4,5,6))