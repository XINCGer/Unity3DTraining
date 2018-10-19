string1 = "Lua"
print("\"String1 is\"",string1)
string2 = 'www.baidu.com'
print("String2 is",string2)

string3 = [["Lua is good!"]]
print("String2 is",string3)

--[[
string.gsub(mainString,findString,replaceString,num)
在字符串中替换,mainString为要替换的字符串， findString 为被替换的字符
replaceString 要替换的字符，num 替换次数（可以忽略，则全部替换）
]]--

print(string.gsub("aaaa","a","z",3))

--[[
string.strfind (str, substr, (init,end))
在一个指定的目标字符串中搜索指定的内容(第三个参数为索引),返回其具体位置。不存在则返回 nil
]]--

print(string.find("Hello Lua user", "Lua",1))

string1 = "Lua";
print(string.upper(string1))
print(string.lower(string1))

string = "Lua Tutorial"
-- 查找字符串
print(string.find(string,"Tutorial"))
reversedString = string.reverse(string)
print("New String is",reversedString)


string1 = "Lua"
string2 = "Tutorial"
number1 = 10
number2 = 20
-- 基本字符串格式化
print(string.format("Base Format %s %s",string1,string2))
-- 日期格式化
date = 2; month = 1; year = 2014
print(string.format("Date Format %02d/%02d/%03d", date, month, year))
-- 十进制格式化
print(string.format("%.4f",1/3))


-- 字符转换
-- 转换第一个字符
print(string.byte("Lua"))
-- 转换第三个字符
print(string.byte("Lua",3))
-- 转换末尾第一个字符
print(string.byte("Lua",-1))
-- 第二个字符
print(string.byte("Lua",2))
-- 转换末尾第二个字符
print(string.byte("Lua",-2))

-- 整数 ASCII 码转换为字符
print(string.char(97))

string1 = "www."
string2 = "baidu"
string3 = ".com"
-- 使用 .. 进行字符串连接
print("Connect String:",string1..string2..string3)

-- 字符串长度
print("Length of String: ",string.len(string2))

-- 字符串复制 2 次
repeatedString = string.rep(string2,2)
print(repeatedString)