using UnityEngine;

public class GameStagesController : MonoBehaviour
{
    [SerializeField]
    StartScreenContntroller startScreenContntroller;
    [SerializeField]
    ExcavationStageController excavationStageController;
    [SerializeField]
    PuzzleStageController puzzleStageController;
    void OnStartScreenStageEnded()
    {
        excavationStageController.Begin();
    }
    void OnExcavationStageFinal()
    {
        puzzleStageController.Begin();
    }
    void Start()
    {
        startScreenContntroller.Begin();
    }
    void OnEnable()
    {
        startScreenContntroller.StartScreenStageEnded += OnStartScreenStageEnded;
        excavationStageController.ExcavationStageFinal += OnExcavationStageFinal;
    }

    void OnDisable()
    {
        startScreenContntroller.StartScreenStageEnded -= OnStartScreenStageEnded;
        excavationStageController.ExcavationStageFinal -= OnExcavationStageFinal;
    }
}
