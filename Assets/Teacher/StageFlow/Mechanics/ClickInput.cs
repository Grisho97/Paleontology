using UnityEngine;
using UnityEngine.InputSystem;

namespace Teacher
{
    public class ClickInput : MonoBehaviour
    {
        [SerializeField] private ToolSelecter toolSelecter;
        [SerializeField] private LayersSequence layersSequence;

        private void Update()
        {
            LayerGridController layer = layersSequence.GetCurrentLayer();
            Mouse mouse = Mouse.current;
            if (layer == null || !layer.isActiveAndEnabled || mouse == null)
                return;

            if (mouse.leftButton.isPressed || mouse.leftButton.wasPressedThisFrame)
            {
                layer.OnClick(mouse.position.ReadValue(), toolSelecter.GetPikedTool());
            }
            else
            {
                layer.HideBrush();
            }
        }
    }
}
