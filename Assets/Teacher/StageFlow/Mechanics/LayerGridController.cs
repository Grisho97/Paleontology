using System;
using UnityEngine;
using UnityEngine.UI;

namespace Teacher
{
    public class LayerGridController : MonoBehaviour
    {
        [SerializeField] private int radius;
        [SerializeField, Range(1f, 100f)] private float autoClearPercent;
        [SerializeField] private RectTransform brushPrefab;
        [SerializeField] private RectTransform rawsParent;
        [SerializeField] private PikedTool pikedTool1;

        private RectTransform brush;
        private RawImage[] tiles;
        private int clearedTiles;

        public bool IsFinished { get; private set; }
        public event Action Finished;

        private void Awake()
        {
            tiles = GetComponentsInChildren<RawImage>(true);
            brush = Instantiate(brushPrefab, rawsParent);
            HideBrush();
        }

        public void HideBrush()
        {
            brush.gameObject.SetActive(false);
        }

        public void OnClick(Vector2 position, PikedTool tool)
        {
            if (IsFinished || tool != pikedTool1)
                return;

            brush.position = position;
            brush.gameObject.SetActive(true);

            foreach (RawImage tile in tiles)
            {
                if (!tile.gameObject.activeSelf)
                    continue;

                Vector2 tilePosition = RectTransformUtility.WorldToScreenPoint(null, tile.rectTransform.position);
                if (Vector2.Distance(position, tilePosition) <= radius)
                {
                    tile.gameObject.SetActive(false);
                    clearedTiles++;
                }
            }

            if (tiles.Length > 0 && 100f * clearedTiles / tiles.Length >= autoClearPercent)
            {
                IsFinished = true;
                foreach (RawImage tile in tiles)
                {
                    tile.gameObject.SetActive(false);
                }

                HideBrush();
                Finished?.Invoke();
            }
        }
    }
}
