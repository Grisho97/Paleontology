using UnityEngine;
using UnityEngine.UI;

public class LayerGridController : MonoBehaviour
{
    private int rawsCount = 0;
    public RawImage[] raws;
    void CheckFinal()
    {
        if(rawsCount == raws.Length)
        {
            Debug.Log("Final");
        }
    }
    public void OnClick(Vector2 clickPosition)
    {
        foreach(RawImage raw in raws)
        {
            if(raw.gameObject.activeSelf == false)
            {
                continue;
            }
            if(RectTransformUtility.RectangleContainsScreenPoint(raw.rectTransform, clickPosition))
            {
                raw.gameObject.SetActive(false);
                rawsCount++;
                CheckFinal();
            }
        }
    }
    void Start()
    {
        raws = gameObject.GetComponentsInChildren<RawImage>(true);
    }

    void Update()
    {
        
    }
}
