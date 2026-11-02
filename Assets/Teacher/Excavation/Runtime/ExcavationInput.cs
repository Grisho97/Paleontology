using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Paleontology.Teacher
{
    public sealed class ExcavationInput : MonoBehaviour
    {
        [SerializeField] SoilSurface surface;
        readonly Dictionary<int, Vector2> previous = new Dictionary<int, Vector2>();
        readonly HashSet<int> accepted = new HashSet<int>();
        public int ActivePointers => accepted.Count;
        public void Configure(SoilSurface target) { surface = target; }
        void OnEnable() { EnhancedTouchSupport.Enable(); }
        void OnDisable() { ClearPointers(); EnhancedTouchSupport.Disable(); }
        void OnApplicationFocus(bool focus) { if (!focus) ClearPointers(); }
        public void ClearPointers() { previous.Clear(); accepted.Clear(); }

        void Update()
        {
            if (!surface) return;
            foreach (var touch in Touch.activeTouches)
            {
                int id = touch.touchId;
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) Begin(id, touch.screenPosition);
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) Move(id, touch.screenPosition);
                else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended) Move(id, touch.screenPosition);
                    End(id);
                }
            }
            // Do not process an emulated mouse alongside real fingers.
            var mouse = Mouse.current;
            if (Touch.activeTouches.Count > 0) { End(-1); return; }
            if (mouse == null) return;
            if (mouse.leftButton.wasPressedThisFrame) Begin(-1, mouse.position.ReadValue());
            else if (mouse.leftButton.isPressed) Move(-1, mouse.position.ReadValue());
            if (mouse.leftButton.wasReleasedThisFrame) { Move(-1, mouse.position.ReadValue()); End(-1); }
        }

        bool GetPoint(Vector2 screen, out Vector2 normalized)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(surface.Area, screen, null, out var local);
            var rect = surface.Area.rect;
            normalized = new Vector2((local.x - rect.xMin) / rect.width, (local.y - rect.yMin) / rect.height);
            return rect.Contains(local);
        }

        void Begin(int id, Vector2 position)
        {
            if (!GetPoint(position, out var point)) return;
            accepted.Add(id); previous[id] = point;
            surface.EraseStroke(point, point);
        }

        void Move(int id, Vector2 position)
        {
            if (!accepted.Contains(id)) return;
            if (!GetPoint(position, out var point)) { previous.Remove(id); return; }
            var from = previous.TryGetValue(id, out var last) ? last : point;
            surface.EraseStroke(from, point);
            previous[id] = point;
        }
        void End(int id) { accepted.Remove(id); previous.Remove(id); }
    }
}
