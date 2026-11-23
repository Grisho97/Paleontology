using System;
using UnityEngine;

public class ExcavationStageController : MonoBehaviour
{
    [SerializeField]
    ClickInput clickInput;
    [SerializeField]
    LayersSequence layersSequence;
    [SerializeField]
    ToolSelecter toolSelecter;
    public event Action ExcavationStageFinal;
    public void Begin()
    {
        clickInput.enabled = true;
        layersSequence.enabled = true;
        toolSelecter.enabled = true;
    }
    public void End()
    {
        clickInput.enabled = false;
        layersSequence.enabled = false;
        toolSelecter.enabled = false;
        ExcavationStageFinal.Invoke();
    }
    void Start()
    {
        layersSequence.AllLayersFinished += End;
    }

    void OnDestroy()
    {
        layersSequence.AllLayersFinished -= End;
    }
}
