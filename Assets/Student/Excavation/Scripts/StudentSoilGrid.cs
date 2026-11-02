using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Student
{
    public class StudentSoilGrid : MonoBehaviour
    {
        [SerializeField] RectTransform cellsRoot;
        RawImage[] cells;
        int removed;
        public int Removed => removed;
        public int Total => cells.Length;
        public float Progress => (float)removed / cells.Length;
        public bool Completed => removed == cells.Length;
        public void Configure(RectTransform root) { cellsRoot = root; }

        void Awake()
        {
            // Клетки уже расставлены в префабе. Создавать их в коде не нужно.
            cells = cellsRoot.GetComponentsInChildren<RawImage>(true);
        }

        public void EraseAt(Vector2 screenPosition)
        {
            foreach (RawImage cell in cells)
            {
                if (!cell.gameObject.activeSelf) continue;
                if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, screenPosition))
                {
                    cell.gameObject.SetActive(false);
                    removed++;
                }
            }
        }

        public void ResetGrid()
        {
            foreach (RawImage cell in cells) cell.gameObject.SetActive(true);
            removed = 0;
        }
    }
}
