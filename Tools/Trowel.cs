using UnityEngine;

public class Trowel : MonoBehaviour, IClickable
{
    [SerializeField]
    FollowCursor followCursor = null;
    PlayerInputs player;

    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
    }

    public void TrowelPlant(PlantSeed seed)
    {
        Debug.Log("Troweling " + seed.name);
        seed.GetComponent<FollowCursor>().rooted = false;
        if (seed.plantSpot != null)
        {
            FindObjectOfType<AudioManager>().Play("Planting");
            seed.plantSpot.EmptySpot();
        }

        followCursor.MouseDrop(); // drop trowel
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold)
        {
            FindObjectOfType<AudioManager>().Play("ToolSelect2");
            followCursor.MousePickup();
            player.tool = CursorTool.trowel;
        }
    }

}
