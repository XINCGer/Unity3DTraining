using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChildHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Child OnPointerClick" + eventData.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Child OnPointerDown" + eventData.ToString());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Child OnPointerUp" + eventData.ToString());
    }
}