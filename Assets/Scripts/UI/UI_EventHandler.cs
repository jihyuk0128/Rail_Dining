using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour,IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnBeginDragHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEndDragHandler = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("onPointerClick");
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (OnBeginDragHandler != null)
            OnBeginDragHandler.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("onOnDrag");
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        if (OnEndDragHandler != null)
            OnEndDragHandler.Invoke(eventData);
    }
}
