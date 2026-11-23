using System;
using UnityEngine;

namespace Teacher
{
    public class PuzzleFinishController : MonoBehaviour
    {
        [SerializeField] private int totalBones;

        public bool IsFinished { get; private set; }
        public event Action Finished;

        public void CheckCompletion(int connectedBones)
        {
            if (IsFinished || connectedBones != totalBones)
                return;

            IsFinished = true;
            Finished?.Invoke();
        }
    }
}
