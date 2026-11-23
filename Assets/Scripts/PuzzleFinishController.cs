using UnityEngine;

public class PuzzleFinishController : MonoBehaviour
{
    public bool isFinished = false;
    public bool isCanMove = false;
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
