using UnityEngine;

public class Bin : MonoBehaviour, IClickable
{
    PlayerInputs player;

    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
    }

    public void BinObject(GameObject obj)
    {
        FindObjectOfType<AudioManager>().Play("Bin");
        Destroy(obj);
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold)
        {
            if (player.heldObject != null)
            {
                PlantSeed heldSeed = player.heldObject.GetComponent<PlantSeed>();
                if (heldSeed != null)
                {
                    if (heldSeed.plantSpot != null) { heldSeed.plantSpot.EmptySpot(); }
                    BinObject(heldSeed.gameObject);
                }
            }
        }
        else if (tool == CursorTool.extract)
        {
            player.heldObject.GetComponent<PollenExtractor>().EmptyExtractor();
        }
    }
}
