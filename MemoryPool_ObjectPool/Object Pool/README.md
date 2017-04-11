## Unity3D内存管理——对象池(Object Pool)   


原文地址：[C# Memory Management for Unity Developers (part 3 of 3)](http://www.gamasutra.com/blogs/WendelinReich/20131127/203843/C_Memory_Management_for_Unity_Developers_part_3_of_3.php)  
其实从原文标题可以看出，这是一系列文章中的第三篇，前两篇讲解了从C#语言本身优化内存和Unity3D Profiler的使用，都很精彩，有兴趣的童鞋可以参考一下。  
[C# Memory Management for Unity Developers (part 1 of 3)](http://www.gamasutra.com/blogs/WendelinReich/20131109/203841/C_Memory_Management_for_Unity_Developers_part_1_of_3.php)   
[C# Memory Management for Unity Developers (part 2 of 3)](http://www.gamasutra.com/blogs/WendelinReich/20131119/203842/C_Memory_Management_for_Unity_Developers_part_2_of_3.php)   

### 从一个简单的对象池类开始说起  
对象池背后的理念其实是非常简单的。我们将对象存储在一个池子中，当需要时在再次使用，而不是每次都实例化一个新的对象。池的最重要的特性，也就是对象池设计模式的本质是允许我们获取一个“新的”对象而不管它真的是一个新的对象还是循环使用的对象。该模式可以用以下简单的几行代码实现：  
```C#  
public class ObjectPool<T> where T : class, new()
{
    private Stack<T> m_objectStack = new Stack<T>();

    public T New()
    {
        return (m_objectStack.Count == 0) ? new T() : m_objectStack.Pop();
    }

    public void Store(T t)
    {
        m_objectStack.Push(t);
    }
}   
```  
很简单，也很好地体现了该模式的核心。如果你不太理解”where T”，没关系，稍后会解释的。如何使用呢？很简单，你只需要找到之前使用new操作符的表达式，例如：
```C#  
void Update()
{
    MyClass m = new MyClass();
}
```  
然后将其替换为New()和Store()。  
```C#  
ObjectPool<MyClass> poolOfMyClass = new ObjectPool<MyClass>();

void Update()
{
    MyClass m = poolOfMyClass.New();

    // do stuff...

    poolOfMyClass.Store(m);
}
```  
### 增加点复杂度  
我是简洁主义的忠实信徒，但就目前而言ObjectPool类或许过于简单了。如果你搜索下用C#实现的对象池类库，你会发现其中很多是相当复杂的。我们先暂停一下，仔细想想在一个通用的对象池中到底哪些是我们需要的，哪些是不需要的：

* 很多类型的对象被重新使用前，在某些情况下，需要被reset。至少，所有的成员变量都要设置成初始值。这可以在池中实现而不需要用户处理。何时和如何重置需要考虑以下两个方面：
* 重置是立即的（例如，在存储对象时即重置）还是延迟的（例如，在对象被重新使用后重置）。
* 重置是被池管理（例如，对于被放入池中的对象来说是透明的）还是声明池对象的类。
* 在上面的例子中，poolofMyClass池对象需要显示申明在类级别作用域。显然，当我们需要一个其它类型的对象池时就需要重新申明一个。或许我们可以实现一个对用户透明。
* 创建管理所有类型池的ObjectPool。
* 一些对象池类库管理了太多种类的可怕的资源（如内存，数据库连接，游戏对象，外部资产等）。这无疑增加了对象池的代码复杂度。
* 某些类型的资源是很珍贵的（如数据库连接），池需要显示上限并提供一个针对分配对象失败的安全措施；
* 当池中对象很多却很少使用时，或许需要收缩的功能（不管是自动的还是强制的）。
* 最后，池可以被多个线程共享，因此需要实现为线程安全的。
 

那么其中那些是必需的呢？你的答案或许和我的不一样，但请允许我阐述我的观点：

* 重置是必需的。但是正如你将在下面看的那样，我并没有强制到底是在池中还是被管理类中处理重置逻辑。你可能两种都需要，之后的代码中我将向你展示各自两个版本。
* Unity强制限制多线程。你可以在主线程中定义工作者线程，但只有主线程可以调用Unity API。以我的经验看来，我们并不需要将池实现为支持多线程。
* 仅个人而言，我并不介意每次为一个类型申明一个新的池。可选的方案是采用单例模式：创建一个新的对象池并放置于存储池的字典中，该字典放置在一个静态变量中。为了安全使用，你需要将将你的对象池实现为支持多线程。但就我看到的对象池而言没有一个是100%安全的。
* 在本篇文章中我重点处理内存。其它类型资源池也是很重要的，但超出本篇文章的范围。这很大程度上减少了以下的需求：
  * 不需要一个作限制用的最大值。如果你的游戏使用太多的资源，你已经陷入麻烦了，对象池也救不了你。
  * 我们也可以假设没有其它进程等待你尽快释放内存。这就意味着重置可以是延迟的，也不需要提供收缩功能。  
  
### 基本的实现初始化和重置的池(A basic pool with initialization and reset)  
修订后的版本如下：  
```C#  
public class ObjectPool<T> where T : class, new()
{
    private Stack<T> m_objectStack;

    private Action<T> m_resetAction;
    private Action<T> m_onetimeInitAction;

    public ObjectPool(int initialBufferSize, Action<T>
        ResetAction = null, Action<T> OnetimeInitAction = null)
    {
        m_objectStack = new Stack<T>(initialBufferSize);
        m_resetAction = ResetAction;
        m_onetimeInitAction = OnetimeInitAction;
    }

    public T New()
    {
        if (m_objectStack.Count > 0)
        {
            T t = m_objectStack.Pop();

            if (m_resetAction != null)
                m_resetAction(t);

            return t;
        }
        else
        {
            T t = new T();

            if (m_onetimeInitAction != null)
                m_onetimeInitAction(t);

            return t;
        }
    }

    public void Store(T obj)
    {
        m_objectStack.Push(obj);
    }
}
```  
该实现非常简单直白。参数T被指明为”where T:class,new()”，意味着有两个限制。首先，T必须为一个类（毕竟，只有引用类型需要被obejct-pool）；其次，它必须要有一个无参构造函数。

构造函数将池可能的最大值作为第一个参数。另外两个是可选的闭包，如果传入值，第一个闭包将用来重置池，第二个初始化一个新的对象。除了构造函数外，ObjectPool<T>只有两个方法：New()和Store()。因为池使用了延迟策略，主要的工作在于New()。其中，新的和循环使用的对象要么被实例化，要么被重置，这两个操作通过传入的闭包实现。以下是池的使用方法：  
```C#  
class SomeClass : MonoBehaviour
{
    private ObjectPool<List<Vector3>> m_poolOfListOfVector3 =
        //32为假设的最大数量
        new ObjectPool<List<Vector3>>(32,
        (list) => {
            list.Clear();
        },
        (list) => {
            //初始化容量为1024
            list.Capacity = 1024;
        });

    void Update()
    {
        List<Vector3> listVector3 = m_poolOfListOfVector3.New();

        // do stuff

        m_poolOfListOfVector3.Store(listVector3);
    }
}
```   
### 被管理类型自重置的池(A pool that lets the managed type reset itself)   

上述的对象池实现了基本功能，但还是有瑕疵。它将初始化和重置对象在对象定义中分开了，在一定程度了违反了封装原则。导致了紧耦合，这是需要尽可能避免的。在上述SomeClass中，我们是没有真正的替代方案的，因为我们不能修改List<T>的定义。然而，当你用自定义类时，你可以实现IResetable接口作为代替。对应的ObjectPoolWithReset<T>也可以不需要指明两个闭包了（请注意，为了灵活性我还是留下了）。  
```C#  
public interface IResetable
{
    void Reset();
}

public class ObjectPoolWithReset<T> where T : class, IResetable, new()
{
    private Stack<T> m_objectStack;

    private Action<T> m_resetAction;
    private Action<T> m_onetimeInitAction;

    public ObjectPoolWithReset(int initialBufferSize, Action<T>
        ResetAction = null, Action<T> OnetimeInitAction = null)
    {
        m_objectStack = new Stack<T>(initialBufferSize);
        m_resetAction = ResetAction;
        m_onetimeInitAction = OnetimeInitAction;
    }

    public T New()
    {
        if (m_objectStack.Count > 0)
        {
            T t = m_objectStack.Pop();
            //自行重置
            t.Reset();

            if (m_resetAction != null)
                m_resetAction(t);

            return t;
        }
        else
        {
            T t = new T();

            if (m_onetimeInitAction != null)
                m_onetimeInitAction(t);

            return t;
        }
    }

    public void Store(T obj)
    {
        m_objectStack.Push(obj);
    }
}
```  
### 集体重置池(A pool with collective reset)  
有一些类型不需要在一系列帧中存留，仅在帧结束前就失效了。在这种情况下，我们可以在一个合适的时机将所有已经池化的对象(pooled objects)再次存储于池中。现在，我们重写该池使之更加简单高效。  
```C#  
public class ObjectPoolWithCollectiveReset<T> where T : class, new()
{
    private List<T> m_objectList;
    private int m_nextAvailableIndex = 0;

    private Action<T> m_resetAction;
    private Action<T> m_onetimeInitAction;

    public ObjectPoolWithCollectiveReset(int initialBufferSize, Action<T>
        ResetAction = null, Action<T> OnetimeInitAction = null)
    {
        m_objectList = new List<T>(initialBufferSize);
        m_resetAction = ResetAction;
        m_onetimeInitAction = OnetimeInitAction;
    }

    public T New()
    {
        if (m_nextAvailableIndex < m_objectList.Count)
        {
            // an allocated object is already available; just reset it
            T t = m_objectList[m_nextAvailableIndex];
            m_nextAvailableIndex++;

            if (m_resetAction != null)
                m_resetAction(t);

            return t;
        }
        else
        {
            // no allocated object is available
            T t = new T();
            m_objectList.Add(t);
            m_nextAvailableIndex++;

            if (m_onetimeInitAction != null)
                m_onetimeInitAction(t);

            return t;
        }
    }

    public void ResetAll()
    {
       //重置索引
        m_nextAvailableIndex = 0;
    }
}
```  
相比于原始的ObjectPool <T> ，改动还是蛮大的。先不管类的签名，可以看到，Store()已经被ResetAll()代替了,且仅在所有已经分配的对象需要被放入池中时调用一次。在类内部，Stack<T>被List<T>代替，其中保存了所有已分配的对象（包括正在使用的对象）的引用。我们也可以跟踪最近创建或释放的对象在list中索引，由此，New()便可以知道是创建一个新的对象还是重置一个已有的对象。  
### 拓展阅读（非译文部分）  
原文地址：http://www.gamasutra.com/blogs/WendelinReich/20131127/203843/C_Memory_Management_for_Unity_Developers_part_3_of_3.php

