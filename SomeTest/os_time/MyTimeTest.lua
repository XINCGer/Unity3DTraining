print("=================os.time test===============")
print(os.time())
print(os.time({day=26,month=4,year=2018}))
print(os.date("%y/%m/%d, %H:%M:%S",os.time({day=26,month=4,year=2018})))
print("=================os.date test===============")
print(os.date())
print(os.date("%x",os.time()))
print(os.date("*t"))
print(os.date("*t").year)
print(os.date("*t").month)
print(os.date("*t").day)
print(os.date("*t").hour)
print(os.date("*t").wday)
-- 显示当前年份
print(os.date("%Y"))
-- 显示当前是一年中的第几周
print(os.date("%U"))
-- 组合格式化时间
print(os.date("%Y-%m-%d, %H:%M:%S",os.time()))