using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student
{
    public class StudentDigHUD : MonoBehaviour
    {
        [SerializeField] StudentLayerSequence sequence;
        [SerializeField] Text status;
        [SerializeField] Button reset;
        public void Configure(StudentLayerSequence target, Text label, Button button)
        { sequence = target; status = label; reset = button; }
        void Start() { reset.onClick.AddListener(sequence.ResetLayers); }
        void Update()
        {
            StudentSoilGrid grid = sequence.CurrentLayer;
            if (grid == null)
            {
                status.text = "Раскопка завершена!";
                return;
            }

            string toolName = grid.RequiredTool switch
            {
                DigTool.Shovel => "лопата",
                DigTool.Pickaxe => "кирка",
                DigTool.Brush => "кисточка",
                _ => "—"
            };
            status.text = $"Инструмент: {toolName}\nУбрано: {grid.Removed} / {grid.Total}\nОчищено: {grid.Progress:P0}";
        }
    }
}
