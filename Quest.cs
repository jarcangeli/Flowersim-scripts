using UnityEngine;
using SG = SeedGenerator;

public class Quest : MonoBehaviour
{
    public PlantSeed questSeed;
    [SerializeField]
    GameObject plantPrefab = null;
    int reward = 5;
    int[] rewardTypes = new int[] { 4, 30, 100 };

    public float SubmitMatch(PlantSeed seed)
    {
        Color.RGBToHSV(questSeed.petalColor, out float targHue, out float targSat, out float targVal);
        Color.RGBToHSV(seed.petalColor, out float hue, out float sat, out float val);
        float match = Mathf.Pow(Mathf.Clamp(0.33f - Mathf.Abs(hue - targHue), 0, 1) / 0.33f, 2)  * 0.8f;
        match += (1f - Mathf.Abs(sat - targSat))*0.1f + (1f - Mathf.Abs(val - targVal))*0.1f;
        Debug.Log($"Match is {match} for simple HSV distance");
        // penalize wrong types
        FlowerType questType = questSeed.flowerTypes[0];
        bool matches = false;
        if (!seed.isCutting)
        {
            foreach (FlowerType type in seed.flowerTypes)
            {
                if (type == questType)
                {
                    matches = true;
                }
            }
        }
        else
        {
            FlowerMeshGenerator flower = seed.GetComponentInChildren<FlowerMeshGenerator>();
            if (flower != null && flower.type == questType) matches = true;
        }

        if (!matches) { match *= 0.5f; }
        return match;
    }
    public int GetReward(float match)
    {
        return (int)(reward * match);
    }

    public void GenerateQuestSeed(float petalColor = -1f, FlowerType type = FlowerType.normal)
    {
        if (questSeed != null) { Destroy(questSeed.gameObject); }

        questSeed = SeedGenerator.GenerateSeed(plantPrefab, petalColor, type);

        questSeed.rand = new System.Random(questSeed.randomSeed);
        questSeed.isCutting = true;
        questSeed.nLeaves = new IntRange(1, 1); // fix to one leaf
        questSeed.flowerFreq = 1f; // and one flower
        questSeed.StartSeed();

        FlowerMeshGenerator flower = questSeed.GetComponentInChildren<FlowerMeshGenerator>();
        //LeafMeshGenerator leaf = questSeed.GetComponentInChildren<LeafMeshGenerator>();
        flower.SetStage(1f);
        flower.transform.localScale = Vector3.one * 1.2f;
        //leaf.SetStage(1f);
        reward = rewardTypes[(int)flower.type];

        // Disable some normal seed components
        questSeed.GetComponentInChildren<SpriteRenderer>().enabled = false;
        questSeed.GetComponentInChildren<FollowCursor>().enabled = false;
        foreach (Collider2D collider in questSeed.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

    }
}
