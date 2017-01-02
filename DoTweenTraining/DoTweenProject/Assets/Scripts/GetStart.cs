using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GetStart : MonoBehaviour {

    public Vector3 myValue = new Vector3(0,0,0);
    public Transform CubeTransform;
    public RectTransform PanelRectTransform;
	// Use this for initialization
	void Start ()
	{
        //DOTween.To(() => myValue, x => myValue = x, new Vector3(10, 10, 10), 2);
	    DOTween.To(() => myValue, x => myValue = x, new Vector3(0, 0, 0), 2);
	}
	
	// Update is called once per frame
	void Update ()
	{
        //CubeTransform.position = myValue;
	    PanelRectTransform.localPosition = myValue;
	}
}
