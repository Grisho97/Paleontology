using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student.Editor
{
    // Подготовка преподавателя: ученик работает с готовой сценой и StudentSoilGrid.
    public static class StudentExcavationBuilder
    {
        const string ScenePath = "Assets/Student/Excavation/Scenes/StudentFirstLayer.unity";

        [MenuItem("Teacher/Student excavation/Create student scene")]
        public static void CreateScene()
        {
            if (EditorApplication.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Directory.CreateDirectory("Assets/Student/Excavation/Scenes");
            Directory.CreateDirectory("Assets/Student/Excavation/Prefabs");
            // Save a separate copy before changing anything in the demonstration.
            var scene = EditorSceneManager.OpenScene("Assets/Teacher/Excavation/Scenes/FirstLayerDemo.unity");
            EditorSceneManager.SaveScene(scene, ScenePath);
            var canvas = Object.FindAnyObjectByType<Canvas>();
            canvas.name = "Student Excavation Canvas";
            Text Label(string name) => canvas.GetComponentsInChildren<Text>(true).First(t => t.name == name);
            Label("Title").text = "РАСКОПКИ · УЧЕБНАЯ СЕТКА";
            Label("Instruction").text = "Проводите пальцем или мышью с зажатой левой кнопкой — клетки грунта исчезают";
            Label("Layer").text = "01 / СЕТКА 18 × 12";
            Label("Tool").text = "Один объект — одна клетка";
            Label("Description").text = "Коснитесь клетки грунта,\nчтобы выключить её.\n\nПод грунтом находятся\nусловные кости.";
            Label("Progress label").fontSize = 27;
            Label("Progress label").rectTransform.sizeDelta = new Vector2(440, 210);
            Label("Progress label").text = "Сетка готова до запуска Play";
            Label("Note").text = "Готовый префаб SoilGrid.\nОдин палец или мышь.\nБыстрые движения могут оставлять пропуски.";
            foreach (string name in new[] { "Progress track", "Pointers", "Brush radius", "Smaller brush", "Larger brush" })
            {
                var child = canvas.GetComponentsInChildren<Transform>(true).First(t => t.name == name);
                Object.DestroyImmediate(child.gameObject);
            }
            var field = canvas.GetComponentsInChildren<RectTransform>(true).First(t => t.name == "Excavation field");
            var preview = field.GetComponentsInChildren<RawImage>().First(i => i.name == "Erasable soft soil");
            preview.name = "Editor preview (hidden in Play Mode)";
            var reset = canvas.GetComponentsInChildren<Button>().First(b => b.name == "Reset excavation");
            Object.DestroyImmediate(Object.FindAnyObjectByType<Paleontology.Teacher.SoilSurface>().gameObject);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Student/Excavation/Prefabs/SoilGrid.prefab");
            var preparedGrid = (GameObject)PrefabUtility.InstantiatePrefab(prefab, field);
            Object.DestroyImmediate(preview.gameObject);
            var logic = new GameObject("Student Excavation Controller");
            var grid = logic.AddComponent<StudentSoilGrid>();
            var indicator = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Student/Excavation/Prefabs/DigRadiusIndicator.prefab");
            grid.Configure(preparedGrid.GetComponent<RectTransform>(), indicator.GetComponent<RectTransform>());
            var input = logic.AddComponent<StudentDigInput>(); input.Configure(grid);
            logic.AddComponent<StudentDigHUD>().Configure(grid, input, Label("Progress label"), reset);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("STUDENT_SCENE_CREATED: " + ScenePath);
        }

    }
}
