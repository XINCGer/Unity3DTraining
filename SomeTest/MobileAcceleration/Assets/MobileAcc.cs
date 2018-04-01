using System;
using UnityEngine;
using System.Collections;

public class MobileAcc : MonoBehaviour
{
	void OnGUI()
	{  
		GUI.Box(new Rect(5, 5, 100, 50), String.Format("{0:0.00}", Input.acceleration.x));
		GUI.Box(new Rect(5, 60, 100, 50), String.Format("{0:0.00}", Input.acceleration.y));
		GUI.Box(new Rect(5, 115, 100, 50), String.Format("{0:0.00}", Input.acceleration.z));
	}
}

