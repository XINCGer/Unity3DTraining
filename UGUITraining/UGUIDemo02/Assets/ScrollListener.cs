using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ScrollListener : MonoBehaviour,IBeginDragHandler,IEndDragHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
