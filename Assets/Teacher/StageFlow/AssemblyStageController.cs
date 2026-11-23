using System;
using TMPro;
using UnityEngine;

namespace Teacher
{
    public class AssemblyStageController : MonoBehaviour
    {
        [SerializeField] private PuzzleFinishController puzzleFinishController;
        [SerializeField] private BoneMover[] bones;
        [SerializeField] private TMP_Text instruction;

        public event Action Finished;

        private void OnEnable()
        {
            puzzleFinishController.Finished += OnPuzzleFinished;
        }

        private void OnDisable()
        {
            puzzleFinishController.Finished -= OnPuzzleFinished;
        }

        public void Begin()
        {
            foreach (BoneMover bone in bones)
            {
                bone.enabled = true;
            }

            instruction.text = "Перетащите части скелета, чтобы соединить их.";
        }

        public void End()
        {
            foreach (BoneMover bone in bones)
            {
                bone.enabled = false;
            }

            instruction.text = "Скелет собран!";
        }

        private void OnPuzzleFinished()
        {
            Finished?.Invoke();
        }
    }
}
