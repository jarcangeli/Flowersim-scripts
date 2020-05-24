using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(MeshFilter))]
public class LeafMeshGenerator : MeshGenerator
{
    public PlantSeed seed;
    bool setup = false;

    Random rand;

    Color color;
    [SerializeField]
    float leafLength;
    [SerializeField]
    float leafWidth;
    [SerializeField]
    float leafRotation; // in the z
    [SerializeField]
    float leafQuadB;
    [SerializeField]
    float leafQuadC;
    [SerializeField]
    int nSpikes;
    [SerializeField]
    float outVertX;
    [SerializeField]
    float inVertX;
    [SerializeField]
    float inVertY;

    // Quad debug storage
    float[] x = new float[10];
    float[] y = new float[10];

    Vector3[] stemVertices;
    Vector3[] inSpikeVertices;
    Vector3[] outSpikeVertices;

    Vector3[] upVertices;
    Vector3[] downVertices;
    int[] upTriangles;
    int[] downTriangles;

    public int parentVertInd = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (seed != null && !setup)
        {
            SetUpLeaf(seed);
        }
    }

    public void SetUpLeaf(PlantSeed sourceSeed)
    {
        seed = sourceSeed;

        GetSeedValues();
        float dir = parentVertInd % 2 == 0 ? 1f : -1f;
        transform.localRotation *= Quaternion.Euler(0f, 0f, leafRotation *  dir);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();

        SetStage(0);

        setup = true;
    }

    public void UpdatePosition()
    {
        StemMeshGenerator stem = transform.parent.GetComponent<StemMeshGenerator>();
        if (stem != null)
        {
            transform.localPosition = stem.vertices[parentVertInd];
        }
    }

    void GetSeedValues()
    {
        rand = seed.rand;
        nSpikes = seed.nSpikes;
        leafLength = seed.leafLength.GetValue(rand);
        leafWidth = seed.leafWidth.GetValue(rand);
        leafRotation = seed.leafRotation.GetValue(rand);
        leafQuadB = seed.leafQuadB.GetValue(rand);
        leafQuadC = seed.leafQuadC.GetValue(rand);
        outVertX = seed.outVertX;
        inVertX = seed.inVertX;
        inVertY = seed.inVertY;
        color = seed.leafColor;

        // quick check on the xs
        if (false && inVertX < outVertX) 
        {
            float _ = inVertX;
            inVertX = outVertX;
            outVertX = _;
        }
    }

    public override void CreateShape()
    {
        stemVertices = new Vector3[nSpikes];
        inSpikeVertices = new Vector3[nSpikes - 1];
        outSpikeVertices = new Vector3[nSpikes];

        upVertices = new Vector3[3 * nSpikes - 1];
        upTriangles = new int[3 * (3 * (nSpikes - 1))];

        Vector3 stemV, outV, inV; // just for GC
        float outX, inX;

        // define quadratic weights at each vertex
        float Quad(float _x)
        {
            return leafQuadC * _x * (_x - leafQuadB * leafWidth);
        }
        float yNorm = 0f;
        float yMean = 0f;
        for (int i = 0; i < 10; ++i) 
        { 
            float _x = leafLength / 10 * i;
            x[i] = _x;
            y[i] = Quad(_x);
            yMean += y[i];
            if (Mathf.Abs(y[i]) > yNorm) { yNorm = Mathf.Abs(y[i]); }
        }
        yMean /= 10;

        float NQuad(float _x)
        {
            float _y = Quad(_x);
            return .5f + (_y - yMean) / yNorm / 2f;
        }




        for (int i = 0; i < nSpikes; ++i)
        {
            // assign vertices //
            stemV = new Vector3(leafLength * i / nSpikes, 0f);
            outX = leafLength * (outVertX + i) / nSpikes;
            outV = new Vector3(outX, leafWidth * NQuad(outX));

            if (i == nSpikes - 1) { outV = new Vector3(outV.x, 0f, outV.z); }

            stemVertices[i] = stemV;
            upVertices[3 * i] = stemV;
            outSpikeVertices[i] = outV;
            upVertices[3 * i + 1] = outV;

            if ( i < (nSpikes - 1))
            {
                // not last spike
                inX = leafLength * ( inVertX + i) / nSpikes;
                inV = new Vector3(inX, leafWidth * inVertY * NQuad(inX));
                inSpikeVertices[i] = inV;
                upVertices[3 * i + 2] = inV;
            }

            // Build Triangles //
            // split into triplets
            // first triplet tri
            if ( i == (nSpikes - 1))
            {
                // last tri of leaf
                upTriangles[upTriangles.Length - 3] = 3 * i - 1;
                upTriangles[upTriangles.Length - 2] = 3 * i + 1;
                upTriangles[upTriangles.Length - 1] = 3 * i;
            }
            else if ( i > 0 ) // no first trip tri on first spike
            {
                upTriangles[3 * 3 * i - 3] = 3 * i - 1;
                upTriangles[3 * 3 * i + 1 - 3] = 3 * i + 1;
                upTriangles[3 * 3 * i + 2 - 3] = 3 * i + 2;
            }

            if (i < (nSpikes - 1)) // last triplet only has first tri
            {
                // second triplet tri
                if (i == 0)
                {
                    // first tri of leaf
                    upTriangles[0] = 0;
                    upTriangles[1] = 1;
                    upTriangles[2] = 2;

                    // third triplet tri
                    upTriangles[3] = 0;
                    upTriangles[4] = 2;
                    upTriangles[5] = 3;
                }
                else
                {
                    upTriangles[3 * (3 * i + 1) - 3] = 3 * i - 1;
                    upTriangles[3 * (3 * i + 1) + 1 - 3] = 3 * i + 2;
                    upTriangles[3 * (3 * i + 1) + 2 - 3] = 3 * i;

                    // third triplet tri
                    upTriangles[3 * (3 * i + 2) - 3] = 3 * i;
                    upTriangles[3 * (3 * i + 2) + 1 - 3] = 3 * i + 2;
                    upTriangles[3 * (3 * i + 2) + 2 - 3] = 3 * i + 3;
                }
            }
        }

        // flipped in y
        downVertices = new Vector3[upVertices.Length]; 
        for (int k = 0; k < upVertices.Length; ++k)
        {
            downVertices[k] = new Vector3(upVertices[k].x, -upVertices[k].y, upVertices[k].z);
        }
        // reverse order + length of upVertices
        downTriangles = new int[upTriangles.Length];
        for (int k = 0; k < 3 * (nSpikes-1); ++k)
        {
            downTriangles[3 * k] = upVertices.Length + upTriangles[3 * k];
            // flip last two vertices
            downTriangles[3 * k + 1] = upVertices.Length + upTriangles[3 * k + 2];
            downTriangles[3 * k + 2] = upVertices.Length + upTriangles[3 * k + 1];
        }

        vertices = new Vector3[upVertices.Length * 2];
        upVertices.CopyTo(vertices, 0);
        downVertices.CopyTo(vertices, upVertices.Length);
        triangles = new int[upTriangles.Length * 2 + 3];
        upTriangles.CopyTo(triangles, 0);
        downTriangles.CopyTo(triangles, upTriangles.Length);
        // add cap tri
        triangles[triangles.Length - 3] = upVertices.Length - 2;
        triangles[triangles.Length - 2] = upVertices.Length - 1;
        triangles[triangles.Length - 1] = upVertices.Length * 2 - 1;

        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
        {
            colors[i] = color;
        }

        adultVertices = new Vector3[vertices.Length];
        vertices.CopyTo(adultVertices, 0);
    }

    
    public override void SetStage(float frac)
    {
        frac -= growthDelay;
        if (frac < 0) frac = 0;
        else if (frac > 1) frac = 1;

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = adultVertices[i] * frac * frac;
        }
        UpdateMesh();
    }
}
