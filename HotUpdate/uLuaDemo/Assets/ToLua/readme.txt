tolua#
git地址: https://github.com/topameng/tolua
bug 反馈群: 286510803

如果你想在手机上测试，首先点击菜单Lua/Copy lua files to Resources， 之后再build
如果在mac上发布ios，删除x86和x86_64目录
更新插件之前，请先执行Lua/Clear wrap files，更新后再重新生成wrap文件。

1.01
- FIX: 5.x AssetBundle.Load函数废弃问题.
- FIX: 修正模版类导出命名空间问题
- FIX: pblua protobuf协议tostring卡死问题
- FIX: Array index 不再检测null参数
- FIX: LuaInteger64 重载函数匹配检测问题
- NEW: 指定RenderSettings为静态类
- NEW: LuaFunction转委托函数支持可变参数列表
- NEW: Wrap函数出错同时附加c#异常堆栈

1.02
- New: c# event +=和-=操作支持
- New: 添加 mac 和 ios 运行库
- Opt: 优化list双向链表

1.0.3(需要重新导出Wrap文件)
- FIX: 在mac unity5 luac协同异常后，unity追踪堆栈(一般是log类函数)不崩溃。(luac与unity配合问题)
- FIX: ios发布mono版本编译问题
- FIX: 模拟unity协同在使用过程中发生协同被gc的bug. 加入StartCoroutine 和 StopCoroutine 来启动或者停止这种协同
- FIX: LuaFunction递归调用自身问题
- NEW: 出错后能反映两端正确的堆栈（并且格式与unity相同,无论是c#异常还是lua异常！)
- NEW: 从LuaClient拆分出LuaLooper（负责update驱动分发）
- NEW: Lua API 接口按照lua头文件方式排序，加入所有的Lua API函数（无法兼容的除非，部分被改写来）
- NEW: 重写大量可发生异常的Lua API（native异常转换为C#异常）。
- NEW: lua 全双工协同加入 coroutine.stop 函数，请跟 coroutine.start 配合使用
- NEW: Event 改为小写 event, 增加 c# 端委托 +- LuaFunction
- NEW: Add utf-8 libs and examples
- NEW: Add cjson libs and examples
- NEW: CustomSettings.cs 加入新的静态类，以及out类链表(默认不在为每个类加.out属性, 除非out列表有这个类型）
- NEW: 加入LuaConst， 可以自定义Lua文件目录，设置后让例子环境正常运行

1.0.4 (需要重新导出Wrap文件,即使下载过这个版本也要重新导出)
- FIX: 修复遗漏的TrackedReference问题(导出问题)
- FIX: 导出wrap文件时当一个ref类型在其他非系统dll中也能正确找到。
- FIX: abstrace class 作为基类不再自动导出（默认跳过），如果需要导出，请加入到导出列表
- FIX: 如果函数名字与属性名相同（如get_Name 函数与 Name 属性），可以正确产生重载函数。
- FIX: char[] 转换问题
- FIX: int64.tonum2 符号位不对问题，int64加入范围检测
- NEW: int64 使用字符串赋值时加入溢出检查
- NEW: 修改proto-gen-lua库，使之支持int64, uint64。fixed64, ufixed64等等
- NEW: CheckTypes系列函数放入TypeChecker类
- NEW: 加入预加载库功能，预加载的库通过require类型延迟导入, 比如 require "UnityEngine.GameObject"。严格区分.与/。使用目录切勿用.
- NEW: LuaConst加入ZeroBraneStudio路径设置，可以通过LuaClient.OpenZbsDebugger启动ZeroBraneStudio调试
- NEW: print 编辑器下可以打印所在的lua文件名和位置
- NEW: this操作符增加this属性，可以通过get和set操作, 在get_Item有重载函数，并且重载函数折叠掉this属性函数可以使用
- NEW: 增加LuaByteBufferAttribute, 加上这个标记的委托类型，在压入byte[]时作为lua string压入，而不是System.Array
- Opt: 优化update系列函数速度

1.0.5 (需要重新生成库文件，重新导出wrap文件)
- NEW: loader 遵从 lua 方式，c# loader 兼容package path路径方式
- NEW: 加入静态反射，使用方法见例子22_Reflection
- NEW: 修改require, module 对于使用.和/不会作为不同文件加载。推荐用.
- NEW: 支持c# 基础类型out修饰符。需要require 'tolua.out' 来加载。
- NEW: 加入LuaRenameAttribute元属性， 对于重载折叠掉的函数，可以使用这个属性设置一个新的函数名字从而实现单独导出
- NEW: 使用一个没有require的preloading库会触发一次警告，push 没有wrap的类型，做为注册过的基类类型存入（最差是System.Object）
- NEW: 补齐一些极少用到的数组类型(如bool[]极少见)或者param数组(param string[](一般用param object[]))类型参数
- NEW: 导出支持增加扩展类型导出相应的扩展函数，支持可预知参数类型的模版函数导出。通过配置CustomSetting即可导出DoTween类库
- NEW: 支持ZeroBrandStudio调试
- NEW: luajit2.1 beta1 升级为 luajit2.1 beta2
- New: 打包lua文件名小写和u5.x一致，加入u5.x打包代码。
- FIX: 修改LuaSocket使用 git 上最新的LuaSocket版本，而不是之前的修改版
- FIX: 导出的数组支持c#所有数组函数，而不是只有[]和length
- FIX: 去掉Type类一些无法使用的函数，使用静态反射方案替代
- FIX: luaref 默认值设置为-1，调用不存在或者未初始化的的函数出错信息同lua一致
- FIX: 修改AddSearchPath方式，c#查找文件方式与lua相同，并且兼容lua修改package.path，c#与c loader搜索目录不在重叠
- FIX: 清除LuaFunction 记录堆栈数据的gc alloc
- FIX: int64作为object push check 问题
- FIX: LuaTable int key c#不做判断，按照lua标准执行或者报错
- FIX: 一些小的导出问题

1.0.6 (需要重新生成库文件，需要Clear all, 重新导出wrap)
- NEW: 加入LuaStatePtr最为LuaDLL函数简单封装层
- NEW: LuaState ToLuaException 更名为 ThrowLuaException
- NEW: Debugger 放入到 LuaInterface namespace
- NEW: module.name 如果.name不存在，可以自动进行preloading操作，相当于 require "module.name"
- NEW: 在控制台窗口点击print打印的lua log, 将会自动打开lua文件，或者跳转到设置的cs文件中
- NEW: 支持int64和uint64. c# 端long做为int64压入，ulong作为uint64压入
- NEW: 支持list和dictionary的通用导出
- NEW: list 支持数组操作符，如果Dictionary key 为int也支持，非int key 继续使用get_Item函数
- NEW: 支持委托转换LuaFunction函数，附带self。即System.Action(self.func, self)， 这样转换可自动作为:调用

- FIX: tolua_pushcclosure 调整为 tolua_pushcfunction
- FIX: userdata 访问__newindex不能存在的属性不创建peer表。如需peer表请主动创建
- FIX: 委托自动适配lua函数时，支持out参数。- 委托操作，支持自动转换函数。 
- FIX: 部分GetHashCode函数可能潜在的问题
- FIX: CheckInteger64 更名为 CheckLong
- FIX: DefaultMember("ItemOf")类导出问题
- FIX: 5.6部分编辑器函数或参数导出问题
- FIX：Object新的Instantiate函数导出问题
- FIX: int64反向计算问题
- FIX: 枚举唯一性问题
- Fix: LuaArrayTable LuaDictTable 迭代中break问题

1.0.7 (需要重新生成库文件，需要Clear all, 重新导出wrap)
- NEW: LuaState增加直接调用一个lua函数，不生成临时的LuaFunction
- NEW: LuaTable增加直接调用一个lua函数，不生成临时的LuaFunction
- NEW: 通用模板支持， LuaFunction可以写简短调用方式，LuaTable 增加Get RawGet等无GC获取
- NEW: LuaFunction可转换为DelegateFactory中注册的委托
- NEW: CheckType采用模板形式，提高了重载函数匹配速度
- NEW: 优化了Physics.RayCast调用速度, 以及Check数组优化速度外加扩充
- NEW: 增加了struct类型自行扩展机制，通过自行扩展注入到tolua系统，快速无GC转换c#类型到lua table
- NEW: luajit 升级为2.1b3, 并且极大减小在安卓上jit失败情况。
- NEW: 重载速度提升，相同参数个数，类型相同位置延迟参数类型检查
- NEW: 支持导出带有默认值的函数

- FIX: luajit不再因64位分配内存地址报not enough memory. 错误函数调用不在此列。参考http://luajit.org/status.html
- FIX: 安卓上jit失败造成卡机问题
- FIX: 在系统中Instantis对象上的脚本Awake调用LuaFunction失败，通过LuaState.ThrowLuaException时堆栈错误上报出错问题
- FIX: 修正一些lua脚本中的书写错误
- FIX: 作为object PushLayerMask问题
