using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger{
    public delegate void VoidDelegate (GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    static public EventTriggerListener Get (GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if(onClick != null)     onClick(gameObject);
    }
    public override void OnPointerDown (PointerEventData eventData){
        if(onDown != null) onDown(gameObject);
    }
    public override void OnPointerEnter (PointerEventData eventData){
        if(onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit (PointerEventData eventData){
        if(onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp (PointerEventData eventData){
        if(onUp != null) onUp(gameObject);
    }
    public override void OnSelect (BaseEventData eventData){
        if(onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected (BaseEventData eventData){
        if(onUpdateSelect != null) onUpdateSelect(gameObject);
    }
}
