using UnityEngine;
using UnityEngine.EventSystems;

public enum CursorTool { hold, trowel, extract, secateur }
public class PlayerInputs : MonoBehaviour
{
    Camera mainCamera;
    public FollowCursor heldObject = null;
    public CursorTool tool = CursorTool.hold;
    AudioManager audioManager;

    Texture2D normalTexture = null;
    [SerializeField]
    Texture2D disabledTexture;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        audioManager = FindObjectOfType<AudioManager>();
     
    }

    private void OnEnable()
    {
        Cursor.SetCursor(normalTexture, Vector2.zero, CursorMode.Auto);
    }
    private void OnDisable()
    {
        Cursor.SetCursor(disabledTexture, Vector2.one * 8, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() )
        {
            Vector2 clickPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(clickPoint, Vector3.forward, 100);
            if (hit && hit.collider.GetComponent<IClickable>() is IClickable clickable)
            {
                Debug.Log("Clicked on " + hit.collider.name);
                audioManager.Play("Button1");
                audioManager.PlayTheme("ThemeDrums", 4);
                clickable.OnClick(tool);
            }
        }
        if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
        {
            // old drop implementation
            if (heldObject != null && heldObject.droppable)
            {
                audioManager.Play("Button2");
                heldObject.MouseDrop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete) && tool == CursorTool.hold && heldObject != null) 
        {
            PlantSeed heldSeed = heldObject.GetComponent<PlantSeed>();
            if (heldSeed != null)
            {
                heldObject.MouseDrop();
                if (heldSeed.plantSpot != null) { heldSeed.plantSpot.EmptySpot(); }
                Destroy(heldSeed.gameObject);
            }
        }
    }

    public void UpdateColliderDraw()
    {
        /*
        foreach (Collider2D collider in FindObjectsOfType<Collider2D>())
        {
            if (tool in collider.GetComponent<IClickable>().allowedTools)
            {
                DrawColliderBounds(collider);
            }
        }
        */
    }
    
    void DrawColliderBounds(Collider2D collider)
    {
        Color boundColor = Color.green;

    }
}
