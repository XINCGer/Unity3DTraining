##UNet功能练习  
##UNet局域网射击小游戏  

>* 开发环境  
Unity5.5.0f3+vs2013  
>*  预览  
![](https://github.com/XINCGer/Unity3DTraining/blob/master/UNetTraining/Previews/Previews1.png)  
![](https://github.com/XINCGer/Unity3DTraining/blob/master/UNetTraining/Previews/Previews2.png)  
>* Win版下载  
[点我下载](http://pan.baidu.com/s/1i4ZT2Zz)密码：hoqh   

##Unity3D UNet网络组件详解

###UNet常见概念简介

* Spawn:简单来说，把服务器上的GameObject，根据上面的NetworkIdentity组件找到对应监视连接，在监视连接里生成相应的GameObject.

* Command:客户端调用，服务器执行，这样客户端调用的参数必需要UNet可以序列化，这样服务器在执行时才能把参数反序列化。需要注意，在客户端需要有权限的NetworkIdentity组件才能调用Command命令。

* ClientRpc:服务端调用，客户端执行，同上，服务端的参数序列化到客户端执行，一般来说，服务端会找到上面的NetworkIdentity组件，确定那些客户端在监视这个NetworkIdentity，Rpc命令会发送给所有的监视客户端。

* Server/ServerCallback:只在服务器端运行，Callback是Unity内部函数。

* Client/ClientCallback:同上，只在客户端运行，Callback是Unity内部函数。

* SyncVar:服务器的值能自动同步到客户端，保持客户端的值与服务器一样。客户端值改变并不会影响服务器的值。  

　　上面的大部分特性都会转化成相应的MsgType,其中服务器调用，客户端执行对应MsgType有如Spawn,ClientRpc,SyncVar对应的MsgType分别为ObjectSpawn,Rpc,UpdateVars,这些都是NetworkServer调用，客户端得到相应消息，执行相应方法。客户端调用，服务器执行的MsgType有如Command,客户端发送，服务器检测到相应消息后执行。
　　

###UNet主要类介绍

* NetworkIdentity组件介绍：网络物体最基本的组件，客户端与服务器确认是否是一个物体(netID)，也用来表示各个状态，如是否是服务器，是否是客户端，是否有权限，是否是本地玩家等。一个简单例子，A是Host(又是服务器，又是客户端），B是一个Client,A与B分别有一个玩家PlayA与PlayB.在机器A上，playA与playB isServer为true,isClent为true,其中playA有权限，是本地玩家，B没权限，也不是本地玩家。在机器B上，playA与playB isServer为false,isClent为true,其中playB有权限，是本地玩家，A没权限，也不是本地玩家。A与B上的PlayA的netID相同，A与B上的PlayB的netID也相同，其中netID用来表示他们是同一网络物体在不同的机器上。在下面用网络物体来表示带有NetworkIdentity组件的GameObject.
* NetworkConnection:定义一个客户端与服务器的连接，包含当前客户端监视那些服务器上的网络物体，以及封装发送和接收到服务器的消息。

* NetworkClient：主要持有当前NetworkConnection对象与所有NetworkClient列表的静态对象，处理一些默认客户端的消息。

* 网络物体上的监视者就是一个或多个NetworkConnection，用来表示一个或多个客户端对这个网络物体保持监视，那么当这个网络物体在服务器上更新后，会自动更新对所有监视者的对应的网络物体。

* NetworkScene:简单来说，1Server与Client需要维护一个网络物体列表，Server可以遍历所有网络物体发送消息等，并且维持Server与Client上的网络物体保持同步，并且客户端记录需要注册的prefab列表.其中NetworkServer与ClientScene都包含一个NetworkScene对象，引用网络物体列表。

* NetworkServer:主要持有一个NetworkScene并且做一些只有在服务器上才能对网络服务做的事，如spawn, destory等。以及维护所有客户端连接。

* ClientScene:主要持有一个静态NetworkScene对象，用于注册网络物体的prefab列表,以及客户端场景上已经有的网络物体列表，处理SyncVar,Rpc,SyncEvent特性等，还有以及ObjectSpawn,objectDestroy,objectHide消息等。

###UNet用时想到的问题

* 问题1 spawn发生了什么，客户端为什么要注册相应的prefab.  
  当服务器spawn一个网络物体时,网络物体调用OnStartServer,分配netID.并注册到相应服务器上的的NetworkScene的网络物体列表中，更新如isServer为true等信息。   
  查找所有客户端连接，查看每个客户端连接是否需要监视这个网络物体，如果为true,那么给这个客户端上一个消息MsgType.ObjectSpawn或是MsgType.ObjectSpawnScene(这种一般是服务场景变换后自动调用），并传递上面的netID.  
  当客户端接受到ObjectSpawn消息，会在注册的prefab里查找，查找到后Instantiate个网络物体，当接受到ObjectSpawnScene时，会在场景里查找这个网络物体，然后都注册到ClientScene里的NetworkScene的网络物体列表中，并更新netID与服务器的一样。更新如isClent为true等信息。  
  我们手动spawn一个物体时，调用的是ObjectSpawn消息，客户端接到这个消息处理得到一个assetID，我们要根据prefabe实例一个新对象，只有客户端注册了相应的prefabe信息才能根据对应的assetID找到prefabe.  

* 问题2 NetworkIdentity的netID表示什么，那个时候分配。  

　　当服务器与客户端的netID相同，表示他们是同一物体，相应标示如SyncVar，服务器变了，对应客户端上相同的netID的网络物体，更新成服务器上的数据，Rpc,Commandg 一般也是相同的netID之间调用。

　　分配一般发生在服务器spawn一个网络物体时，网络物体调用OnStartServer时发生产生netID。

　　在客户端接受相应的ObjectSpawn消息，会把服务器上的对应物体的netID传递过来，产生新的网络物体并赋这个netID。

* 问题3 NetworkIdentity的sceneID是什么，在场景里已经有NetworkIdentity组件的物体是如何在客户端与服务器联系的。

　　当网络物体并不是spawn产生在服务器与客户端，而是在服务器与客户端场景本身就有时，我们也需要在服务器与客户端之间建立联系，这种物体会有一个sceneID来标示，这种模型一般是服务器场景变换完成后，NetworkServer调用spawnObjects会把这种网络物体与所有客户端同步，当spawn完成后过后，相应客户端会产生一个和服务端相同的netID。

* 问题4 服务器场景切换后，各个NetworkIdentity组件的物体如何与客户端联系。

　　如下顺序因为有异步操作，并不能确定，如下顺序只是一般可能的顺序。

   服务器异步调用场景，发送给所有客户端开始切换场景。MsgType.Scene

   客户端接受MsgType.Scene,开始切换场景。

   服务器场景完成，会查找所有的网络物体，然后spawn这些网络物体，这样各个网络物体通过相同的netID联系起来。

   客户端场景完成后,再次调用OnClientConnect,一般来说，不执行任何操作。

* 问题5 客户端为什么要网络物体的权限，它有了权限能做什么。

　　一般来说，当spawn某个服务器上的网络物体后，服务器有它的权限，客户端并不能更改这个网络物体，或是说更改这个网络物体相应的属性后并不能同步到服务器和别的客户端上，只是本机上能看到改变。

　　那么我如果需要能改变这个网络物体上的状态，并能同步到所有别的客户端上，我们需要拥有这个网络物体的权限，因为这样才能在本机上发送Command命令，才能告诉服务器我改变了状态，服务器也才能告诉所有客户端这个网络物体改变了状态。

　　其中本地player在创建时，当前客户端对本地player有权限。客户端上有权限的网络物体上的SyncVar改变后，也并不会能同步到服务器，服务器根本没有注册UpdateVars消息，这种还是需要客户端自己调用Command命令。

* 问题6 UNet常见的封装状态同步处理有那些，其中NetworkTransform与NetworkAnimator分别怎样通信，如果是客户端权限的网络物体又是怎么通信的了。

　　UNet常见的封装状态同步状态方法有二种。

   一是通过ClientRpc与Command是封装发送消息。客户端与服务端一方调用，然后序列化相应的参数，然后到服务器与客户端反序列化参数执行。

   二是网络内置的序列化与反序列化，序列化服务器的状态，然后客户端反序列化相应的值，如SyncVar通过相应的OnSerialize,OnDeserialize.这种只能同步服务器到客户端。

　　这二种本质都是客户端与服务器互相发送MsgType消息，对应的服务器与客户端注册相应消息处理。NetworkAnimator 服务器上的动画改变，会发消息通知所有客户端相应状态改变了，如Rpc。NetworkTransform 服务器通过OnSerialize序列化相应的值，然后客户端反序列化相应的值。

　　如果客户端有对应NetworkTransform与NetworkAnimator网络物体的权限。NetworkAnimator 相应客户端提交状态到服务器上，然后分发到所有客户端，相当于调用了Command，并在Command里调用了Rpc方法。NetworkTransform 相应客户端发送消息到服务器上，服务器更新相应位置，方向。然后通过反序列化到所有客户端。

　　所以如果客户端有授权，那么NetworkAnimator与NetworkTransform在服务器或是有授权的客户端的状态改变都能更新到所有客户端，注意这二个组件对localPlayerAuthority的处理不同，在NetworkTransform中，localPlayerAuthority为false时,客户端不能更新到所有客户端，在NetworkAnimator中，localPlayerAuthority为true时，服务器不能更新到客户端上。

　　其中注意SyncVar特性，就算客户端授权，客户端改变后，也不会同步到别的机器上。

　　所以如果我们自己设计类似的网络组件，需要考虑客户端授权的相应处理，就是差不多添加一个Command命令。

* 问题7 客户端授权与本地player授权有什么区别。

　　一般物体的权限都在服务器上，如果要对网络物体授权给客户端，一般通过SpawnWithClientAuthority实现，这样在相应客户端上的hasAuthority为true,其中相应的playerControllerID为-1。

　　而本地player授权localPlayerAuthority，在相应的网络物体上的Local Player Authority勾选上，在对这个网络物体的所有监视客户端上,本地player授权都是true，这种一般用于玩家，或是玩家控制位移的物体，playerControllerID大于等于0。

　　所以客户端授权针对是某个客户端，在这个客户端上的这个网络物体的hasAuthority为true，而本地player针对是某个网络物体，在所有客户端上的这个网络物体的localPlayerAuthority都为true.

* 问题8 UNet怎么实现迷雾地图

　　通过NetworkProximityChecker,这样每桢检测当前网络物体的监视连接，确定那些客户端需要这个网络物体。同样，想实现更复杂的可以自己实现类似。

* 问题9 NetworkServer.Destroy做了啥

　　必须是网络物体，且最好能在服务器调用,调用时，发给所有的监视Connect,销毁对应网络物体，然后服务器销毁。请看MsgType.ObjectDestroy消息流程.

　　需要注意的是在服务器中，Destroy某网络物体，会自动调用NetworkServer.Destroy。代码在NetworkIdentity.OnDestroy.

* 问题10 服务器添加角色时做了那些事。

　　当客户端连接服务器时，设置自动创建角色后，会自动创建角色。

　　1 服务器添加一个player，设定playercontrollerID

　　2 设置当前conn的ready为true.然后检测当前的conn是否需要监视服务器上NetworkScene的网络物体列表的各个网络物体，其中客户端上的isspawnFinished表示NetworkScene的网络物体列表是否检测完成。

　　3 把服务器的player的spawn下去，设定对应网络物体记录的本地权限客户端为当前客户端,相应的playercontrollerid发送到客户端。

* 问题11 NetworkClient与networkServer的active表示什么，那些时候用

　　networkServer开始监听后，设定active为true。

　　networkClient连接上服务器后，设定为true。

　　当有些消息发送，或是Rpc与Command等的调用时，时机可能会在active之前，引发错误。

* 问题12 网络的Update做些啥。

　　1 服务器更新，处理一些如客户端链接与丢失链接，还有接收消息并找到对应事件处理，以及序列化服务器网络物体要更新的数据。

　　2 客户端更新，如上服务器的处理，主要也是相应消息处理。

　　3 检查服务器与客户端的场景是否加载完成。

最后，想象一下，在网络环境下，我们拉开弓箭，生成箭，箭在客户端上缓缓拉开，我们应该如何做？

　　首先弓箭要让所有客户端看的到，我们要在服务器上生成，然后spawn分发到相应多个客户端，然后当前客户端还需要当前箭的权限，这样当前用户才能控制这把箭，并把当前用户控制箭产生的新位置同步给所有的客户端。

　　其次如果采用Valve的LabRender渲染器，需要在开始服务器时关闭，等到对应的角色加载后，再通过localplayer打开各自对应的valveCamer,不然服务器上的valveCamer可能得不到正确的阴影图。
