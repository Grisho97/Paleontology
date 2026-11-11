using TMPro;
using UnityEngine;

// Урок 10. Компонент описывает одну совместимую пару, например череп и рёбра.
// Каждая точка стыка является дочерним объектом своей детали и движется вместе с ней.
public sealed class PuzzleConnection : MonoBehaviour
{
    [SerializeField] private PuzzlePieceDrag firstPiece;
    [SerializeField] private RectTransform firstPoint;
    [SerializeField] private PuzzlePieceDrag secondPiece;
    [SerializeField] private RectTransform secondPoint;
    // Расстояние задаётся в локальных единицах поля и масштабируется вместе с Canvas.
    [SerializeField, Min(1f)] private float snapRadius = 65f;
    [SerializeField] private TMP_Text status;

    private bool joined;

    public void TrySnap(PuzzlePieceDrag moving)
    {
        // Стыковка выполняется один раз и только между деталями, заданными в Inspector.
        if (joined || (moving != firstPiece && moving != secondPiece))
            return;

        // Игрок может поднести любую деталь пары к другой. Вторую оставляем на месте.
        // Если её сейчас держит другой указатель, откладываем стыковку.
        bool movingFirst = moving == firstPiece;
        PuzzlePieceDrag other = movingFirst ? secondPiece : firstPiece;
        if (other.IsHeld)
            return;

        RectTransform movingPoint = movingFirst ? firstPoint : secondPoint;
        RectTransform otherPoint = movingFirst ? secondPoint : firstPoint;
        Transform board = moving.transform.parent;

        // Обе точки заданы относительно своих деталей. Переводим их мировые позиции
        // в координаты общего поля. Разность показывает, куда и насколько сдвинуть
        // подвижную деталь, чтобы её точка совпала со второй.
        Vector2 difference = board.InverseTransformPoint(otherPoint.position)
            - board.InverseTransformPoint(movingPoint.position);
        // Сравниваем квадрат расстояния с квадратом радиуса. Так не нужно вычислять корень.
        if (difference.sqrMagnitude > snapRadius * snapRadius)
            return;

        // Прибавляем разницу к позиции детали. Угол остаётся прежним.
        // Стыковку у края пропускаем, если после сдвига часть выйдет за границы.
        Vector2 target = (Vector2)moving.transform.localPosition + difference;
        if (!moving.CanSnapTo(target))
            return;

        // Фиксируем обе детали в итоговых позициях. Перемещение группы появится в уроке 11.
        moving.LockAt(target);
        other.LockAt(other.transform.localPosition);
        joined = true;
        if (status != null)
            status.text = "Череп и рёбра соединены";
    }
}
