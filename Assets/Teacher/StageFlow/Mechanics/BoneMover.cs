using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Teacher
{
    public class BoneMover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private PuzzleFinishController puzzleFinishController;
        [SerializeField] private float matchRadius = 60f;

        private BoneConnector[] connectors;
        private List<BoneMover> group = new();
        private RectTransform piece;
        private RectTransform board;
        private Vector2 offset;
        private int? pointerId;

        private void Awake()
        {
            group.Add(this);
            connectors = GetComponentsInChildren<BoneConnector>();
            piece = (RectTransform)transform;
            board = (RectTransform)transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || pointerId != null)
                return;

            pointerId = eventData.pointerId;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.pressPosition, eventData.pressEventCamera, out Vector2 point);
            offset = (Vector2)piece.localPosition - point;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (pointerId != eventData.pointerId)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.position, eventData.pressEventCamera, out Vector2 point);
            Vector2 delta = point + offset - (Vector2)piece.localPosition;
            if (CanMoveGroup(delta))
            {
                MoveGroup(delta);
            }

            if (!puzzleFinishController.IsFinished)
            {
                TryMatch();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (pointerId == eventData.pointerId)
            {
                pointerId = null;
            }
        }

        private void TryMatch()
        {
            foreach (BoneMover bone in group)
            {
                foreach (BoneConnector connector in bone.connectors)
                {
                    BoneConnector otherPoint = connector.matchigPoint;
                    BoneMover otherBone = otherPoint.parentBone;
                    Vector2 delta = board.InverseTransformVector(connector.transform.position - otherPoint.transform.position);
                    if (group == otherBone.group || delta.magnitude > matchRadius)
                        continue;

                    otherBone.MoveGroup(delta);
                    group.AddRange(otherBone.group);
                    foreach (BoneMover member in otherBone.group)
                    {
                        member.group = group;
                    }

                    otherBone.pointerId = null;
                    puzzleFinishController.CheckCompletion(group.Count);
                    return;
                }
            }
        }

        private bool CanMoveGroup(Vector2 delta)
        {
            foreach (BoneMover bone in group)
            {
                if (!board.rect.Contains(bone.piece.localPosition + (Vector3)delta))
                    return false;
            }

            return true;
        }

        private void MoveGroup(Vector2 delta)
        {
            foreach (BoneMover bone in group)
            {
                bone.piece.localPosition += (Vector3)delta;
            }
        }
    }
}
