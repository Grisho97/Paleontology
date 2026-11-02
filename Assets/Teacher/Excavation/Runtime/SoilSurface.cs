using UnityEngine;
using UnityEngine.UI;

namespace Paleontology.Teacher
{
    // A writable runtime copy; the imported source texture is never modified.
    public sealed class SoilSurface : MonoBehaviour
    {
        [SerializeField] Texture2D source;
        [SerializeField] RawImage image;
        [SerializeField, Range(16, 100)] float brushRadius = 52;
        [SerializeField, Range(0.5f, 1)] float completionThreshold = 0.85f;
        Texture2D working;
        Color32[] original, pixels;
        long removedAlpha;
        bool dirty;
        public RectTransform Area => image.rectTransform;
        public float Progress => pixels == null ? 0 : (float)((double)removedAlpha / (pixels.Length * 255L));
        public bool Completed { get; private set; }
        public float BrushRadius { get => brushRadius; set => brushRadius = Mathf.Clamp(value, 16, 100); }

        public void Configure(Texture2D texture, RawImage target) { source = texture; image = target; image.texture = source; }

        void Awake()
        {
            if (!source || !image) { enabled = false; return; }
            // GPU copy avoids requiring Read/Write on the imported texture.
            var temporary = RenderTexture.GetTemporary(768, 512, 0, RenderTextureFormat.ARGB32);
            var previous = RenderTexture.active;
            try
            {
                Graphics.Blit(source, temporary);
                RenderTexture.active = temporary;
                working = new Texture2D(768, 512, TextureFormat.RGBA32, false);
                working.name = "Runtime erasable soil";
                working.ReadPixels(new Rect(0, 0, 768, 512), 0, 0);
                working.Apply(false);
            }
            finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(temporary); }
            working.wrapMode = TextureWrapMode.Clamp;
            working.filterMode = FilterMode.Bilinear;
            original = working.GetPixels32();
            pixels = (Color32[])original.Clone();
            image.texture = working;
        }

        // Normalized field coordinates; input and texture processing stay independent.
        public void EraseStroke(Vector2 from, Vector2 to)
        {
            if (pixels == null || Completed) return;
            var a = Vector2.Scale(from, new Vector2(working.width, working.height));
            var b = Vector2.Scale(to, new Vector2(working.width, working.height));
            float radius = brushRadius / Area.rect.width * working.width;
            int steps = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(a, b) / (radius * 0.35f)));
            for (int i = 0; i <= steps; i++) Stamp(Vector2.Lerp(a, b, (float)i / steps), radius);
            if (Progress >= completionThreshold) FinishLayer();
        }

        void Stamp(Vector2 center, float radius)
        {
            int xmin = Mathf.Max(0, Mathf.FloorToInt(center.x - radius));
            int xmax = Mathf.Min(working.width - 1, Mathf.CeilToInt(center.x + radius));
            int ymin = Mathf.Max(0, Mathf.FloorToInt(center.y - radius));
            int ymax = Mathf.Min(working.height - 1, Mathf.CeilToInt(center.y + radius));
            for (int y = ymin; y <= ymax; y++)
            for (int x = xmin; x <= xmax; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;
                if (distance >= 1) continue;
                byte alpha = (byte)Mathf.RoundToInt(255 * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.72f, 1, distance)));
                int index = y * working.width + x;
                if (alpha >= pixels[index].a) continue;
                removedAlpha += pixels[index].a - alpha;
                pixels[index].a = alpha;
                dirty = true;
            }
        }

        void FinishLayer()
        {
            Completed = true;
            for (int i = 0; i < pixels.Length; i++) pixels[i].a = 0;
            removedAlpha = pixels.Length * 255L;
            dirty = true;
        }

        public void ResetLayer()
        {
            if (original == null) return;
            System.Array.Copy(original, pixels, pixels.Length);
            removedAlpha = 0;
            Completed = false;
            dirty = true;
        }

        void LateUpdate()
        {
            if (!dirty) return;
            working.SetPixels32(pixels);
            working.Apply(false); // At most once per frame, shared by every finger.
            dirty = false;
        }

        void OnDestroy() { if (working) Destroy(working); }
    }
}
