using System;
using NUnit.Framework;
using UnityEngine;

public class LayersSequence : MonoBehaviour
{
    [SerializeField]
    LayerGridController[] layers;
    public event Action AllLayersFinished;
    void OnLayerFinished()
    {
        if(IsEveryLayerFinal())
        AllLayersFinished.Invoke();
    }
    public bool IsEveryLayerFinal()
    {
        foreach(LayerGridController layer in layers)
        {
            if(layer.isFinal == false)
            {
            return false;
            }
        }
        return true;
    }
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
    void OnDestroy()
    {
        foreach(LayerGridController layer in layers)
        {
            layer.LayerFinished -= OnLayerFinished;
        }
    }
    void Start()
    {
        foreach(LayerGridController layer in layers)
        {
            layer.LayerFinished += OnLayerFinished;
        }
    }

    void Update()
    {
        
    }
}
