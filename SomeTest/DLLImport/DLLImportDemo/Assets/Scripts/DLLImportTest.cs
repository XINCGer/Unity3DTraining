using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DLLImportTest : MonoBehaviour {


    [DllImport("MyDLL")]
    private static extern int Pow2(int num);

    //[DllImport("MyDLL")]
    //private static extern void SomeFunction(ref string str);

    // Use this for initialization
    void Start () {
		Debug.Log(Pow2(10));
        //string str = "LiMing.";
        //SomeFunction(ref str);
        //Debug.Log(str);
    }

    

	// Update is called once per frame
	void Update () {
		
	}
}
