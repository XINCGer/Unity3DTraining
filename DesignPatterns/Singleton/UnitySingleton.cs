/* 
Unity中的单例模式
*/
public class UnitySingleton: MonoBehaviour
{
    private static UnitySingleton _instance = null;

    void Awake(){
        _instance = this;
    }

    public static UnitySingleton GetInstance(){
        return _instance;
    }
}

public class Test{

    void static Main(string[] args){
            UnitySingleton.GetInstance();
    }
}