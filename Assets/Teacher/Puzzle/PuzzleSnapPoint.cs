using UnityEngine;

public sealed class PuzzleSnapPoint : MonoBehaviour
{
    // В Inspector достаточно назначить эту ссылку на одной из двух точек.
    [SerializeField] private PuzzleSnapPoint matchingPoint;

    internal PuzzleSnapPoint MatchingPoint => matchingPoint;
    internal PuzzleGroupPiece Bone { get; private set; }

    private void Awake()
    {
        Bone = GetComponentInParent<PuzzleGroupPiece>();
        if (matchingPoint != null && matchingPoint.matchingPoint == null)
            matchingPoint.matchingPoint = this;
    }
}
