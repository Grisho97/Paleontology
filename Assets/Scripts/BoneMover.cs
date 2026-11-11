using UnityEngine;
using UnityEngine.EventSystems;

public class BoneMover : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    RectTransform boardSize;
    RectTransform puzzlePeace;
    Vector2 offset;
    int? pointerID;
      bool IsCenterInsideBoard(Vector2 localPos)
    {
        Rect boardRect = boardSize.rect;
        return boardRect.Contains(localPos);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left || pointerID != null)
        return;
        pointerID = eventData.pointerId;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardSize, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localPoint);
        offset = (Vector2)puzzlePeace.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(pointerID != eventData.pointerId)
        return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardSize, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        if(IsCenterInsideBoard(localPoint + offset))
        puzzlePeace.localPosition = localPoint + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(pointerID == eventData.pointerId)
        pointerID = null;
    }

    void Awake()
    {
        puzzlePeace = (RectTransform)this.transform;
        boardSize = (RectTransform)this.transform.parent;
    }

    void Update()
    {
        
    }
}
