using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// На поле хранится список совместимых стыков и текущее разбиение костей на группы.
[RequireComponent(typeof(RectTransform))]
public sealed class PuzzleAssemblyController : MonoBehaviour
{
    [Serializable]
    private sealed class Joint
    {
        public RectTransform firstPoint;
        public RectTransform secondPoint;

        [NonSerialized] public PuzzleGroupPiece firstPiece;
        [NonSerialized] public PuzzleGroupPiece secondPiece;
    }

    private sealed class Group
    {
        public readonly List<PuzzleGroupPiece> Pieces = new();
        public PuzzleGroupPiece GrabbedPiece;
        public int PointerId;
        public Vector2 GrabOffset;
    }

    [SerializeField] private Joint[] joints = Array.Empty<Joint>();
    [SerializeField, Min(1f)] private float snapRadius = 65f;
    [SerializeField] private TMP_Text status;

    private readonly Dictionary<PuzzleGroupPiece, Group> groups = new();
    private RectTransform board;

    private void Awake()
    {
        board = (RectTransform)transform;
        foreach (Transform child in board)
        {
            if (!child.TryGetComponent(out PuzzleGroupPiece piece))
                continue;

            var group = new Group();
            group.Pieces.Add(piece);
            groups.Add(piece, group);
        }

        foreach (Joint joint in joints)
        {
            if (joint.firstPoint != null)
                joint.firstPiece = joint.firstPoint.GetComponentInParent<PuzzleGroupPiece>();
            if (joint.secondPoint != null)
                joint.secondPiece = joint.secondPoint.GetComponentInParent<PuzzleGroupPiece>();
        }
    }

    internal void BeginDrag(PuzzleGroupPiece piece, PointerEventData pointer)
    {
        if (pointer.button != PointerEventData.InputButton.Left || groups[piece].GrabbedPiece != null)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, pointer.pressPosition, pointer.pressEventCamera, out Vector2 press))
            return;

        Group group = groups[piece];
        group.GrabbedPiece = piece;
        group.PointerId = pointer.pointerId;
        group.GrabOffset = (Vector2)piece.Rect.localPosition - press;
        foreach (PuzzleGroupPiece member in group.Pieces)
            member.transform.SetAsLastSibling();
    }

    internal void Drag(PuzzleGroupPiece piece, PointerEventData pointer)
    {
        Group group = groups[piece];
        if (group.GrabbedPiece != piece || group.PointerId != pointer.pointerId)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, pointer.position, pointer.pressEventCamera, out Vector2 position))
            return;

        Vector2 requestedDelta = position + group.GrabOffset - (Vector2)piece.Rect.localPosition;
        Vector2 delta = ClampDelta(group, requestedDelta);
        Move(group, delta);
        TryJoin(group);
    }

    internal void EndDrag(PuzzleGroupPiece piece, int pointerId)
    {
        Group group = groups[piece];
        if (group.GrabbedPiece == piece && group.PointerId == pointerId)
            group.GrabbedPiece = null;
    }

    internal void CancelDrag(PuzzleGroupPiece piece)
    {
        if (groups.TryGetValue(piece, out Group group) && group.GrabbedPiece == piece)
            group.GrabbedPiece = null;
    }

    private void TryJoin(Group moving)
    {
        Group partner = null;
        Vector2 snapDelta = Vector2.zero;
        float bestDistance = snapRadius * snapRadius;
        foreach (Joint joint in joints)
        {
            if (joint.firstPiece == null || joint.secondPiece == null ||
                !groups.TryGetValue(joint.firstPiece, out Group first) ||
                !groups.TryGetValue(joint.secondPiece, out Group second))
                continue;

            if (first == second || (first != moving && second != moving))
                continue;

            bool firstIsMoving = first == moving;
            Group other = firstIsMoving ? second : first;
            if (other.GrabbedPiece != null)
                continue;

            Vector2 delta = board.InverseTransformPoint(joint.secondPoint.position)
                - board.InverseTransformPoint(joint.firstPoint.position);
            if (!firstIsMoving)
                delta = -delta;

            float distance = delta.sqrMagnitude;
            if (distance > bestDistance || (partner != null && distance == bestDistance))
                continue;

            // У края поля точный стык возможен, только если помещается вся подвижная группа.
            if ((ClampDelta(moving, delta) - delta).sqrMagnitude >= 0.0001f)
                continue;

            partner = other;
            snapDelta = delta;
            bestDistance = distance;
        }

        if (partner == null)
            return;

        Move(moving, snapDelta);
        // Перенос при стыковке меняет смещение относительно пальца. Сохраняем его,
        // чтобы следующий кадр перетаскивания не дёрнул группу обратно.
        moving.GrabOffset += snapDelta;
        foreach (PuzzleGroupPiece piece in partner.Pieces)
        {
            moving.Pieces.Add(piece);
            groups[piece] = moving;
        }

        if (status != null)
            status.text = moving.Pieces.Count == groups.Count
                ? "Скелет собран. Перемещайте его за любую кость."
                : $"В группе: {moving.Pieces.Count} из {groups.Count}. Можно продолжать перемещение.";
    }

    private Vector2 ClampDelta(Group group, Vector2 delta)
    {
        // Все кости являются прямыми потомками поля. Складываем их позиции и
        // границы RectTransform, чтобы ограничить группу как единое целое.
        Vector2 min = new(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 max = new(float.NegativeInfinity, float.NegativeInfinity);
        foreach (PuzzleGroupPiece piece in group.Pieces)
        {
            Vector2 position = piece.Rect.localPosition;
            min = Vector2.Min(min, position + piece.Rect.rect.min);
            max = Vector2.Max(max, position + piece.Rect.rect.max);
        }

        return new Vector2(
            Mathf.Clamp(delta.x, board.rect.xMin - min.x, board.rect.xMax - max.x),
            Mathf.Clamp(delta.y, board.rect.yMin - min.y, board.rect.yMax - max.y));
    }

    private static void Move(Group group, Vector2 delta)
    {
        foreach (PuzzleGroupPiece piece in group.Pieces)
            piece.Rect.localPosition += (Vector3)delta;
    }
}
