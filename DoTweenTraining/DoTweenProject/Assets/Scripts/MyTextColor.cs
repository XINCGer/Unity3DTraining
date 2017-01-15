using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MyTextColor : MonoBehaviour
{

    private Text _text;
	// Use this for initialization
	void Start ()
	{
	    _text = GetComponent<Text>();
        //_text.DOColor(Color.red, 2);
	    _text.DOFade(1, 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
