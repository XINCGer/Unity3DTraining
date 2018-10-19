--Lua提供了一个名为require的函数用来加载模块。要加载一个模块，只需要简单地调用就可以了
-- require("<模块名>") or require "<模块名>"
--执行 require 后会返回一个由模块常量或函数组成的 table，并且还会定义一个包含该 table 的全局变量
-- test_module.lua 文件
-- module 模块为上文提到到 module.lua
require("module")
 
print(module.constant)
 
module.func3()

--或者给加载的模块定义一个别名变量，方便调用
-- test_module2.lua 文件
-- module 模块为上文提到到 module.lua
-- 别名变量 m
local m = require("module")
 
print(m.constant)
 
m.func3()

