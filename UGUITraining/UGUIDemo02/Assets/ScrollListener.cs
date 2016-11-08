using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScrollListener : MonoBehaviour,IBeginDragHandler,IEndDragHandler {

    private ScrollRect scrollRect;
    private float[] pageArray = new float []{ 0, 0.333333f, 0.666666f, 1 };
	// Use this for initialization
	void Start () {
        scrollRect=this.GetComponent<ScrollRect>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    /// <summary>
    /// 列表滑动自动吸附功能
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = scrollRect.horizontalNormalizedPosition;
        Debug.Log(posX);
        int index=0;
        float offset = Mathf.Abs(posX - pageArray[index]);
        Debug.Log("offset:" + offset);
        for (int i = 1; i < pageArray.Length; i++)
        {
            float offsetTemp = Mathf.Abs(posX - pageArray[i]);
            Debug.Log("offsetTemp:" + offsetTemp);
            if (offsetTemp < offset)
            {
                index = i;
                offset = offsetTemp;
            }
        }
        Debug.Log(index);
        scrollRect.horizontalNormalizedPosition = pageArray[index];
    }
}
