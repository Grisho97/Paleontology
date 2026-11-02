using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Teacher
{
    public sealed class ExcavationHUD : MonoBehaviour
    {
        [SerializeField] SoilSurface surface;
        [SerializeField] ExcavationInput input;
        [SerializeField] Text status, pointers, radiusLabel;
        [SerializeField] Image progress;
        [SerializeField] Button reset, smaller, larger;

        public void Configure(SoilSurface soil, ExcavationInput controls, Text state, Text touches,
            Text radius, Image bar, Button resetButton, Button minus, Button plus)
        {
            surface = soil; input = controls; status = state; pointers = touches;
            radiusLabel = radius; progress = bar; reset = resetButton; smaller = minus; larger = plus;
        }
        void Start()
        {
            reset.onClick.AddListener(() => { input.ClearPointers(); surface.ResetLayer(); });
            smaller.onClick.AddListener(() => surface.BrushRadius -= 10);
            larger.onClick.AddListener(() => surface.BrushRadius += 10);
        }
        void Update()
        {
            float value = surface.Progress;
            progress.rectTransform.anchorMax = new Vector2(value, 1);
            status.text = surface.Completed ? "Слой очищен!" : $"Очищено: {value:P0}";
            pointers.text = $"Активных касаний: {input.ActivePointers}";
            radiusLabel.text = $"Радиус: {surface.BrushRadius:0}";
        }
    }
}
