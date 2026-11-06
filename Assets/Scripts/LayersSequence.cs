using NUnit.Framework;
using UnityEngine;

public class LayersSequence : MonoBehaviour
{
    [SerializeField]
    LayerGridController[] layers;
    public LayerGridController GetCurrentLayer()
    {
        foreach(LayerGridController layer in layers)
        {
            if(layer.isFinal == false)
            {
            return layer;
            }
        }
        return null;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
