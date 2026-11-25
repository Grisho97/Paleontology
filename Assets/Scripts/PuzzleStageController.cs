using UnityEngine;

public class PuzzleStageController : MonoBehaviour
{
    [SerializeField]
    BonesHolder bonesHolder;
    public void Begin()
    {
        bonesHolder.InitializeFields();
    }
    public void End()
    {
        bonesHolder.isFinished = true;
        Debug.Log("Puzzle finished");
    }
    void OnEnable()
    {
        bonesHolder.PuzzleStageFinal += End;
    }

    void OnDisable()
    {
        bonesHolder.PuzzleStageFinal -= End;
    }
}
