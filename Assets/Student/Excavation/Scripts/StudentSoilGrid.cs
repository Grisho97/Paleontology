using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student
{
    public class StudentSoilGrid : MonoBehaviour
    {
        [SerializeField] RectTransform cellsRoot;
        [SerializeField] DigTool requiredTool = DigTool.Shovel;
        [SerializeField, Min(0f), Tooltip("Радиус очистки в экранных пикселях.")]
        float digRadius = 90f;
        [SerializeField, Range(1, 100)] int autoClearPercent = 85;
        [SerializeField] RectTransform brushPrefab;
        RectTransform brush;
        RawImage[] cells;
        int removed;
        public int Removed => removed;
        public int Total => cells.Length;
        public float Progress => (float)removed / cells.Length;
        public bool Completed => removed == cells.Length;
        public DigTool RequiredTool => requiredTool;
        public void Configure(RectTransform root, RectTransform indicatorPrefab)
        {
            cellsRoot = root;
            brushPrefab = indicatorPrefab;
        }

        void Awake()
        {
            // Клетки уже расставлены в префабе. Создавать их в коде не нужно.
            cells = cellsRoot.GetComponentsInChildren<RawImage>(true);
            if (brushPrefab != null)
            {
                brush = Instantiate(brushPrefab, cellsRoot);
                HideBrush();
            }
        }

        public void EraseAt(Vector2 screenPosition, DigTool selectedTool)
        {
            if (selectedTool == DigTool.None || selectedTool != requiredTool || Completed)
            {
                HideBrush();
                return;
            }

            ShowBrush(screenPosition);

            foreach (RawImage cell in cells)
            {
                if (!cell.gameObject.activeSelf) continue;
                // null подходит для Canvas в режиме Screen Space - Overlay.
                Vector2 centerOnScreen = RectTransformUtility.WorldToScreenPoint(
                    null, cell.rectTransform.position);

                if (Vector2.Distance(screenPosition, centerOnScreen) <= digRadius)
                {
                    cell.gameObject.SetActive(false);
                    removed++;
                }
            }

            if (!Completed && removed * 100 >= Total * autoClearPercent)
            {
                foreach (RawImage cell in cells) cell.gameObject.SetActive(false);
                removed = Total;
            }

            if (Completed) HideBrush();
        }

        void ShowBrush(Vector2 screenPosition)
        {
            if (brush == null) return;

            // На Canvas Overlay мировая позиция соответствует позиции на экране.
            brush.position = screenPosition;
            float diameter = digRadius * 2f;
            brush.sizeDelta = new Vector2(diameter / Mathf.Abs(cellsRoot.lossyScale.x),
                diameter / Mathf.Abs(cellsRoot.lossyScale.y));
            brush.gameObject.SetActive(true);
        }

        public void HideBrush()
        {
            if (brush != null) brush.gameObject.SetActive(false);
        }

        public void ResetGrid()
        {
            HideBrush();
            foreach (RawImage cell in cells) cell.gameObject.SetActive(true);
            removed = 0;
        }
    }
}
