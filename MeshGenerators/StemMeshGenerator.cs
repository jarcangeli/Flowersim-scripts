using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(MeshFilter))]
public class StemMeshGenerator : MeshGenerator
{
    public PlantSeed seed;

    Random rand;

    public Color color;
    public float width;
    public float height;
    public FloatRange wiggle;
    public int nHeights;

    public void SetUpStem(PlantSeed sourceSeed)
    {
        seed = sourceSeed;

        GetSeedValues();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();

        if (GetComponent<OutlineCreator>() is OutlineCreator outline)
        {
            outline.CreateOutline();
        }

        SetStage(0); // born
    }

    void GetSeedValues()
    {
        rand = seed.rand;
        width = seed.stemWidth.GetValue(rand);
        height = seed.stemHeight.GetValue(rand);
        wiggle = new FloatRange(-seed.stemWiggle.GetValue(rand)/2f, seed.stemWiggle.GetValue(rand)/2f);
        color = seed.stemColor;

        nHeights = seed.totLeaves; // 2 vertices at each height, plus a ground pair
    }

    public override void CreateShape()
    {
        vertices = new Vector3[(nHeights + 1) * 2];
        triangles = new int[2 * 3 * (nHeights - 1)  + 3];

        // linear width decay from bottom to top, 1-0x
        // x pos has a random wiggle attached
        vertices[0] = new Vector3(-width / 2, 0f, 0f);
        vertices[1] = new Vector3(width / 2, 0f, 0f);
        float wig = 0f;
        for (int i = 0; i < nHeights; ++i)
        {
            float hH = height * (i + 1) / nHeights;
            float hW;
            if (nHeights != 1) { hW = width / 2 * (nHeights - 1 - i) / (nHeights - 1) + 0.05f; }
            else { hW = width / 2; }
            wig += wiggle.GetValue(rand);
            vertices[2*i+2] = new Vector3(-hW + wig, hH, 0f);
            vertices[2*i+3] = new Vector3(hW + wig, hH, 0f);

            // define the two tris
            triangles[3 * 2 * i] = 2 * i + 2;
            triangles[3 * 2 * i + 1] = 2 * i + 1;
            triangles[3 * 2 * i + 2] = 2 * i;
            if (i < nHeights - 1)
            {
                triangles[3 * 2 * i + 3] = 2 * i + 1;
                triangles[3 * 2 * i + 4] = 2 * i + 2;
                triangles[3 * 2 * i + 5] = 2 * i + 3;
            }
        }

        colors = new Color[vertices.Length];
        for (int j = 0; j < colors.Length; ++j)
        {
            colors[j] = color;
        }

        adultVertices = new Vector3[vertices.Length];
        vertices.CopyTo(adultVertices, 0);
    }


    public override void SetStage(float frac)
    {
        frac -= growthDelay;
        if (frac < 0) frac = 0;
        else if (frac > 1) frac = 1;

        Vector3 frac3 = new Vector3(frac * frac, frac, 1f);
        float wig = 0f;

        for (int i = 0; i < vertices.Length; ++i)
        {
            if ( i > 0 && i % 2 == 0 ) { wig = wiggle.GetValue(rand) / 4f; }
            vertices[i] = Vector3.Scale(adultVertices[i], frac3) +  wig * Vector3.left;
        }
        UpdateMesh();

        if (GetComponent<OutlineCreator>() is OutlineCreator outline) { outline.UpdateLines(frac); }
    }
}
