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
            Label("Instruction").text = "Выберите инструмент и ведите мышью по грунту с зажатой ЛКМ.";
            Label("Layer").text = "ТРИ СЛОЯ ПОРОДЫ";
            Label("Tool").text = "Выберите инструмент";
            Label("Description").text = "Очищайте слои по очереди.";
            Label("Progress label").fontSize = 27;
            Label("Progress label").rectTransform.sizeDelta = new Vector2(440, 210);
            Label("Progress label").text = "Сетка готова до запуска Play";
            Label("Note").text = "Лопата → кирка → кисточка.";
            foreach (string name in new[] { "Progress track", "Pointers", "Brush radius", "Smaller brush", "Larger brush" })
            {
                var child = canvas.GetComponentsInChildren<Transform>(true).First(t => t.name == name);
                Object.DestroyImmediate(child.gameObject);
            }
            var field = canvas.GetComponentsInChildren<RectTransform>(true).First(t => t.name == "Excavation field");
            var preview = field.GetComponentsInChildren<RawImage>().First(i => i.name == "Erasable soft soil");
            preview.name = "Editor preview (hidden in Play Mode)";
            var reset = canvas.GetComponentsInChildren<Button>().First(b => b.name == "Reset excavation");
            Button ToolButton(string name, string title, float y)
            {
                var button = Object.Instantiate(reset, reset.transform.parent);
                button.name = name;
                var rect = (RectTransform)button.transform;
                rect.anchoredPosition = new Vector2(1410, -y);
                rect.sizeDelta = new Vector2(410, 56);
                var label = button.GetComponentInChildren<Text>();
                label.text = title;
                label.rectTransform.sizeDelta = rect.sizeDelta;
                return button;
            }
            Object.DestroyImmediate(Object.FindAnyObjectByType<Paleontology.Teacher.SoilSurface>().gameObject);
            StudentSoilGrid AddLayer(string name)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Student/Excavation/Prefabs/{name}.prefab");
                var layer = (GameObject)PrefabUtility.InstantiatePrefab(prefab, field);
                return layer.GetComponent<StudentSoilGrid>();
            }
            // В Canvas верхний слой рисуется последним.
            var boneMatrix = AddLayer("BoneMatrixLayer");
            var hardRock = AddLayer("HardRockLayer");
            var softSoil = AddLayer("SoftSoilLayer");
            Object.DestroyImmediate(preview.gameObject);
            var logic = new GameObject("Student Excavation Controller");
            var sequence = logic.AddComponent<StudentLayerSequence>();
            sequence.Configure(new[] { softSoil, hardRock, boneMatrix });
            var tools = logic.AddComponent<StudentToolSelection>();
            tools.Configure(ToolButton("Shovel", "Лопата", 350),
                ToolButton("Pickaxe", "Кирка", 417), ToolButton("Brush", "Кисточка", 484));
            var input = logic.AddComponent<StudentDigInput>(); input.Configure(sequence, tools);
            logic.AddComponent<StudentDigHUD>().Configure(sequence, Label("Progress label"), reset);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("STUDENT_SCENE_CREATED: " + ScenePath);
        }

    }
}
