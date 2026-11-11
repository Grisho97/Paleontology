using UnityEngine;
using UnityEngine.EventSystems;

// Урок 9. EventSystem вызывает обработчики начала, движения и завершения жеста.
// Один экземпляр компонента управляет одной деталью на поле Canvas.
[RequireComponent(typeof(RectTransform))]
public sealed class PuzzlePieceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Ссылка на правило стыковки из урока 10. Если поле пустое, деталь просто перетаскивается.
    [SerializeField] private PuzzleConnection connection;

    private RectTransform piece;
    private RectTransform board;
    // null означает, что деталь свободна. Во время жеста храним ID мыши или конкретного касания.
    private int? pointerId;
    // Вектор от точки нажатия до опорной точки детали в координатах поля.
    private Vector2 grabOffset;
    private bool joined;

    public bool IsHeld => pointerId.HasValue;

    private void Awake()
    {
        // Все детали лежат прямо внутри Puzzle Board. Поэтому localPosition детали
        // измеряется в той же системе координат, что и границы поля.
        piece = (RectTransform)transform;
        board = (RectTransform)transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Соединённую или уже захваченную деталь нельзя захватить снова.
        // EventSystem передаёт касание как основное нажатие, как и левую кнопку мыши.
        if (joined || IsHeld || eventData.button != PointerEventData.InputButton.Left)
            return;

        // pressPosition хранит место исходного нажатия. К началу перетаскивания
        // указатель уже мог пройти несколько пикселей, преодолевая порог начала жеста.
        // Переводим экранные пиксели в координаты поля. У Overlay Canvas камера равна null.
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.pressPosition, eventData.pressEventCamera, out Vector2 press))
            return;

        pointerId = eventData.pointerId;
        // Запоминаем, где была опорная точка относительно указателя, чтобы деталь
        // не перескочила центром под курсор при захвате за край.
        grabOffset = (Vector2)piece.localPosition - press;
        // Canvas рисует последний дочерний объект поверх остальных деталей поля.
        piece.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Двигать деталь может только указатель, который начал её перетаскивать.
        if (pointerId != eventData.pointerId)
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                board, eventData.position, eventData.pressEventCamera, out Vector2 pointer))
            return;

        // Восстанавливаем исходную точку захвата и ограничиваем позицию краями поля.
        // После движения проверяем стыковку, пока игрок ещё держит деталь.
        piece.localPosition = ClampToBoard(pointer + grabOffset);
        if (connection != null)
            connection.TrySnap(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Отпускание другого пальца не завершает текущий захват.
        if (pointerId == eventData.pointerId)
            pointerId = null;
    }

    // При отключении объекта или потере фокуса событие отпускания может не прийти.
    // Сбрасываем захват, чтобы деталь можно было взять снова после возвращения.
    private void OnDisable() => pointerId = null;

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            pointerId = null;
    }

    private Vector2 ClampToBoard(Vector2 position)
    {
        Rect field = board.rect;
        Rect bounds = piece.rect;
        // Учитываем весь прямоугольник детали. Например, её левый край находится
        // в position.x + bounds.xMin и должен оставаться правее левой границы поля.
        return new Vector2(
            Mathf.Clamp(position.x, field.xMin - bounds.xMin, field.xMax - bounds.xMax),
            Mathf.Clamp(position.y, field.yMin - bounds.yMin, field.yMax - bounds.yMax));
    }

    // Если ограничение заметно меняет позицию, при точной стыковке деталь выйдет за поле.
    // Маленький допуск учитывает погрешность вычислений с float.
    public bool CanSnapTo(Vector2 position) => (ClampToBoard(position) - position).sqrMagnitude < 0.0001f;

    // После стыковки освобождаем указатель и фиксируем деталь до перезапуска сцены.
    public void LockAt(Vector2 position)
    {
        piece.localPosition = position;
        joined = true;
        pointerId = null;
    }
}
