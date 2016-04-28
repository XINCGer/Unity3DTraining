using UnityEngine;
using System.Collections;

public class HelloWorld : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // 在函数OnGUI中定义UI的布局和功能
    void OnGUI()
    {

        // 改变字符的大小
        GUI.skin.label.fontSize = 100;

        // 输出文字
        GUI.Label( new Rect( 10, 10, Screen.width, Screen.height ), "Hello World" );
    }
}
