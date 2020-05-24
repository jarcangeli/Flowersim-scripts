using UnityEngine;
using Random = System.Random;

public enum FlowerType { normal, tulip, rose }

public static class SeedGenerator
{
    // these properties are global limits on plant properties
    // e.g. all plants will have nLeaves between IntRangeManager nLeaves.min and nLeaves.max
    // but each plant from a seed will have nLeaves restricted by the IntRange nLeaves
    // when adding new property, add to end of array, or break all quests

    // everything is explicit here but converted to array of floats for rest of code

    // Float Range Managers //
    public static FloatRangeManager leafWidth = new FloatRangeManager(0.1f, 1f, 0.1f, 0.2f);
    public static FloatRangeManager leafLength = new FloatRangeManager(0.2f, 2f, 0.1f, 0.2f);
    public static FloatRangeManager leafRotation = new FloatRangeManager(0, 20, 1, 5);
    public static FloatRangeManager leafQuadB = new FloatRangeManager(-.4f, .25f, 0.1f, 0.3f);
    public static FloatRangeManager leafQuadC = new FloatRangeManager(-1f, 0f, 0.1f, 0.3f);
    public static FloatRangeManager stemWidth = new FloatRangeManager(.2f, .4f, 0.1f, 0.15f);
    public static FloatRangeManager stemHeight = new FloatRangeManager(1f, 8.5f, 0.1f, 4f);
    public static FloatRangeManager stemWiggle = new FloatRangeManager(0.1f, 0.5f, 0.1f, 0.2f);

    public static FloatRangeManager[] floatRangeManagers = new FloatRangeManager[]
        { leafWidth, leafLength, leafRotation, leafQuadB, leafQuadC, stemWidth, stemHeight, stemWiggle };

    // Int Range Managers //
    public static IntRangeManager nLeaves = new IntRangeManager(2, 10, 1, 5);

    public static IntRangeManager[] intRangeManagers = new IntRangeManager[] { nLeaves };
    // Float Ranges //
    public static FloatRange outVertX = new FloatRange(0f, 1f);
    public static FloatRange inVertX = new FloatRange(0.1f, 1f);
    public static FloatRange inVertY = new FloatRange(0f, 1f);
    public static FloatRange flowerSize = new FloatRange(0.5f, 0.8f);
    public static FloatRange flowerCenterRadius = new FloatRange(0.05f, 0.2f);
    public static FloatRange petalWidth = new FloatRange(0.1f, 0.2f);
    public static FloatRange petalLength = new FloatRange(0.3f, 0.7f);
    public static FloatRange petalTip = new FloatRange(-.3f, .3f);
    public static FloatRange flowerFreq = new FloatRange(0.4f, 1f);
    public static FloatRange petalHue = new FloatRange(0f, 1f);
    public static FloatRange petalSat = new FloatRange(.7f, 1f);
    public static FloatRange petalVal = new FloatRange(.8f, 1f);
    public static FloatRange flowerCenterHue = new FloatRange(20f/360, 70f/360);
    public static FloatRange flowerCenterSat = new FloatRange(.8f, 1f);
    public static FloatRange flowerCenterVal = new FloatRange(.6f, .8f);
    public static FloatRange leafColorHue = new FloatRange(30f / 360, 160f / 360);
    public static FloatRange leafColorSat = new FloatRange(.5f, 1f);
    public static FloatRange leafColorVal = new FloatRange(.25f, .5f);
    public static FloatRange stemColorHue = new FloatRange(80f / 360, 160f / 360);
    public static FloatRange stemColorSat = new FloatRange(.5f, 1f);
    public static FloatRange stemColorVal = new FloatRange(.3f, .5f);

    public static FloatRange[] floatRanges = new FloatRange[]
    {
        outVertX, inVertX, inVertY, flowerSize, flowerCenterRadius, petalWidth, petalLength, petalTip, flowerFreq,
        petalHue, petalSat, petalVal, flowerCenterHue, flowerCenterSat, flowerCenterVal, 
        leafColorHue, leafColorSat, leafColorVal, stemColorHue, stemColorSat, stemColorVal
    };

    // Int Ranges //
    public static IntRange randomSeed = new IntRange(0, 1024); // each seed gets a random number assigned to generate plant
    public static IntRange nSpikes = new IntRange(2, 6);
    public static IntRange nPetals = new IntRange(5, 10);

    public static IntRange[] intRanges = new IntRange[] { randomSeed, nSpikes, nPetals };

    public static FlowerType[] flowerTypes = new FlowerType[] { FlowerType.normal, FlowerType.tulip, FlowerType.rose };

    // define a generator for a new seed
    public static PlantSeed GenerateSeed(GameObject prefab, float petalHue = -1f, FlowerType type = FlowerType.normal)
    {
        GameObject seedObj = GameObject.Instantiate(prefab);
        seedObj.name = "New Seed";
        seedObj.SetActive(false);
        PlantSeed seed = seedObj.GetComponent<PlantSeed>();

        // Generate new arrays
        FloatRange[] newFloatRanges = new FloatRange[floatRangeManagers.Length];
        for (int i = 0; i < floatRangeManagers.Length; ++i)
        {
            newFloatRanges[i] = floatRangeManagers[i].GetFloatRange();
        }
        IntRange[] newIntRanges = new IntRange[intRangeManagers.Length];
        for (int i = 0; i < intRangeManagers.Length; ++i)
        {
            newIntRanges[i] = intRangeManagers[i].GetIntRange();
        }
        float[] newFloats = new float[floatRanges.Length];
        for (int i = 0; i < floatRanges.Length; ++i)
        {
            newFloats[i] = floatRanges[i].GetValue();
        }
        int[] newInts = new int[intRanges.Length];
        for (int i = 0; i < intRanges.Length; ++i)
        {
            newInts[i] = intRanges[i].GetValue();
        }
        FlowerType[] newFlowerTypes = new FlowerType[] { type };

        if (petalHue > 0f && petalHue < 1f) { newFloats[9] = petalHue; }

        seed.PlantPropertiesFromArrays(newFloatRanges, newIntRanges, newFloats, newInts, newFlowerTypes);

        seedObj.SetActive(true);
        return seed;
    }
}
