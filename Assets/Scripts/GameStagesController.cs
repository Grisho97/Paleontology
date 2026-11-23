using UnityEngine;

public class GameStagesController : MonoBehaviour
{
    [SerializeField]
    ExcavationStageController excavationStageController;
    [SerializeField]
    PuzzleStageController puzzleStageController;
    void OnExcavationStageFinal()
    {
        puzzleStageController.Begin();
    }
    void Start()
    {
        excavationStageController.Begin();
    }
    void OnEnable()
    {
        excavationStageController.ExcavationStageFinal += OnExcavationStageFinal;
    }

    void OnDisable()
    {
        excavationStageController.ExcavationStageFinal -= OnExcavationStageFinal;
    }
}
