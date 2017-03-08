using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    #region Enter And Exit
    public static Action<Transform> OnEnter;
    public static Action OnExit;
    public void OnPointerExit(PointerEventData eventData) {
        if (eventData.pointerEnter.tag == "Grid") {
            if (OnExit != null) {
                OnExit();
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (eventData.pointerEnter.tag == "Grid") {
            if (OnEnter != null) {
                OnEnter(transform);
            }
        }
    }
    #endregion

    public void OnBeginDrag(PointerEventData eventData) {
        throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData) {
        throw new NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData) {
        throw new NotImplementedException();
    }
}
