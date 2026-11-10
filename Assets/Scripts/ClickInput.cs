using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickInput : MonoBehaviour
{
    public ToolSelecter toolSelecter;
    public LayersSequence layersSequence;
    void Start()
    {
        
    }

    void Update()
    {
        if(Mouse.current.leftButton.isPressed == true || Mouse.current.leftButton.wasPressedThisFrame == true)
        {
            if(layersSequence.GetCurrentLayer() != null && layersSequence.GetCurrentLayer().isActiveAndEnabled == true)
            layersSequence.GetCurrentLayer().OnClick(Mouse.current.position.ReadValue(), toolSelecter.GetPikedTool());
        }
        else
        {
            if(layersSequence.GetCurrentLayer() != null && layersSequence.GetCurrentLayer().isActiveAndEnabled == true)
            layersSequence.GetCurrentLayer().HideBrush();
        }
    }
}
