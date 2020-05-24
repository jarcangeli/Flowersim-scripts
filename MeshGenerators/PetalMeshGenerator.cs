using UnityEngine;

public class PetalMeshGenerator : MeshGenerator
{
    int nVerts = 9;
    FlowerMeshGenerator flower;
    public Vector3 localPos = Vector3.forward * 0.25f;

    public void SetUpPetal(FlowerMeshGenerator sourceFlower)
    {
        //GetSeedValues(); // got in flower
        flower = sourceFlower;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();

        UpdatePosition();

        SetStage(0); // born
    }


    public override void CreateShape()
    {
        vertices = new Vector3[nVerts];
        triangles = new int[(nVerts - 2) * 3];
        colors = new Color[vertices.Length];

        float angDelta = 2f * Mathf.PI / nVerts;
        Vector3 vertex0 = new Vector3(flower.petalLength / 2f, 0f, 0f);
        // draw an ellipse with nVerts vertices
        for (int i = 0; i < nVerts; ++i)
        {
            float ang = angDelta * i;
            float x = Mathf.Cos(ang) * flower.petalLength / 2f;
            float y = Mathf.Sin(ang) * flower.petalWidth / 2f;
            if (i == 5) { y *= (1 + flower.petalTip); }

            vertices[i] = new Vector3(x, y, 0f) - vertex0;
            colors[i] = flower.petalColor;

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

    public void UpdateColor(Color newColor)
    {
        colors = new Color[colors.Length];
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = newColor;
        }
        UpdateMesh();
    }

    public void UpdatePosition()
    {
        if (flower != null)
        {
            transform.localPosition = localPos;
        }
    }
}
