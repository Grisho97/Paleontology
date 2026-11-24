using UnityEngine;

public class PuzzleStageController : MonoBehaviour
{
    [SerializeField]
    PuzzleFinishController puzzleFinishController;
    public void Begin()
    {
        puzzleFinishController.isCanMove = true;
        puzzleFinishController.isFinished = false;
    }
    public void End()
    {
        
    }
    void OnEnable()
    {
        puzzleFinishController.PuzzleStageFinal += End;
    }

    void OnDisable()
    {
        puzzleFinishController.PuzzleStageFinal -= End;
    }
}
