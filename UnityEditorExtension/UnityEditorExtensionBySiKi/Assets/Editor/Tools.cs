using UnityEngine;
using UnityEditor;

public class Tools {

    //[MenuItem("Tools/Test/Test1")]
    //static void Test() {
    //    Debug.Log("测试");
    //}

    //[MenuItem("Tools/Test/Test2")]
    //static void Test2() {
    //    Debug.Log("测试");
    //}

    //[MenuItem("Window/MyTool")]
    //static void Test3() {
    //    Debug.Log("测试");
    //}

    //每一个菜单栏的默认优先级为1000
    [MenuItem("Tools/Test1", false, 1)]
    static void Test1() {
        Debug.Log("测试1");
    }

    [MenuItem("Tools/Test2", false, 14)]
    static void Test2() {
        Debug.Log("测试2");
    }

    [MenuItem("Tools/Test3", false, 3)]
    static void Test3() {
        Debug.Log("测试3");
    }

    [MenuItem("GameObject/MyTool", false, 10)]
    static void Test4() {
        Debug.Log("测试4");
    }

    [MenuItem("Assets/MyButton")]
    static void Test5() {
        Debug.Log("测试5");
    }
}
