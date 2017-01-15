using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MyShakeScreen : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
        //this.transform.DOShakePosition(1f, 1);
	    this.transform.DOShakePosition(1, new Vector3(3, 3, 0));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
