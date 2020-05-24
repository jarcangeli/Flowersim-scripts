using System.Runtime.CompilerServices;
using UnityEngine;

public class PlantSpot : MonoBehaviour, IClickable
{
    public PlantSeed plant;
    TimeManager timeManager;
    public bool filled = false;
    PlayerInputs player;
    public bool germinate = true;
    public bool forCuttings = false;
    [SerializeField]
    SpriteRenderer fillSprite = null;

    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
        timeManager = FindObjectOfType<TimeManager>();
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold && player.heldObject != null)
        {
            PlantSeed heldPlant = player.heldObject.gameObject.GetComponent<PlantSeed>();
            if (heldPlant == plant)
            {
                heldPlant.GetComponent<FollowCursor>().MouseDrop();
            }
            else if (heldPlant != null && !filled &&  // something to put in an empty spot
                ( (heldPlant.isCutting && forCuttings ) || 
                  (!heldPlant.isCutting && !forCuttings &&
                        ((!germinate && heldPlant.growthStage == 0) ||
                         germinate)
                  )
                ))
            {
                heldPlant.GetComponent<FollowCursor>().MouseDrop(false);
                FillSpot(heldPlant);
            }
        }
        else if (tool == CursorTool.extract)
        {
            PollenExtractor extract = player.heldObject.GetComponent<PollenExtractor>();
            extract.DepositSeed(this);
        }
    }

    public void FillSpot(PlantSeed newPlant)
    {
        if (!filled && newPlant != null)
        {
            if (newPlant.plantSpot != null && newPlant.plantSpot != this)
            {
                newPlant.plantSpot.EmptySpot(); // leave current spot
            }

            filled = true;
            plant = newPlant;
            plant.transform.SetParent(transform, false);
            plant.plantSpot = this;
            if (germinate) 
            {
                FindObjectOfType<AudioManager>().Play("Planting");
                plant.growing = true;
                plant.GetComponent<FollowCursor>().rooted = true;
                fillSprite.enabled = true;
                timeManager.FastForward();
            }
            if (forCuttings)
            {
                // now that its parented allow drops
                plant.GetComponent<FollowCursor>().droppable = true;
            }
        }
    }
    public void EmptySpot()
    {
        if (filled)
        {
            filled = false;
            plant.transform.parent = null;
            plant.growing = false; // stop growing
            plant.plantSpot = null;
            plant = null;
            if (germinate)
            {
                fillSprite.enabled = false;
            }
        }
    }

    void Update()
    {
        if (plant != null)
        {
            plant.transform.localPosition = Vector3.back;
        }
    }
}
