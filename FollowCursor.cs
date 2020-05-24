using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public bool followCursor = false;
    public bool rooted = false;
    Camera mainCamera;
    PlayerInputs player;
    public Vector2 offset = Vector2.zero;
    public bool droppable = true;
    public Vector2 defaultPos = Vector2.zero; // where you get dropped to

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
        player = FindObjectOfType<PlayerInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followCursor)
        {
            transform.position = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) + offset;
        }
    }

    public void MousePickup()
    {
        if (player.heldObject == null)
        {
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
            player.heldObject = this;
            followCursor = true;
        }

    }
     
    public void MouseDrop(bool resetPos = true)
    {
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = true;
        }
        player.heldObject = null;
        followCursor = false;
        player.tool = CursorTool.hold;
        if (resetPos) transform.position = defaultPos;
    }
}
