using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Controller_01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Ex_Func1(int a,int b)
    {
        Debug.Log(a + "," + b);
    }

    public int Ex_Func2(string name,int a)
    {
        Debug.Log(name + "," + "a");
        return a + 1;
    }

    public object Func()
    {
        return Ex_Func2("",1);
    }
}
