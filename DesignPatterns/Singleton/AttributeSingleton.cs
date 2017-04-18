/*
C# 利用属性的方法实现单例模式
 */
public class AttributeSingleton
{
     public string Name{get;set;}
     private static AttributeSingleton _instance;
     
     //私有构造器
     private AttributeSingleton(){

     }

     public static AttributeSingleton Instance{
         get{
             if(_instance ==null){
                 _instance = new AttributeSingleton();
             }
             return _instance;
         }
     }
}

public class Test{

    void static Main(string[] args){
            AttributeSingleton.Instance.Name;
    }
}