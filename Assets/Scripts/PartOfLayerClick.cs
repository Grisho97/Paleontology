using UnityEngine;
using UnityEngine.EventSystems;

public class PartOfLayerClick : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
