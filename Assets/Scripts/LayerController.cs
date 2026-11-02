using UnityEngine;
using UnityEngine.UI;

public class LayerController : MonoBehaviour
{
    public RawImage[] raws;
    void CheckFinal()
    {
        bool isFinal = true;
        foreach(RawImage raw in raws)
        {
            if(raw.gameObject.activeSelf == true)
            {
                isFinal = false;
                break;
            }
        }
        if(isFinal){
        Debug.Log("Finaled");
        }
    }
    void Start()
    {
        raws = gameObject.GetComponentsInChildren<RawImage>(true);
    }

    void Update()
    {
        CheckFinal();
    }
}
