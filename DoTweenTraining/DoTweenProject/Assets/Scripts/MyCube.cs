using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class MyCube : MonoBehaviour
{
    private int y;
	// Use this for initialization
	void Start ()
	{
	    transform.DOMoveX(5, 1).From();
        //默认为ToTween,从当前位置移动到目标位置，加上From以后，表示从目标位置移动到当前位置
        transform.DOMoveX(5, 1).From(true);
        //设置为true时，表示相对坐标

	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
