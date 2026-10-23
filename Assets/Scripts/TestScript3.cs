using UnityEngine;

public class TestScript3 : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Color color = Color.red;
    [SerializeField]
    private float size = 1f;
    private void ChangeView()
    {
        spriteRenderer.color = color;
        spriteRenderer.gameObject.transform.localScale = new Vector3(size, size, 1f);
        Debug.Log("Color changed");
    }
    void Start()
    {
        if(spriteRenderer == null)
        {
            Debug.Log("Нет компонента SpriteRenderer");
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        ChangeView();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
