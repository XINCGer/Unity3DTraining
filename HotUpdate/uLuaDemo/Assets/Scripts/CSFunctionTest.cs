using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSFunctionTest
{
    /// <summary>
    /// 此方法可以通过ulua自动生成warp文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static void CSFunction(string name)
    {
        Debug.Log("Hello," + name);
    }

    public static void CSAdd(int num)
    {
        Debug.Log("Result: " + num + 1);
    }
}
