/*懒汉式单例模式
懒汉式是典型的时间换空间,就是每次获取实例都会进行判断，看是否需要创建实例，浪费判断的时间。
当然，如果一直没有人使用的话，那就不会创建实例，则节约内存空间
*/
public class LazySingleton
{
    private staitc LazySingleton _instance = null;

    //私有构造器
    private LazySingleton(){

    }

    public staitc LazySingleton GetInstance(){
        if(_instance ==null){
            _instance = new LazySingleton();
        }
        return _instance;
    }
}

public class Test{

    void static Main(string[] args){
            LazySingleton.GetInstance();
    }
}