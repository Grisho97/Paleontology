using System;
using UnityEngine;

public class PuzzleFinishController : MonoBehaviour
{
    [SerializeField]
    BoneMover boneMover;
    public bool isFinished = false;
    public bool isCanMove = false;
    [SerializeField]
    int totalBones;
    public event Action PuzzleStageFinal;
    public void OnAnyBoneConnected()
    {
        if(boneMover.connectedPuzzlePeaces.Count == totalBones)
        PuzzleStageFinal.Invoke();
    }
    public void IsPuzzleFinished(int connectedBones)
    {
        if(connectedBones == totalBones)
        {
            isFinished = true;
            Debug.Log("Puzzle finished");
        }
    }
    void OnEnable()
    {
        boneMover.AnyBoneConnected += OnAnyBoneConnected;
    }

    void OnDisable()
    {
        boneMover.AnyBoneConnected -= OnAnyBoneConnected;
    }
}
