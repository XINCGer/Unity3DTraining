using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour
{

    private Text _text;
	// Use this for initialization
	void Start ()
	{
	    _text = this.GetComponent<Text>();
	    _text.DOText("接下来，进入游戏第二篇章", 4);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
