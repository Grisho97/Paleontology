using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public sealed class PuzzleAssemblyController : MonoBehaviour
{
    [SerializeField] private TMP_Text status;

    private int totalBones;
    private bool completed;

    private void Awake()
    {
        totalBones = GetComponentsInChildren<PuzzleGroupPiece>(true).Length;
    }

    internal void ConnectionMade(int connectedCount)
    {
        if (completed)
            return;

        // После стыка кость передаёт размер своей сборки.
        completed = connectedCount == totalBones;
        if (status != null)
            status.text = completed
                ? "Скелет собран. Перемещайте его за любую кость."
                : $"В группе: {connectedCount} из {totalBones}. Можно продолжать перемещение.";
    }
}
