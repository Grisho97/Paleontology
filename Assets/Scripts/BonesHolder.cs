using System;
using UnityEngine;

public class BonesHolder : MonoBehaviour
{
    [SerializeField]
    float matchRadius = 60f;
    [SerializeField]
    BoneMover[] boneMovers;
    public bool isFinished = false;
    public event Action PuzzleStageFinal;
    public void InitializeFields()
    {
        foreach(BoneMover bone in boneMovers)
        {
            bone.SetIsCanMove(true);
            bone.SetIsFinished(false);
            bone.matchRadiusLocal = matchRadius;
        }
        isFinished = false;
    }
    public void OnAnyBoneConnected(int count)
    {
        if(isFinished)
        return;
        if(count == boneMovers.Length)
        {
            PuzzleStageFinal.Invoke();
            foreach(BoneMover bone in boneMovers)
            {
                bone.SetIsFinished(false);
            }
            isFinished = false;
        }
    }
    void OnEnable()
    {
        foreach(BoneMover bone in boneMovers)
        {
            bone.AnyBoneConnected += OnAnyBoneConnected;
        }
    }

    void OnDisable()
    {
        foreach(BoneMover bone in boneMovers)
        {
            bone.AnyBoneConnected -= OnAnyBoneConnected;
        }
    }
}
