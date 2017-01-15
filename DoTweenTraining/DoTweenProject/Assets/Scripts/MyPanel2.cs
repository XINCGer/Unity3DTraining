using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MyPanel2 : MonoBehaviour
{

    private DOTweenAnimation _tweenAnimation;
    private bool _isShow=false;

	// Use this for initialization
	void Start ()
	{
	    _tweenAnimation = GetComponent<DOTweenAnimation>();
	}

    public void OnClick()
    {
        if (_isShow == false)
        {
            _tweenAnimation.DOPlayBackwards();
        }
        else
        {
            _tweenAnimation.DOPlayForward();
        }
        _isShow = !_isShow;
    }
}
