using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public sealed class RockExcavationVisual : MonoBehaviour
{
    private static readonly int DigMask = Shader.PropertyToID("_DigMask");
    private static readonly Color32 Intact = new Color32(255, 255, 255, 255);
    private static readonly Color32 Removed = new Color32(0, 0, 0, 255);

    [SerializeField] private RectTransform cellsRoot;

    private struct Cell
    {
        public RawImage Graphic;
        public int MaskIndex;
        public bool WasVisible;
    }

    private Image surface;
    private Material originalMaterial;
    private Material surfaceMaterial;
    private Texture2D mask;
    private Color32[] pixels;
    private Cell[] cells;

    private void OnEnable()
    {
        surface = GetComponent<Image>();
        originalMaterial = surface.material;
        surface.enabled = false;

        if (cellsRoot == null || !originalMaterial.HasProperty(DigMask))
            return;

        var graphics = cellsRoot.GetComponentsInChildren<RawImage>(true);
        if (graphics.Length == 0)
            return;

        // The existing RawImages already encode their positions in the full layer's UVs.
        Rect firstTile = graphics[0].uvRect;
        if (firstTile.width <= 0 || firstTile.height <= 0)
            return;

        int columns = Mathf.RoundToInt(1f / firstTile.width);
        int rows = Mathf.RoundToInt(1f / firstTile.height);
        if (columns <= 0 || rows <= 0 || columns * rows != graphics.Length)
            return;

        cells = new Cell[graphics.Length];
        pixels = new Color32[graphics.Length];
        for (int i = 0; i < graphics.Length; i++)
        {
            RawImage graphic = graphics[i];
            Vector2 uv = graphic.uvRect.center;
            int x = Mathf.FloorToInt(uv.x * columns);
            int y = Mathf.FloorToInt(uv.y * rows);
            int index = y * columns + x;
            if (x < 0 || x >= columns || y < 0 || y >= rows || pixels[index].a != 0)
                return;

            cells[i] = new Cell { Graphic = graphic, MaskIndex = index, WasVisible = graphic.enabled };
            pixels[index] = graphic.gameObject.activeSelf ? Intact : Removed;
        }

        mask = new Texture2D(columns, rows, TextureFormat.RGBA32, false, true)
        {
            name = "Rock excavation mask",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        UploadMask();

        surfaceMaterial = new Material(originalMaterial);
        surfaceMaterial.SetTexture(DigMask, mask);
        surface.material = surfaceMaterial;
        surface.enabled = true;

        foreach (Cell cell in cells)
            cell.Graphic.enabled = false;
    }

    private void LateUpdate()
    {
        if (mask == null)
            return;

        bool changed = false;
        foreach (Cell cell in cells)
        {
            // activeSelf is also the state used for digging, including resets and automatic clearing.
            Color32 state = cell.Graphic.gameObject.activeSelf ? Intact : Removed;
            if (pixels[cell.MaskIndex].r == state.r)
                continue;

            pixels[cell.MaskIndex] = state;
            changed = true;
        }

        if (changed)
            UploadMask();
    }

    private void UploadMask()
    {
        mask.SetPixels32(pixels);
        mask.Apply(false);
    }

    private void OnDisable()
    {
        if (mask == null)
            return;

        surface.enabled = false;
        surface.material = originalMaterial;
        foreach (Cell cell in cells)
        {
            if (cell.Graphic != null)
                cell.Graphic.enabled = cell.WasVisible;
        }

        Destroy(surfaceMaterial);
        Destroy(mask);
        mask = null;
    }
}
