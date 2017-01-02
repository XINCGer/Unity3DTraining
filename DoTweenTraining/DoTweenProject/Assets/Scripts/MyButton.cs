using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MyButton : MonoBehaviour
{

    public RectTransform PanelRectTransform;
    private bool isIn = false;
    private Tweener tweener;

	// Use this for initialization
	void Start () {
        tweener = PanelRectTransform.DOLocalMove(new Vector3(0, 0, 0), 0.35f);
        tweener.SetAutoKill(false);
        tweener.Pause();
	}

    public void OnClick()
    {
        if (isIn == false)
        {
            tweener.PlayForward();
            isIn = true;
        }
        else
        {
            tweener.PlayBackwards();
            isIn = false;
        }
    }
}
