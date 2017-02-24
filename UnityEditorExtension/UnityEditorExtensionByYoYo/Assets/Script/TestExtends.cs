using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Script/TestExtends")]
[RequireComponent(typeof(Rigidbody))]
public class TestExtends : MonoBehaviour {

    [ContextMenu("测试")]
    public void TestMenu()
    {
        Debug.Log("输出.....");
    }
}
