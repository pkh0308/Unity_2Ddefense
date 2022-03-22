using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Vector3 originPos;
    public Vector3 limitPos;
    public int endPosOffset;
    Vector3 originEndPos;
    Vector3 limitEndPos;
    Vector3 startPos;
    Vector2 anchoredSartPos;
    RectTransform rectTransform;
    public enum DragType { Horizontal, Vertical }
    public DragType dragType;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        switch (dragType)
        {
            case DragType.Horizontal:
                originEndPos = originPos + Vector3.right * endPosOffset;
                limitEndPos = limitPos - Vector3.right * endPosOffset;
                break;
            case DragType.Vertical:
                originEndPos = originPos - Vector3.up * endPosOffset;
                limitEndPos = limitPos + Vector3.up * endPosOffset;
                break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
        anchoredSartPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float offset;
        Vector3 curPos;
        switch (dragType)
        {
            case DragType.Horizontal:
                offset = eventData.position.x - startPos.x;
                curPos = anchoredSartPos + Vector2.right * offset;

                if (curPos.x > originEndPos.x)
                    rectTransform.anchoredPosition = originEndPos;
                else if (curPos.x < limitEndPos.x)
                    rectTransform.anchoredPosition = limitEndPos;
                else
                    rectTransform.anchoredPosition = curPos;
                break;
            case DragType.Vertical:
                offset = eventData.position.y - startPos.y;
                curPos = anchoredSartPos + Vector2.up * offset;

                if (curPos.y < originEndPos.y)
                    rectTransform.anchoredPosition = originEndPos;
                else if (curPos.y > limitEndPos.y)
                    rectTransform.anchoredPosition = limitEndPos;
                else
                    rectTransform.anchoredPosition = curPos;
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