using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(FollowCursor))]
public class PlantSeed : MonoBehaviour, IClickable
{
    AudioManager audioManager;
    public GameObject leafPrefab = null;
    public GameObject stemPrefab = null;
    public GameObject flowerPrefab= null;
    public GameObject plantPrefab;

    [SerializeField] 
    Sprite normalSprite = null;
    [SerializeField]
    Sprite tulipSprite = null;
    [SerializeField]
    Sprite roseSprite = null;
    public Sprite[] seedSprites;
    [SerializeField]
    SpriteRenderer seedSprite = null;

    [SerializeField]
    FollowCursor followCursor = null;
    PlayerInputs playerInputs;

    public PlantSpot plantSpot = null;
    public Random rand;

    // stem
    StemMeshGenerator stemMesh;
    // leaves
    List<LeafMeshGenerator> leaves = new List<LeafMeshGenerator>();
    [HideInInspector]
    public int totLeaves;
    // flowers
    List<FlowerMeshGenerator> flowers = new List<FlowerMeshGenerator>();
    public bool isCutting = false;

    // Plant Properties //
    public int randomSeed;
    public int nSpikes;
    public int nPetals;

    public IntRange nLeaves;

    public FloatRange leafWidth;
    public FloatRange leafLength;
    public FloatRange leafRotation;
    public FloatRange leafQuadB;
    public FloatRange leafQuadC;
    public FloatRange stemWidth;
    public FloatRange stemHeight;
    public FloatRange stemWiggle;

    public float outVertX;
    public float inVertX;
    public float inVertY;
    public float flowerSize;
    public float flowerCenterRadius;
    public float flowerFreq;
    public float petalWidth;
    public float petalLength;
    public float petalTip;

    public Color leafColor;
    public Color stemColor;
    public Color petalColor;
    public Color flowerCenterColor;

    public FlowerType[] flowerTypes;

    // end of plant properties //
    // arrays
    public FloatRange[] floatRanges;
    public IntRange[] intRanges;
    public float[] floats;
    public int[] ints;
    public Color[] colors;

    public void PlantPropertiesFromArrays(
        FloatRange[] _floatRanges, IntRange[] _intRanges,
        float[] _floats, int[] _ints, FlowerType[] _flowerTypes)
    {
        floatRanges = _floatRanges;
        intRanges = _intRanges;
        floats = _floats;
        ints = _ints;

        randomSeed = ints[0];
        nSpikes = ints[1];
        nPetals = ints[2];

        nLeaves = intRanges[0];

        leafWidth = floatRanges[0];
        leafLength = floatRanges[1];
        leafRotation = floatRanges[2];
        leafQuadB = floatRanges[3];
        leafQuadC = floatRanges[4];
        stemWidth = floatRanges[5];
        stemHeight = floatRanges[6];
        stemWiggle = floatRanges[7];

        outVertX = floats[0];
        inVertX = floats[1];
        inVertY = floats[2];
        flowerSize = floats[3];
        flowerCenterRadius = floats[4];
        petalWidth = floats[5];
        petalLength = floats[6];
        petalTip = floats[7];
        flowerFreq = floats[8];
        petalColor = Color.HSVToRGB(floats[9], floats[10], floats[11]);
        flowerCenterColor = Color.HSVToRGB(floats[12], floats[13], floats[14]);
        leafColor = Color.HSVToRGB(floats[15], floats[16], floats[17]);
        stemColor = Color.HSVToRGB(floats[18], floats[19], floats[20]);

        flowerTypes = _flowerTypes;
        seedSprite.sprite = seedSprites[(int)_flowerTypes[0]];
        Debug.Log(flowerTypes[0]);
        Debug.Log((int)flowerTypes[0]);
        Debug.Log(seedSprite.sprite);
    }

    // growth
    public float growthTime = 4f; // seconds in real time when FF
    public int nGrowthStages = 10;
    public float growthRate = 1f;
    public int growthStage = 0;
    public bool growing = false;
    bool seeded = false;
    float startTime = 0f;
    TimeManager timeManager;

    private void Awake()
    {
        plantPrefab = FindObjectOfType<SeedDispenser>().plantPrefab;
        rand = new Random(randomSeed);
        playerInputs = FindObjectOfType<PlayerInputs>();
        timeManager = FindObjectOfType<TimeManager>();
        audioManager = FindObjectOfType<AudioManager>();

        seedSprites = new Sprite[] { normalSprite, tulipSprite, roseSprite };
    }

    public void StartSeed()
    {
        StartGrow();
        seeded = true;
        followCursor.rooted = true;
        followCursor.droppable = false;
    }

    public void StartGrow()
    {
        totLeaves = nLeaves.GetValue(rand);

        // stem
        GameObject stem = Instantiate(stemPrefab, this.transform);
        stem.name = "Stem";
        stem.transform.localPosition = Vector3.zero;
        stemMesh = stem.GetComponent<StemMeshGenerator>();
        stemMesh.SetUpStem(this);

        // add leaves and flowers
        for (int i = 0; i < totLeaves; ++i)
        {
            GameObject leaf = Instantiate(leafPrefab, stem.transform);
            leaf.name = "Leaf " + i;
            LeafMeshGenerator leafMesh = leaf.GetComponent<LeafMeshGenerator>();
            leafMesh.parentVertInd = 2 * (i + 1) + i % 2;
            leafMesh.transform.rotation *= Quaternion.Euler(0f, 0f, 180 * (i % 2 - 1));
            leafMesh.SetUpLeaf(this);
            leafMesh.growthDelay = (float)i / nGrowthStages;
            leaves.Add(leafMesh);

            if (rand.NextDouble() < flowerFreq || (i == totLeaves - 1 && flowers.Count == 0))
            {
                GameObject flower = Instantiate(flowerPrefab, stem.transform);
                flower.transform.localScale = Vector3.one * flowerSize;
                flower.name = "Flower " + i;
                FlowerMeshGenerator flowerMesh = flower.GetComponent<FlowerMeshGenerator>();
                flowerMesh.parentVertInd = 2 * (i + 1) + i % 2;
                flowerMesh.SetUpFlower(this);
                flowerMesh.growthDelay = (float)i / nGrowthStages;
                flowers.Add(flowerMesh);
            }
        }
    }
    void PlayTheme()
    {
        // get the instrument
        bool playMando = false;
        bool playClari = false;
        bool playWobble = false;
        foreach (FlowerMeshGenerator flower in flowers)
        {
            if (flower.type == FlowerType.normal)
            {
                playMando = true;
            }
            if (flower.type == FlowerType.tulip)
            {
                playClari = true;
            }
            if (flower.type == FlowerType.rose)
            {
                playWobble = true;
            }
        }
        // and get the level
        int lev = 2; // default
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager.currQuest != null)
        {
            float match = questManager.currQuest.SubmitMatch(this);
            lev = 7 - Mathf.CeilToInt(match * 7f);
            if (lev == 0) lev = 1;

        }
        // audio manager will batch and stagger these sounds
        if (playMando) audioManager.PlayTheme("Mando" + lev, 1, true);
        if (playClari) audioManager.PlayTheme("Clari" + lev, 1, true);
        if (playWobble) audioManager.PlayTheme("Wobble" + lev, 1, true);

    }

    private void Update()
    {
        if (growing && !followCursor.followCursor)
        {
            if (startTime == 0f && timeManager.state == TimeState.ff) 
            {
                startTime = timeManager.elapsedTime; 
            }
            if (!seeded)
            {
                StartSeed();
                SetGrowthStage(0);
            }
            // if we hit a new growth stage
            if (startTime != 0f && growthStage < (nGrowthStages + 1) && timeManager.elapsedTime > (startTime + growthStage * growthTime / nGrowthStages ))
            {
                if (growthStage == 0) 
                { 
                    PlayTheme();
                    playerInputs.enabled = false;
                }

                float frac = (float)growthStage / nGrowthStages;
                SetGrowthStage(frac);
                ++growthStage;

                if (growthStage == nGrowthStages + 1) 
                { 
                    timeManager.PauseTime();
                    playerInputs.enabled = true;
                }
            }
        }
    }

    void SetGrowthStage(float frac)
    {
        stemMesh.SetStage(frac);
        foreach (LeafMeshGenerator leaf in leaves)
        {
            leaf.SetStage(frac);
            leaf.UpdatePosition();
        }
        foreach (FlowerMeshGenerator flower in flowers)
        {
            flower.SetStage(frac);
            flower.UpdatePosition();
        }
    }
    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.hold && !followCursor.rooted)
        {
            followCursor.MousePickup();
        }
        else if (tool == CursorTool.trowel && !isCutting)
        {
            // then can safely get the trowel
            Trowel trowel = playerInputs.heldObject.GetComponent<Trowel>();
            trowel.TrowelPlant(this);
            followCursor.MousePickup();
        }
    }

    public PlantSeed Breed(PlantSeed seedB)
    {
        PlantSeed seedA = this;
        GameObject seedObj = Instantiate(plantPrefab);
        PlantSeed seedC = seedObj.GetComponent<PlantSeed>();
        // Generate new arrays
        FloatRange[] newFloatRanges = new FloatRange[floatRanges.Length];
        for (int i = 0; i < floatRanges.Length; ++i)
        {
            newFloatRanges[i] = FloatRange.Combine(seedA.floatRanges[i], seedB.floatRanges[i]);
        }
        IntRange[] newIntRanges = new IntRange[intRanges.Length];
        for (int i = 0; i < intRanges.Length; ++i)
        {
            newIntRanges[i] = IntRange.Combine(seedA.intRanges[i], seedB.intRanges[i]);
        }
        float[] newFloats = new float[floats.Length];
        for (int i = 0; i < 9; ++i) // no color floats
        {
            newFloats[i] = new FloatRange(seedA.floats[i], seedB.floats[i]).GetValue();
        }
        for (int j = 0; j < (floats.Length - 9 )/ 3; ++j) // no color floats
        {
            Color colorA = Color.HSVToRGB(seedA.floats[9 + 3 * j], seedA.floats[9 + 3 * j + 1], seedA.floats[9 + 3 * j + 2]);
            Color colorB = Color.HSVToRGB(seedB.floats[9 + 3 * j], seedB.floats[9 + 3 * j + 1], seedB.floats[9 + 3 * j + 2]);
            Color.RGBToHSV(new ColorRange(colorA, colorB).GetValue(), out float h, out float s, out float v);
            newFloats[9 + 3 * j] = h;
            newFloats[9 + 3 * j + 1] = s;
            newFloats[9 + 3 * j + 2] = v;
        }
        int[] newInts = new int[ints.Length];
        for (int i = 0; i < ints.Length; ++i)
        {
            newInts[i] = new IntRange(seedA.ints[i], seedB.ints[i]).GetValue();
        }

        FlowerType[] newFlowerTypes = seedA.flowerTypes.Union(seedB.flowerTypes).ToArray();

        seedC.PlantPropertiesFromArrays(newFloatRanges, newIntRanges, newFloats, newInts, newFlowerTypes);
        // force new random roll so no exact clones
        seedC.randomSeed = SeedGenerator.randomSeed.GetValue();
        seedC.enabled = true;
        return seedC;
    }

    public PlantSeed Copy(bool enabled = false)
    {
        GameObject seedObj = Instantiate(plantPrefab) as GameObject;
        PlantSeed copySeed = seedObj.GetComponent<PlantSeed>();
        copySeed.enabled = enabled;
        // copy properties
        copySeed.PlantPropertiesFromArrays(floatRanges, intRanges, floats, ints, flowerTypes);
        copySeed.rand = rand;
        return copySeed;
    }
}
