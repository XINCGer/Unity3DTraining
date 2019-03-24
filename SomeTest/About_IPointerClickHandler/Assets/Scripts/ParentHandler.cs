using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParentHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Parent OnPointerClick" + eventData.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Parent OnPointerDown" + eventData.ToString());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Parent OnPointerUp" + eventData.ToString());
    }
}
