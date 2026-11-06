using UnityEngine;
using UnityEngine.UI;
public enum PikedTool{Empty, Brush, Shovel, Pickaxe}
public class ToolSelecter : MonoBehaviour
{
    [SerializeField]
    Button brushButton;
    [SerializeField]
    Button shovelButton;
    [SerializeField]
    Button pickaxeButton;
    PikedTool pikedTool = PikedTool.Empty;
    public PikedTool GetPikedTool()
    {
        return pikedTool;
    }
    void SelectTool(PikedTool pikedTool)
    {
        this.pikedTool = pikedTool;
    }
    void Start()
    {
        brushButton.onClick.AddListener(()=>SelectTool(PikedTool.Brush));
        shovelButton.onClick.AddListener(()=>SelectTool(PikedTool.Shovel));
        pickaxeButton.onClick.AddListener(()=>SelectTool(PikedTool.Pickaxe));

    }

    void Update()
    {
        
    }
}
