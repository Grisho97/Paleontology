using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#endif

namespace Paleontology.Teacher
{
    // Only runs when explicitly requested by the command-line validation flag.
    public sealed class TeacherDemoSmokeCheck : MonoBehaviour
    {
#if UNITY_EDITOR
        IEnumerator Start()
        {
            bool requested = SessionState.GetBool("Teacher.Excavation.Validate", false);
            SessionState.SetBool("Teacher.Excavation.Validate", false);
            if (!requested && (!Application.isBatchMode || !System.Array.Exists(System.Environment.GetCommandLineArgs(), a => a == "-teacherValidate"))) yield break;
            yield return null;
            yield return null;
            var soil = GetComponent<SoilSurface>();
            var input = GetComponent<ExcavationInput>();
            Check(soil.Progress == 0, "Initial soil is intact");
            var screen = InputSystem.AddDevice<Touchscreen>();
            Vector2 Point(float x, float y) => RectTransformUtility.WorldToScreenPoint(null,
                soil.Area.TransformPoint(new Vector3(soil.Area.rect.xMin + x * soil.Area.rect.width,
                    soil.Area.rect.yMin + y * soil.Area.rect.height)));
            void Touch(int id, UnityEngine.InputSystem.TouchPhase phase, Vector2 p) =>
                InputSystem.QueueStateEvent(screen, new TouchState { touchId = id, phase = phase, position = p });
            Touch(1, UnityEngine.InputSystem.TouchPhase.Began, Point(0.1f, 0.3f));
            Touch(2, UnityEngine.InputSystem.TouchPhase.Began, Point(0.1f, 0.7f));
            yield return null; yield return null;
            Check(input.ActivePointers == 2, "Two independent fingers accepted");
            Touch(1, UnityEngine.InputSystem.TouchPhase.Moved, Point(0.9f, 0.3f));
            Touch(2, UnityEngine.InputSystem.TouchPhase.Moved, Point(0.9f, 0.7f));
            yield return null; yield return null;
            Check(soil.Progress > 0.06f, "Fast touch strokes erase continuous paths");
            Touch(1, UnityEngine.InputSystem.TouchPhase.Ended, Point(0.9f, 0.3f));
            yield return null; yield return null;
            Check(input.ActivePointers == 1, "Releasing one finger preserves the other");
            Touch(2, UnityEngine.InputSystem.TouchPhase.Ended, Point(0.9f, 0.7f));
            yield return null; yield return null;
            Check(input.ActivePointers == 0, "All fingers released");
            soil.ResetLayer(); input.ClearPointers();
            float before = soil.Progress;
            Touch(3, UnityEngine.InputSystem.TouchPhase.Began, Point(1.15f, 0.5f));
            yield return null; yield return null;
            Touch(3, UnityEngine.InputSystem.TouchPhase.Moved, Point(0.5f, 0.5f));
            yield return null; yield return null;
            Check(soil.Progress == before, "A gesture beginning outside the field does not dig");
            Touch(3, UnityEngine.InputSystem.TouchPhase.Ended, Point(0.5f, 0.5f));
            yield return null; yield return null;
            InputSystem.RemoveDevice(screen);
            var mouse = InputSystem.AddDevice<Mouse>();
            InputSystem.QueueStateEvent(mouse, new MouseState { position = Point(0.1f, 0.5f) }.WithButton(MouseButton.Left));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(mouse, new MouseState { position = Point(0.9f, 0.5f) }.WithButton(MouseButton.Left));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(mouse, new MouseState { position = Point(0.9f, 0.5f) });
            yield return null; yield return null;
            Check(soil.Progress > 0.03f && input.ActivePointers == 0, "Mouse drag erases and releases");
            InputSystem.RemoveDevice(mouse);
            soil.ResetLayer();
            for (float y = 0; y <= 1.01f; y += 0.055f) soil.EraseStroke(new Vector2(0, y), new Vector2(1, y));
            Check(soil.Completed && soil.Progress == 1, "Threshold automatically clears the remaining soil");
            soil.ResetLayer(); input.ClearPointers();
            Check(soil.Progress == 0 && !soil.Completed, "Reset restores the entire layer");
            soil.EraseStroke(new Vector2(0.12f, 0.28f), new Vector2(0.52f, 0.43f));
            soil.EraseStroke(new Vector2(0.5f, 0.43f), new Vector2(0.82f, 0.7f));
            for (int i = 0; i < 5; i++) yield return null;
            Directory.CreateDirectory("Logs/TeacherExcavation");
            ScreenCapture.CaptureScreenshot("Logs/TeacherExcavation/FirstLayerDemo.png");
            for (int i = 0; i < 10; i++) yield return null;
            File.WriteAllText("Logs/TeacherExcavation/Validation.txt", "PASS: initial state, two touches, fast strokes, independent release, outside/UI gesture rejection, mouse, completion, reset.");
            Debug.Log("TEACHER_DEMO_VALIDATION_PASS");
            if (Application.isBatchMode) EditorApplication.Exit(0);
            else EditorApplication.isPlaying = false;
        }
        static void Check(bool value, string message)
        {
            if (value) { Debug.Log("TEACHER_CHECK_PASS: " + message); return; }
            Debug.LogError("TEACHER_CHECK_FAIL: " + message);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else EditorApplication.isPlaying = false;
            throw new System.InvalidOperationException(message);
        }
#endif
    }
}
