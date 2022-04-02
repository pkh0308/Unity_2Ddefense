using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /*
    드래그 필요한 UI에 적용하기 위한 클래스(미션, 스테이지 셀렉트 화면)
    상하 or 좌우 드래그 타입 선택하여 사용(horizontal, vertical)
    endPosOffset : 드래그 중에 originPos, limitPos 를 넘어갈 수 있는 수치
    */

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
                if (rectTransform.anchoredPosition.x > originPos.x)
                    rectTransform.anchoredPosition = originPos;
                else if (rectTransform.anchoredPosition.x < limitPos.x)
                    rectTransform.anchoredPosition = limitPos;
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