/* 饿汉式单例模式
上面的例子中，在这个类被加载时，静态变量instance会被初始化，此时类的私有构造器会被调用。
这时候，单例类的唯一实例就被创建出来了。
饿汉式其实是一种比较形象的称谓。既然饿，那么在创建对象实例的时候就比较着急，
饿了嘛，于是在装载类的时候就创建对象实例。
饿汉式是典型的空间换时间，当类装载的时候就会创建类的实例，不管你用不用，
先创建出来，然后每次调用的时候，就不需要再判断，节省了运行时间。
*/
public class EagerSingleton
{
    private static EagerSingleton _instance = new EagerSingleton();
    
    //私有构造器
    private EagerSingleton(){

    }

    public EagerSingleton GetInstance(){
        return _instance;
    }
}

public class Test{

    void static Main(string[] args){
            EagerSingleton.GetInstance();
    }
}