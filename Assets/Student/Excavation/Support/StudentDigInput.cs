using UnityEngine;
using UnityEngine.InputSystem;

namespace Paleontology.Student
{
    public class StudentDigInput : MonoBehaviour
    {
        [SerializeField] StudentSoilGrid grid;
        public void Configure(StudentSoilGrid target) { grid = target; }

        void Update()
        {
            // На следующем занятии обрабатываем только один палец или мышь.
            var touch = Touchscreen.current?.primaryTouch;
            if (touch != null && (touch.press.isPressed || touch.press.wasPressedThisFrame))
            {
                grid.EraseAt(touch.position.ReadValue());
            }
            else if (Mouse.current != null &&
                     (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasPressedThisFrame))
            {
                grid.EraseAt(Mouse.current.position.ReadValue());
            }
            else
            {
                grid.HideBrush();
            }
        }

        void OnDisable()
        {
            if (grid != null) grid.HideBrush();
        }
    }
}
