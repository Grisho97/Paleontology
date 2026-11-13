using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Как в BoneMover, жест начинается с захвата, меняет позицию и заканчивается отпусканием.
// Здесь тот же сдвиг получает каждая кость, связанная с захваченной.
[RequireComponent(typeof(RectTransform))]
public sealed class PuzzleGroupPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField, Min(1f)] private float snapRadius = 65f;

    // Все кости одной сборки ссылаются на один список. В начале в нём только эта кость.
    private List<PuzzleGroupPiece> connectedBones = new();

    private RectTransform piece;
    private RectTransform board;
    private PuzzleAssemblyController assembly;
    private PuzzleSnapPoint[] snapPoints;
    // null означает, что эту кость сейчас никто не держит.
    private int? pointerId;
    private Vector2 grabOffset;

    private void Awake()
    {
        piece = (RectTransform)transform;
        board = (RectTransform)transform.parent;
        assembly = board.GetComponent<PuzzleAssemblyController>();
        snapPoints = GetComponentsInChildren<PuzzleSnapPoint>();
        connectedBones.Add(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // pressPosition хранит место исходного нажатия в экранных координатах.
        // Переводим его в координаты поля, чтобы сравнить с localPosition кости.
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.pressPosition, eventData.pressEventCamera, out Vector2 pressPosition))
            return;

        // Другой палец может уже держать любую из связанных костей.
        if (AnyHeld(connectedBones))
            return;

        pointerId = eventData.pointerId;
        grabOffset = (Vector2)piece.localPosition - pressPosition;

        // Переносим связанные кости поверх остальных элементов поля.
        foreach (PuzzleGroupPiece bone in connectedBones)
            bone.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pointerId != eventData.pointerId)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.position, eventData.pressEventCamera, out Vector2 pointerPosition))
            return;

        // Как в BoneMover, прибавляем смещение захвата к текущей позиции указателя.
        // Разница между новой и старой позицией даёт общий сдвиг для всех костей.
        Vector2 targetPosition = pointerPosition + grabOffset;
        Vector2 delta = targetPosition - (Vector2)piece.localPosition;
        if (!AreCentersInsideBoard(delta))
            return;

        MoveBones(delta);
        TrySnap();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (pointerId == eventData.pointerId)
            pointerId = null;
    }

    // При отключении объекта или потере фокуса событие OnEndDrag может не прийти.
    private void OnDisable() => pointerId = null;

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            pointerId = null;
    }

    private void TrySnap()
    {
        float radiusSquared = snapRadius * snapRadius;

        // Стыковаться может любая кость в подвижной сборке, в том числе та,
        // которую игрок сейчас не держит напрямую.
        foreach (PuzzleGroupPiece bone in connectedBones)
        {
            foreach (PuzzleSnapPoint point in bone.snapPoints)
            {
                PuzzleSnapPoint match = point.MatchingPoint;
                if (match == null || match.MatchingPoint != point)
                    continue;

                PuzzleGroupPiece other = match.Bone;
                // Одинаковая ссылка на список означает, что кости уже в одной сборке.
                if (other == null || other.board != board || other.connectedBones == connectedBones)
                    continue;

                // Сдвигаем свободную точку match к точке point на удерживаемой кости.
                // Переводим сдвиг из мировых координат в координаты поля.
                Vector3 worldDelta = point.transform.position - match.transform.position;
                Vector2 snapDelta = board.InverseTransformVector(worldDelta);
                if (snapDelta.sqrMagnitude > radiusSquared)
                    continue;

                // Подтягиваем только свободную сборку и проверяем её центры после сдвига.
                List<PuzzleGroupPiece> joiningBones = other.connectedBones;
                if (AnyHeld(joiningBones) || !other.AreCentersInsideBoard(snapDelta))
                    continue;

                // Позиция кости под указателем сохраняется, поэтому grabOffset прежний.
                other.MoveBones(snapDelta);

                // Расширяем список и даём его каждой присоединённой кости.
                // Теперь за любую из них можно переместить всю сборку.
                connectedBones.AddRange(joiningBones);
                foreach (PuzzleGroupPiece joinedBone in joiningBones)
                    joinedBone.connectedBones = connectedBones;

                assembly?.ConnectionMade(connectedBones.Count);
                // После первого стыка завершаем поиск до следующего OnDrag.
                return;
            }
        }
    }

    private bool AreCentersInsideBoard(Vector2 delta)
    {
        Rect boardRect = board.rect;
        foreach (PuzzleGroupPiece bone in connectedBones)
        {
            // Кость лежит прямо на поле, а её Pivot в центре картинки.
            // Если хотя бы один центр выйдет за край, отклоняем сдвиг всей сборки.
            Vector2 center = bone.piece.localPosition;
            if (!boardRect.Contains(center + delta))
                return false;
        }

        return true;
    }

    private void MoveBones(Vector2 delta)
    {
        foreach (PuzzleGroupPiece bone in connectedBones)
            bone.piece.localPosition += (Vector3)delta;
    }

    private static bool AnyHeld(List<PuzzleGroupPiece> bones)
    {
        foreach (PuzzleGroupPiece bone in bones)
        {
            if (bone.pointerId.HasValue)
                return true;
        }

        return false;
    }
}
