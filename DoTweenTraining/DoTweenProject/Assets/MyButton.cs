using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MyButton : MonoBehaviour
{

    public RectTransform PanelRectTransform;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick()
    {
        PanelRectTransform.DOLocalMove(new Vector3(0, 0, 0), 0.35f);
    }
}
