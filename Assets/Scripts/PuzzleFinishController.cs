using UnityEngine;

public class PuzzleFinishController : MonoBehaviour
{
    [SerializeField]
    LayersSequence layersSequence;
    public bool isFinished{get; private set;} = false;
    public bool isCanMove => layersSequence.IsEveryLayerFinal();
    [SerializeField]
    int totalBones;
    public void IsPuzzleFinished(int connectedBones)
    {
        if(connectedBones == totalBones)
        {
            isFinished = true;
            Debug.Log("Puzzle finished");
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
