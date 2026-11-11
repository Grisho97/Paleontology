using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LayerGridController : MonoBehaviour
{
    [SerializeField]
    int radius;
    [SerializeField, Range(1f, 100f)]
    float autoClearPercent;
    [SerializeField]
    RectTransform brushPrefab;
    [SerializeField]
    RectTransform rawsParent;
    [SerializeField]
    PikedTool pikedTool1;
    RectTransform brush;
    public bool isFinal = false;
    private int removedRawsCount = 0;
    public RawImage[] raws;
    public void HideBrush()
    {
        brush.gameObject.SetActive(false);
    }
    void ShowBrush(Vector2 clickPosition)
    {
        brush.position = clickPosition;
        brush.gameObject.SetActive(true);
    }
    void CheckFinal()
    {
        if(raws.Length == 0 || isFinal)
        {
            return;
        }
        float currentPrecent = 100f / (float)raws.Length * removedRawsCount;
        if(currentPrecent >= autoClearPercent)
        {
            foreach(RawImage raw in raws)
            {
                raw.gameObject.SetActive(false);
                isFinal = true;
            }
            HideBrush();
            Debug.Log("final");
        }
    }
    public void OnClick(Vector2 clickPosition, PikedTool pikedTool)
    {
        if(isFinal || pikedTool != pikedTool1)
        {
            return;
        }
        ShowBrush(clickPosition);
        foreach(RawImage raw in raws)
        {
            if(raw.gameObject.activeSelf == false)
            {
                continue;
            }
            Vector2 rawCenterOnScreen = RectTransformUtility.WorldToScreenPoint(null, raw.rectTransform.position);
            if(Vector2.Distance(clickPosition, rawCenterOnScreen) <= radius)
            {
                raw.gameObject.SetActive(false);
                removedRawsCount++;
            }
        }
        CheckFinal();
    }
    void Awake()
    {
        raws = gameObject.GetComponentsInChildren<RawImage>(true);
        if(brushPrefab != null)
        {
            brush = Instantiate(brushPrefab, rawsParent);
            HideBrush();
        }
    }

    void Update()
    {
        
    }
}
