using UnityEngine;
using UnityEngine.EventSystems;

// Каждая кость передаёт события указателя контроллеру общего поля.
[RequireComponent(typeof(RectTransform))]
public sealed class PuzzleGroupPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private PuzzleAssemblyController assembly;

    internal RectTransform Rect => (RectTransform)transform;

    private void Awake()
    {
        assembly = transform.parent.GetComponent<PuzzleAssemblyController>();
    }

    public void OnBeginDrag(PointerEventData eventData) => assembly.BeginDrag(this, eventData);
    public void OnDrag(PointerEventData eventData) => assembly.Drag(this, eventData);
    public void OnEndDrag(PointerEventData eventData) => assembly.EndDrag(this, eventData.pointerId);

    private void OnDisable() => assembly?.CancelDrag(this);

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            assembly?.CancelDrag(this);
    }
}
