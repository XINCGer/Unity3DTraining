/*可以使用“双重检查加锁”的方式来实现，就可以既实现线程安全
又能够使性能不受很大的影响。那么什么是“双重检查加锁”机制呢？
所谓“双重检查加锁”机制，指的是：并不是每次进入getInstance方法都需要同步
而是先不同步，进入方法后，先检查实例是否存在，如果不存在才进行下面的同步块
这是第一重检查，进入同步块过后，再次检查实例是否存在，如果不存在，就在同步的情况下创建一个实例
这是第二重检查。这样一来，就只需要同步一次了，从而减少了多次在同步情况下进行判断所浪费的时间。
这种实现方式既可以实现线程安全地创建实例，而又不会对性能造成太大的影响。它只是第一次创建实例的时候同步
以后就不需要同步了，从而加快了运行速度
*/
public class SyncLazySingleton
{
    private staitc SyncLazySingleton _instance = null;

    //私有构造器
    private SyncLazySingleton(){

    }

    public staitc SyncLazySingleton GetInstance(){
        //先检查实例是否存在，如果不存在才进入下面的同步块
        if(_instance ==null){
            //同步块，线程安全的创建实例
            lock (_instance)
            {
                //再次检查实例是否存在，如果不存在才真正的创建实例
                if(_instance=null)
                _instance = new SyncLazySingleton();                
            }
        }
        return _instance;
    }
}

public class Test{

    void static Main(string[] args){
            SyncLazySingleton.GetInstance();
    }
}