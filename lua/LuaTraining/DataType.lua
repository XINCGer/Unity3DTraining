print ("2"+6)
print ("1"+"8")
print ("abc" .. "def")
print (123 .. 456)

print("\n")
len ="www.baidu.com"
print (#len)
print (#"Linux")

print("\n")
--创建一个空table
local tb1 ={}
--直接初始化table
local tb2 ={"a","b","c"}
a ={}
a["key"]="value"
key =10;
a[key]=22
a[key]= a[key]+11
for k,v in pairs(a) do 
    print (k .. ":" .. v)
end

print("\n")
--lua Table的索引一般以1开始而不是0
local tbl = {"apple", "pear", "orange", "grape"}
for key, val in pairs(tbl) do
    print("Key", key)
end

print("\n")
--table 不会固定长度大小，有新数据添加时 table 长度会自动增长，没初始的 table 都是 nil
a3 = {}
for i = 1, 10 do
    a3[i] = i
end
a3["key"] = "val"
print(a3["key"])
print(a3["none"])

--在 Lua 中，函数是被看作是"第一类值（First-Class Value）"，函数可以存在变量里
print("\n")
function factorial1(n)
    if n == 0 then
        return 1
    else
        return n * factorial1(n - 1)
    end
end
print(factorial1(5))
factorial2 = factorial1
print(factorial2(5))

--function 可以以匿名函数（anonymous function）的方式通过参数传递:
print("\n")
function anonymous(tab, fun)
    for k, v in pairs(tab) do
        print(fun(k, v))
    end
end
tab = { key1 = "val1", key2 = "val2" }
anonymous(tab, function(key, val)
    return key .. " = " .. val
end)