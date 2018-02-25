require 'Class'
require 'SubClass'

local a = Class:new() -- 首先实例化父类的对象，并调用父类中的方法
a:plus()
a:test()

a = SubClass:new()	-- 然后实例化子类对象
a:plus()			-- 子类对象可以访问到父类中的成员和方法
a:go()				-- 子类对象调用子类中的新增方法
a:test()			-- 子类对象调用重写的方法