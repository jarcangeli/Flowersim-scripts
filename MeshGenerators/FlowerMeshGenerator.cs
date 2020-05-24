using UnityEngine;
using Random = System.Random;
public class FlowerMeshGenerator : MeshGenerator, IClickable
{
    public PlantSeed seed;
    [SerializeField]
    GameObject petalPrefab = null;
    public bool cut = false;

    Random rand;

    public FlowerType type;

    public Color centerColor;
    public Color petalColor;
    Color _petalColor;
    float majorColorRange = 0.25f;
    float minorColorRange = 0.1f;

    public int nPetals; // > 3
    int petalLayers;
    PetalMeshGenerator[] petals;
    public float centerRadius;
    public float petalWidth;
    public float petalLength;
    public float petalTip;

    public int parentVertInd = 0;

    CircleCollider2D ownCollider;

    PlayerInputs player;

    void Start()
    {
        player = FindObjectOfType<PlayerInputs>();
    }

    public void SetUpFlower(PlantSeed sourceSeed)
    {
        seed = sourceSeed;

        GetSeedValues();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();

        CreateCollider();

        CreateOutlineMesh();

        SetStage(0); // born
    }

    Color VaryPetalColor(Color color, float range, float val = -1f)
    {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        Color color1 = Color.HSVToRGB(H, Mathf.Clamp(S * (1 + range), 0, 1), Mathf.Clamp(V * (1 - range), 0, 1));
        Color color2 = Color.HSVToRGB(H, Mathf.Clamp(S * (1 - range), 0, 1), Mathf.Clamp(V * (1 + range), 0, 1));
        if (val > 0f && val < 1f)
        {
            return new ColorRange(color1, color2).GetValue(val);
        }
        return new ColorRange(color1, color2).GetValue();
    }

    void GetSeedValues()
    {
        rand = seed.rand;

        type = seed.flowerTypes[rand.Next(0, seed.flowerTypes.Length)];
        centerRadius = seed.flowerCenterRadius;
        nPetals = seed.nPetals;
        petalWidth = seed.petalWidth;
        petalLength = seed.petalLength;
        if (type == FlowerType.rose)
        {
            petalLayers = 3;
            petalWidth = petalLength;
        }
        else
        {
            petalLayers = 1;
            petalWidth = petalLength;
        }
        if (type == FlowerType.tulip)
        {
            petalLength *= 1.5f;
            petalWidth *= 0.8f;
        }
        petalTip = seed.petalTip;

        petalColor = seed.petalColor;
        centerColor = seed.flowerCenterColor;
        _petalColor = petalColor;
    }
    public void UpdatePosition()
    {
        StemMeshGenerator stem = transform.parent.GetComponent<StemMeshGenerator>();
        if (stem != null)
        {
            transform.localPosition = stem.vertices[parentVertInd] + Vector3.back * 0.5f;
        }

        foreach (PetalMeshGenerator petal in petals)
        {
            petal.UpdatePosition();
        }
    }


    public override void CreateShape()
    {
        if (type == FlowerType.normal)
        {
            vertices = new Vector3[nPetals];
            triangles = new int[(nPetals - 2) * 3];
            colors = new Color[vertices.Length];

            float angDelta = 2f * Mathf.PI / nPetals;
            petals = new PetalMeshGenerator[nPetals];
            // draw a circle with nPetals vertices
            for (int i = 0; i < nPetals; ++i)
            {
                float ang = angDelta * i;
                float x = Mathf.Cos(ang);
                float y = Mathf.Sin(ang);
                vertices[i] = new Vector3(x, y, 0f) * centerRadius;

                if (type == FlowerType.normal)
                {
                    colors[i] = centerColor;
                }
                else
                {
                    colors[i] = petalColor;
                }

                if (i > 1)
                {
                    triangles[3 * (i - 2)] = 0;
                    triangles[3 * (i - 2) + 1] = i - 1;
                    triangles[3 * (i - 2) + 2] = i;
                }
            }

            adultVertices = new Vector3[vertices.Length];
            vertices.CopyTo(adultVertices, 0);
        }

        CreatePetals();
    }

    void CreatePetals()
    {
        float angDelta = 2f * Mathf.PI / nPetals;
        float zAng = -90f + ((float)rand.NextDouble() - 0.5f) * 90f; // for tulips
        petals = new PetalMeshGenerator[nPetals * petalLayers];
        // draw a circle with nPetals vertices
        for (int i = 0; i < nPetals; ++i)
        {
            float ang = angDelta * i;
            // at each vertex create a petal
            if (type == FlowerType.rose)
            {
                petalColor = VaryPetalColor(petalColor, minorColorRange);
            }
            else if (type == FlowerType.tulip)
            {
                petalColor = VaryPetalColor(petalColor, majorColorRange, (float)i/nPetals);
            }

            PetalMeshGenerator petal = Instantiate(petalPrefab, transform).GetComponent<PetalMeshGenerator>();
            petal.name = "Petal " + i;
            petal.growthDelay = growthDelay;
            petal.SetUpPetal(this);

            if (type == FlowerType.normal)
            {
                petal.transform.localRotation = Quaternion.Euler(0f, 0f, ang * 360f / 2f / Mathf.PI);
                petals[i] = petal;
            }
            else if (type == FlowerType.rose)
            {
                petal.transform.localRotation = Quaternion.Euler(0f, 0f, ang * 360f / 2f / Mathf.PI);
                petal.UpdateColor(VaryPetalColor(petalColor, majorColorRange, 0f));
                petals[petalLayers * i] = petal;
                // create 3 layers of petals
                for (int j = 1; j < petalLayers; ++j)
                {
                    PetalMeshGenerator petali = Instantiate(petal, transform).GetComponent<PetalMeshGenerator>();
                    petali.SetUpPetal(this);
                    petali.name = "Petal " + i + "-"+j;
                    petali.transform.localScale = Vector3.one * (petalLayers - j) / petalLayers;
                    petali.transform.localRotation = Quaternion.Euler(0f, 0f, ang * 360f / 2f / Mathf.PI + 360f / nPetals * j / petalLayers);
                    petali.localPos += Vector3.back * 0.1f * j;
                    petali.UpdateColor(VaryPetalColor(petalColor, majorColorRange, (float)j / (petalLayers-1)));
                    petals[petalLayers * i + j] = petali;
                }
            }
            else if (type == FlowerType.tulip)
            {
                float deltaZ = -20f + 40f * i / nPetals;
                petal.transform.localRotation = Quaternion.Euler(deltaZ, 0f, zAng + deltaZ);
                petals[i] = petal;
            }
        }
    }

    void CreateCollider()
    {
        ownCollider = gameObject.AddComponent<CircleCollider2D>();
        ownCollider.isTrigger = true;
        ownCollider.radius = petalLength;
    }

    public override void SetStage(float frac)
    {
        if (!cut)
        {
        frac = frac * frac * frac * frac; // delay flower growth
        foreach (PetalMeshGenerator petal in petals)
        {
            petal.SetStage(frac);
        }

        // use the old MeshGen size set
        base.SetStage(frac);
        ownCollider.radius = petalLength * frac;
        }
    }

    public PlantSeed TakePollen()
    {
        return seed;
    }

    public void OnClick(CursorTool tool)
    {
        if (tool == CursorTool.extract)
        {
            PollenExtractor extract = player.heldObject.GetComponent<PollenExtractor>();
            extract.GetPollen(seed);
        }
        else if (tool == CursorTool.secateur && !seed.isCutting)
        {
            Debug.Log("Secateuring flower: " + transform.name);
            Secateur secateur = player.heldObject.GetComponent<Secateur>();
            secateur.CutFlower(this);
        }
        else if (tool == CursorTool.hold && seed.isCutting)
        {
            seed.GetComponent<FollowCursor>().MousePickup();
        }
    }
}
