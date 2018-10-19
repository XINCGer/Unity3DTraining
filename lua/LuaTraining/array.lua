array = {"Lua", "Tutorial"}

for i= 0, 2 do
   print(array[i])
end

--还可以以负数为数组索引值
array = {}

for i= -2, 2 do
   array[i] = i *2
end

for i = -2,2 do
   print(array[i])
end

--多维数组即数组中包含数组或一维数组的索引键对应一个数组
--以下是一个三行三列的阵列多维数组
-- 初始化数组
array = {}
for i=1,3 do
   array[i] = {}
      for j=1,3 do
         array[i][j] = i*j
      end
end

-- 访问数组
for i=1,3 do
   for j=1,3 do
      print(array[i][j])
   end
end

--不同索引键的三行三列阵列多维数组
-- 初始化数组
array = {}
maxRows = 3
maxColumns = 3
for row=1,maxRows do
   for col=1,maxColumns do
      array[row*maxColumns +col] = row*col
   end
end

-- 访问数组
for row=1,maxRows do
   for col=1,maxColumns do
      print(array[row*maxColumns +col])
   end
end

--正如你所看到的，以上的实例中，数组设定了指定的索引值
--这样可以避免出现 nil 值，有利于节省内存空间