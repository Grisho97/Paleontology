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
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                grid.EraseAt(Touchscreen.current.primaryTouch.position.ReadValue());
            }
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                grid.EraseAt(Mouse.current.position.ReadValue());
            }
        }
    }
}
