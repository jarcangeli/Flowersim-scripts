using UnityEngine;

public class Secateur : MonoBehaviour, IClickable
{
    [SerializeField]
    FollowCursor followCursor = null;
    PlayerInputs player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold)
        {
            FindObjectOfType<AudioManager>().Play("ToolSelect3");
            followCursor.MousePickup();
            player.tool = CursorTool.secateur;
        }
    }

    public void CutFlower(FlowerMeshGenerator flower)
    {
        PlantSeed copySeed = flower.seed.Copy(true);
        copySeed.flowerTypes = new FlowerType[] { flower.type };
        copySeed.isCutting = true;
        copySeed.GetComponent<FollowCursor>().droppable = false;
        copySeed.StartGrow();
        if (copySeed.GetComponentInChildren<SpriteRenderer>() is SpriteRenderer seedSprite)
        {
            seedSprite.enabled = false;
        }

        FlowerMeshGenerator newFlowerMesh = Instantiate(copySeed.flowerPrefab, copySeed.transform).GetComponent<FlowerMeshGenerator>();
        newFlowerMesh.transform.localPosition += Vector3.back* 0.5f;
        LeafMeshGenerator newLeafMesh = Instantiate(copySeed.leafPrefab, copySeed.transform).GetComponent<LeafMeshGenerator>();
        newLeafMesh.transform.localPosition += Vector3.forward * 0.2f;

        newFlowerMesh.SetUpFlower(copySeed);
        newLeafMesh.SetUpLeaf(copySeed);

        newFlowerMesh.SetStage(1f);
        newLeafMesh.SetStage(1f);

        followCursor.MouseDrop();
        copySeed.GetComponent<FollowCursor>().MousePickup();
        player.tool = CursorTool.hold;

        // cut the flower
        flower.SetStage(0f);
        flower.cut = true;
        FindObjectOfType<AudioManager>().Play("Shears");
    }
}
