using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomEaseTest : MonoBehaviour
{

    public AnimationCurve Curve;
	// Use this for initialization
	void Start ()
	{
	    this.transform.DOMoveZ(-20, 2).SetEase(Curve);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
