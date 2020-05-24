using UnityEngine;

public class PollenExtractor : MonoBehaviour, IClickable
{
    public PlantSeed seedA;
    public PlantSeed seedB;
    [SerializeField]
    FollowCursor followCursor = null;
    PlayerInputs player;
    [SerializeField]
    SpriteRenderer seedASprite = null;
    [SerializeField]
    SpriteRenderer seedBSprite = null;
    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
    }

    public void DepositSeed(PlantSpot spot)
    {
        if (seedA != null && seedB != null)
        {
            PlantSeed seedC = seedA.Breed(seedB);
            spot.FillSpot(seedC);

            followCursor.MouseDrop(); // drop extractor
            EmptyExtractor();
        }
        else
        {
            Debug.Log("Need more pollen");
        }
    }

    public void EmptyExtractor() 
    {
        seedA = null;
        seedB = null;
        seedASprite.color = Color.white;
        seedBSprite.color = Color.white;
    }

    public void GetPollen(PlantSeed seedSample)
    {
        Debug.Log("Extracting pollen from " + seedSample.name);
        if (seedA == null) 
        { 
            seedA = seedSample;
            seedASprite.color = seedSample.flowerCenterColor;
            FindObjectOfType<AudioManager>().Play("Extractor");
        }
        else if (seedB == null) 
        { 
            seedB = seedSample;
            seedBSprite.color = seedSample.flowerCenterColor;
            FindObjectOfType<AudioManager>().Play("Extractor");
        }
        else { Debug.Log("Pollen Extractor full"); }
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold)
        {
            FindObjectOfType<AudioManager>().Play("ToolSelect1");
            followCursor.MousePickup();
            player.tool = CursorTool.extract;
        }
    }
}
