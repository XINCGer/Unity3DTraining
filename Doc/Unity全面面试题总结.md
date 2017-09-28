## Unity全面的面试题（包含答案）  

### 什么是协同程序？  
> 在主线程运行的同时开启另一段逻辑处理，来协助当前程序的执行，协程很像多线程，但是不是多线程，Unity的协程实在每帧结束之后去检测yield的条件是否满足。  

### Unity3d中的碰撞器和触发器的区别？  
> 碰撞器是触发器的载体，而触发器只是碰撞器身上的一个属性。当Is Trigger=false时，碰撞器根据物理引擎引发碰撞，产生碰撞的效果，可以调用OnCollisionEnter/Stay/Exit函数；当Is Trigger=true时，碰撞器被物理引擎所忽略，没有碰撞效果，可以调用OnTriggerEnter/Stay/Exit函数。如果既要检测到物体的接触又不想让碰撞检测影响物体移动或要检测一个物件是否经过空间中的某个区域这时就可以用到触发器  

### 物体发生碰撞的必要条件？  
> 两个物体都必须带有碰撞器（Collider），其中一个物体还必须带有Rigidbody刚体，而且必须是运动的物体带有Rigidbody脚本才能检测到碰撞。  

### 请简述ArrayList和List的主要区别？  
> ArrayList存在不安全类型（ArrayList会把所有插入其中的数据都当做Object来处理），装箱拆箱的操作（费时），List是泛型类，功能跟ArrayList相似，但不存在ArrayList所说的问题  

### 如何安全的在不同工程间安全地迁移asset数据？三种方法   
> * 将Assets目录和Library目录一起迁移  
> * 导出包，export Package  
> * 用unity自带的assets Server功能  

### OnEnable、Awake、Start运行时的发生顺序？哪些可能在同一个对象周期中反复的发生  
> Awake –>OnEnable->Start，OnEnable在同一周期中可以反复地发生。  

### MeshRender中material和sharedmaterial的区别？  
> 修改sharedMaterial将改变所有物体使用这个材质的外观，并且也改变储存在工程里的材质设置。不推荐修改由sharedMaterial返回的材质。如果你想修改渲染器的材质，使用material替代。  

### Unity提供了几种光源，分别是什么  
> 四种。  
>* 平行光：Directional Light  
>* 点光源：Point Light  
>* 聚光灯：Spot Light  
>* 区域光源：Area Light  

### 简述一下对象池，你觉得在FPS里哪些东西适合使用对象池  
> 对象池就存放需要被反复调用资源的一个空间，当一个对象回大量生成的时候如果每次都销毁创建会很费时间，通过对象池把暂时不用的对象放到一个池中（也就是一个集合），当下次要重新生成这个对象的时候先去池中查找一下是否有可用的对象，如果有的话就直接拿出来使用，不需要再创建，如果池中没有可用的对象，才需要重新创建，利用空间换时间来达到游戏的高速运行效果，在FPS游戏中要常被大量复制的对象包括子弹，敌人，粒子等  

### CharacterController和Rigidbody的区别  
> Rigidbody具有完全真实物理的特性，Unity中物理系统最基本的一个组件，包含了常用的物理特性，而CharacterController可以说是受限的的Rigidbody，具有一定的物理效果但不是完全真实的，是Unity为了使开发者能方便的开发第一人称视角的游戏而封装的一个组件  

### 简述prefab的用处  
> 在游戏运行时实例化，prefab相当于一个模板，对你已经有的素材、脚本、参数做一个默认的配置，以便于以后的修改，同时prefab打包的内容简化了导出的操作，便于团队的交流  

### 请简述sealed关键字用在类声明时与函数声明时的作用  
> sealed修饰的类为密封类，类声明时可防止其他类继承此类，在方法中声明则可防止派生类重写此方法  

### 请简述private，public，protected，internal的区别  
>* public：对任何类和成员都公开，无限制访问  
>* private：仅对该类公开  
>* protected：对该类和其派生类公开  
>* internal：只能在包含该类的程序集中访问该类  

### 使用Unity3d实现2d游戏，有几种方式？  
>* 使用本身的GUI，在Unity4.6以后出现的UGUI  
>* 把摄像机的Projection(投影)值调为Orthographic(正交投影)，不考虑z轴；  
>* 使用2d插件，如：2DToolKit，和NGUI  

### 在物体发生碰撞的整个过程中，有几个阶段，分别列出对应的函数  
> 三个阶段，1.OnCollisionEnter 2.OnCollisionStay 3.OnCollisionExit  

### Unity3d的物理引擎中，有几种施加力的方式，分别描述出来  
> rigidbody.AddForce/AddForceAtPosition，都在rigidbody系列函数中。大家可以自己去查看一下rigidbody的API  

### 什么叫做链条关节？  
> Hinge Joint，可以模拟两个物体间用一根链条连接在一起的情况，能保持两个物体在一个固定距离内部相互移动而不产生作用力，但是达到固定距离后就会产生拉力  
### Unity3d提供了一个用于保存和读取数据的类(PlayerPrefs)，请列出保存和读取整形数据的函数  
> PlayerPrefs.SetInt() PlayerPrefs.GetInt()  

### Unity3d脚本从唤醒到销毁有着一套比较完整的生命周期，请列出系统自带的几个重要的方法  
> Awake——>OnEnable–>Start——>Update——>FixedUpdate——>LateUpdate——>OnGUI——>OnDisable——>OnDestroy  


