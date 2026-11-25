using System;
using UnityEngine;

public class ExcavationStageController : MonoBehaviour
{
    [SerializeField]
    GameObject bonePuzzleScreen;
    [SerializeField]
    GameObject gameInterface;
    [SerializeField]
    ClickInput clickInput;
    [SerializeField]
    LayersSequence layersSequence;
    [SerializeField]
    ToolSelecter toolSelecter;
    public event Action ExcavationStageFinal;
    public void Begin()
    {
        gameInterface.SetActive(true);
        bonePuzzleScreen.SetActive(true);
        gameInterface.GetComponent<Animator>().SetTrigger("Trigger2");
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
