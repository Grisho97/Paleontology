using System;
using TMPro;
using UnityEngine;

namespace Teacher
{
    public class ExcavationStageController : MonoBehaviour
    {
        [SerializeField] private GameObject excavationScreen;
        [SerializeField] private GameObject puzzleScreen;
        [SerializeField] private ClickInput clickInput;
        [SerializeField] private LayersSequence layersSequence;
        [SerializeField] private TMP_Text instruction;

        public event Action Finished;

        private void OnEnable()
        {
            layersSequence.Finished += OnLayersFinished;
        }

        private void OnDisable()
        {
            layersSequence.Finished -= OnLayersFinished;
        }

        public void Begin()
        {
            puzzleScreen.SetActive(true);
            excavationScreen.SetActive(true);
            clickInput.enabled = true;
            instruction.text = "Выберите инструмент и проводите по породе.";
            instruction.gameObject.SetActive(true);
        }

        public void End()
        {
            clickInput.enabled = false;
            excavationScreen.SetActive(false);
        }

        private void OnLayersFinished()
        {
            Finished?.Invoke();
        }
    }
}
