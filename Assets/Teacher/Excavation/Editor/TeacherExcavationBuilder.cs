using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Paleontology.Teacher.Editor
{
    public static class TeacherExcavationBuilder
    {
        const string Folder = "Assets/Teacher/Excavation";
        static Font font;
        static Sprite disc;
        static readonly Color Ink = new Color32(237, 232, 214, 255);

        [MenuItem("Teacher/Excavation/Create first layer demo")]
        public static void CreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Directory.CreateDirectory(Folder + "/Scenes");
            Directory.CreateDirectory(Folder + "/Art");
            CreateDisc();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Main Camera", typeof(Camera));
            camera.tag = "MainCamera";
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color32(25, 35, 32, 255);
            camera.transform.position = new Vector3(0, 0, -10);
            var canvas = new GameObject("Teacher Excavation Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            var root = canvas.transform;
            var background = Panel(root, "Background", 0, 0, 1920, 1080, new Color32(25, 35, 32, 255));
            background.raycastTarget = false;
            Label(root, "Title", "РАСКОПКИ · ПЕРВЫЙ СЛОЙ", 90, 35, 1700, 58, 38);
            Label(root, "Instruction", "Проводите пальцем или мышью с зажатой левой кнопкой по грунту", 90, 98, 1750, 42, 25);
            Panel(root, "Field frame", 80, 174, 1280, 860, new Color32(111, 94, 65, 255));
            var field = Rect(root, "Excavation field", 90, 184, 1260, 840);
            var lower = field.gameObject.AddComponent<RawImage>();
            lower.texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Excavation/Layer02_HardRock.png");
            lower.raycastTarget = false;
            Bone(field, new Vector2(290, -240), 320, 24);
            Bone(field, new Vector2(850, -270), 280, -32);
            Bone(field, new Vector2(550, -590), 370, -12);
            Bone(field, new Vector2(1030, -640), 190, 65);
            var soilImage = Rect(field, "Erasable soft soil", 0, 0, 1260, 840).gameObject.AddComponent<RawImage>();
            soilImage.raycastTarget = false;
            var logic = new GameObject("Excavation Controller");
            var soil = logic.AddComponent<SoilSurface>();
            soil.Configure(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Excavation/Layer01_SoftSoil.png"), soilImage);
            var input = logic.AddComponent<ExcavationInput>(); input.Configure(soil);
            Label(root, "Layer", "01 / МЯГКИЙ ГРУНТ", 1410, 190, 440, 45, 28);
            Label(root, "Tool", "Инструмент: лопата", 1410, 250, 440, 45, 26);
            Label(root, "Description", "Стирайте верхний слой,\nчтобы открыть находки.\n\nНесколько участников могут\nкопать одновременно.", 1410, 318, 440, 210, 25);
            var status = Label(root, "Progress label", "Очищено: 0%", 1410, 555, 440, 50, 32);
            var track = Panel(root, "Progress track", 1410, 618, 410, 12, new Color32(62, 76, 64, 255));
            var fill = new GameObject("Progress fill", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            fill.transform.SetParent(track.transform, false);
            fill.color = new Color32(204, 172, 100, 255); fill.raycastTarget = false;
            fill.rectTransform.anchorMin = Vector2.zero; fill.rectTransform.anchorMax = Vector2.up;
            fill.rectTransform.offsetMin = fill.rectTransform.offsetMax = Vector2.zero;
            var pointers = Label(root, "Pointers", "Активных касаний: 0", 1410, 653, 440, 45, 23);
            var radius = Label(root, "Brush radius", "Радиус: 52", 1410, 720, 270, 45, 25);
            var minus = Button(root, "Smaller brush", "−", 1690, 715, 60, 55);
            var plus = Button(root, "Larger brush", "+", 1760, 715, 60, 55);
            var reset = Button(root, "Reset excavation", "Начать заново", 1410, 820, 410, 75);
            Label(root, "Note", "При очистке 85% остатки исчезнут.\nКости — условные фигуры для теста.\nРадиус указан в единицах Canvas.", 1410, 927, 450, 100, 19);
            logic.AddComponent<ExcavationHUD>().Configure(soil, input, status, pointers, radius, fill, reset, minus, plus);
            logic.AddComponent<TeacherDemoSmokeCheck>();
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            EditorSceneManager.SaveScene(scene, Folder + "/Scenes/FirstLayerDemo.unity");
            AssetDatabase.SaveAssets();
            Debug.Log("Teacher excavation scene created: " + Folder + "/Scenes/FirstLayerDemo.unity");
        }

        public static void BuildAndValidate()
        {
            CreateScene();
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("Teacher/Excavation/Run automated checks")]
        public static void ValidateScene()
        {
            if (EditorApplication.isPlaying) { Debug.LogWarning("Stop Play Mode before running the checks."); return; }
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(Folder + "/Scenes/FirstLayerDemo.unity");
            SessionState.SetBool("Teacher.Excavation.Validate", true);
            EditorApplication.EnterPlaymode();
        }

        static RectTransform Rect(Transform parent, string name, float x, float y, float width, float height)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1); rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height);
            return rect;
        }
        static Image Panel(Transform parent, string name, float x, float y, float w, float h, Color color)
        {
            var image = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Image>();
            image.color = color; image.raycastTarget = false; return image;
        }
        static Text Label(Transform parent, string name, string value, float x, float y, float w, float h, int size)
        {
            var label = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = font; label.fontSize = size; label.text = value; label.color = Ink;
            label.raycastTarget = false; label.horizontalOverflow = HorizontalWrapMode.Wrap;
            return label;
        }
        static Button Button(Transform parent, string name, string value, float x, float y, float w, float h)
        {
            var image = Panel(parent, name, x, y, w, h, new Color32(90, 113, 82, 255));
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            var text = Label(image.transform, "Label", value, 0, 0, w, h, 26);
            text.alignment = TextAnchor.MiddleCenter; return button;
        }
        static void Bone(Transform parent, Vector2 position, float length, float angle)
        {
            var group = Rect(parent, "Placeholder bone", position.x, -position.y, 1, 1);
            group.localRotation = Quaternion.Euler(0, 0, angle);
            // Simple geometry intentionally communicates that these are temporary finds.
            var color = new Color32(224, 211, 173, 255);
            Panel(group, "Shaft", -length / 2, -16, length, 32, color);
            foreach (float end in new[] { -length / 2, length / 2 })
            foreach (float offset in new[] { -13f, 13f })
            {
                var cap = Panel(group, "Rounded end", end - 26, offset - 26, 52, 52, color);
                cap.sprite = disc;
            }
            Panel(group, "Highlight", -length / 2 + 15, -9, length - 30, 5, new Color32(248, 235, 198, 255));
        }
        static void CreateDisc()
        {
            const string path = Folder + "/Art/PlaceholderDisc.png";
            var texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            var colors = new Color32[64 * 64];
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(32, 32));
                colors[y * 64 + x] = new Color(1, 1, 1, Mathf.Clamp01(32 - distance));
            }
            texture.SetPixels32(colors); texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.mipmapEnabled = false; importer.SaveAndReimport();
            disc = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
