using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Paleontology.Student
{
    public class StudentDigInput : MonoBehaviour
    {
        [SerializeField] StudentLayerSequence sequence;
        [SerializeField] StudentToolSelection toolSelection;

        public void Configure(StudentLayerSequence target, StudentToolSelection selection)
        {
            sequence = target;
            toolSelection = selection;
        }

        void Update()
        {
            StudentSoilGrid grid = sequence.CurrentLayer;
            if (grid == null) return;

            var mouse = Mouse.current;
            bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (mouse != null && !overUi &&
                (mouse.leftButton.isPressed || mouse.leftButton.wasPressedThisFrame))
            {
                grid.EraseAt(mouse.position.ReadValue(), toolSelection.SelectedTool);
            }
            else
            {
                grid.HideBrush();
            }
        }

        void OnDisable()
        {
            if (sequence != null && sequence.CurrentLayer != null)
                sequence.CurrentLayer.HideBrush();
        }
    }
}
