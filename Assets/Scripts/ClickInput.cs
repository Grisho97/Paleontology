using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickInput : MonoBehaviour
{
    public LayerGridController layerGridController;
    void Start()
    {
        
    }

    void Update()
    {
        if(Mouse.current.leftButton.isPressed == true || Mouse.current.leftButton.wasPressedThisFrame == true)
        {
            layerGridController.OnClick(Mouse.current.position.ReadValue());
        }
        else
        {
            layerGridController.HideBrush();
        }
    }
}
