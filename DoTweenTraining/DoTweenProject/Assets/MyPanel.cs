using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MyPanel : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    Tweener _tweener = this.transform.DOLocalMoveX(0, 2);
	    _tweener.SetEase(Ease.InExpo);
	    _tweener.SetLoops(1);
	    _tweener.OnComplete(delegate() { Debug.Log("动画播放完毕！");});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
