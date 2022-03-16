using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Vector3 originPos;
    public Vector3 limitPos;
    Vector3 startPos;
    Vector2 anchoredSartPos;
    RectTransform rectTransform;
    public enum DragType { Horizontal, Vertical }
    public DragType dragType;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
        anchoredSartPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float offset = 0;
        switch (dragType)
        {
            case DragType.Horizontal:
                offset = eventData.position.x - startPos.x;
                rectTransform.anchoredPosition = anchoredSartPos + Vector2.right * offset;
                break;
            case DragType.Vertical:
                offset = eventData.position.y - startPos.y;
                rectTransform.anchoredPosition = anchoredSartPos + Vector2.up * offset;
                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch (dragType)
        {
            case DragType.Horizontal:
                if (rectTransform.anchoredPosition.x < originPos.x)
                    rectTransform.anchoredPosition = limitPos;
                else if (rectTransform.anchoredPosition.x > limitPos.x)
                    rectTransform.anchoredPosition = originPos;
                break;
            case DragType.Vertical:
                if (rectTransform.anchoredPosition.y < originPos.y)
                    rectTransform.anchoredPosition = originPos;
                else if (rectTransform.anchoredPosition.y > limitPos.y)
                    rectTransform.anchoredPosition = limitPos;
                break;
        }
    }

    public void PosReset()
    {
        rectTransform.anchoredPosition = originPos;
    }
}