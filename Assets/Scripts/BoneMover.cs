using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoneMover : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    PuzzleFinishController puzzleFinishController;
    BoneConnector[] boneConnectors;
    RectTransform boardSize;
    [SerializeField]
    float matchRadius = 60f;
    List<BoneMover> connectedPuzzlePeaces = new();
    RectTransform currentPuzzlePeace;
    Vector2 offset;
    int? pointerID;
    void TryMatch()
    {
        foreach(BoneMover bone1 in connectedPuzzlePeaces)
        {
        foreach(BoneConnector conection in bone1.boneConnectors)
        {
            BoneConnector secondPoint = conection.matchigPoint;
            BoneMover secondBone = secondPoint.parentBone;
            Vector2 distance = conection.transform.position - secondPoint.transform.position;
            Vector2 boardDistanse = boardSize.InverseTransformVector(distance);
            if(boardDistanse.magnitude > matchRadius || connectedPuzzlePeaces == secondBone.connectedPuzzlePeaces)
            continue;
            secondBone.MoveBones(boardDistanse);
            connectedPuzzlePeaces.AddRange(secondBone.connectedPuzzlePeaces);
            foreach(BoneMover bone in secondBone.connectedPuzzlePeaces)
            {
                bone.connectedPuzzlePeaces = connectedPuzzlePeaces;
            }
            secondBone.pointerID = null;
            puzzleFinishController.IsPuzzleFinished(bone1.connectedPuzzlePeaces.Count);
            return;
        }
        }
    }
    void MoveBones(Vector2 delta)
    {
        foreach(BoneMover bone in connectedPuzzlePeaces)
        {
            bone.currentPuzzlePeace.localPosition += (Vector3)delta;
        }
    }
      bool IsCentersInsideBoard(Vector2 delta)
    {
        Rect boardRect = boardSize.rect;
        foreach(BoneMover bone in connectedPuzzlePeaces)
        {
            if(boardRect.Contains(bone.currentPuzzlePeace.localPosition + (Vector3)delta))
            {
                continue;
            }
            return false;
        }
        return true;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left || pointerID != null || puzzleFinishController.isCanMove != true)
        return;
        pointerID = eventData.pointerId;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardSize, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localPoint);
        offset = (Vector2)currentPuzzlePeace.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(pointerID != eventData.pointerId)
        return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(boardSize, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        Vector2 delta = localPoint +  offset - (Vector2)currentPuzzlePeace.localPosition;
        if(IsCentersInsideBoard(delta))
        MoveBones(delta);
        if(puzzleFinishController.isFinished == false)
        TryMatch();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(pointerID == eventData.pointerId)
        pointerID = null;
    }

    void Awake()
    {
        connectedPuzzlePeaces.Add(this);
        boneConnectors = gameObject.GetComponentsInChildren<BoneConnector>();
        currentPuzzlePeace = (RectTransform)this.transform;
        boardSize = (RectTransform)this.transform.parent;
    }

    void Update()
    {
        
    }
}
