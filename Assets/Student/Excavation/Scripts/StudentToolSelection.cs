using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student
{
    public enum DigTool { None, Shovel, Pickaxe, Brush }

    public class StudentToolSelection : MonoBehaviour
    {
        [SerializeField] Button shovelButton;
        [SerializeField] Button pickaxeButton;
        [SerializeField] Button brushButton;
        [SerializeField] Color normalColor = new Color(0.35f, 0.44f, 0.32f);
        [SerializeField] Color selectedColor = new Color(0.65f, 0.42f, 0.18f);

        public DigTool SelectedTool { get; private set; }

        public void Configure(Button shovel, Button pickaxe, Button brush)
        {
            shovelButton = shovel;
            pickaxeButton = pickaxe;
            brushButton = brush;
        }

        void Start()
        {
            shovelButton.onClick.AddListener(() => SelectTool(DigTool.Shovel));
            pickaxeButton.onClick.AddListener(() => SelectTool(DigTool.Pickaxe));
            brushButton.onClick.AddListener(() => SelectTool(DigTool.Brush));
            SelectTool(DigTool.None);
        }

        void SelectTool(DigTool tool)
        {
            SelectedTool = tool;
            shovelButton.image.color = tool == DigTool.Shovel ? selectedColor : normalColor;
            pickaxeButton.image.color = tool == DigTool.Pickaxe ? selectedColor : normalColor;
            brushButton.image.color = tool == DigTool.Brush ? selectedColor : normalColor;
        }
    }
}
