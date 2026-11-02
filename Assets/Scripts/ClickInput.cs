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
        if(Mouse.current.leftButton.isPressed == true)
        {
            layerGridController.OnClick(Mouse.current.position.ReadValue());
        }
    }
}
