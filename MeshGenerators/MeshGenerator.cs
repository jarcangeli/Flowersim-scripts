using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public Mesh mesh;

    float outlineThickness = 0.01f;
    public Material outlineMat;

    public Vector3[] vertices;
    public int[] triangles;
    public Color[] colors;
    public Vector2[] uvs;

    public float growthDelay = 0f;
    public Vector3[] adultVertices;

    public virtual void CreateShape()
    {
        vertices = new Vector3[]
        {
            new Vector3 (0, 0, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (1, 0, 0)
        };

        triangles = new int[]
        {
            0, 1, 2
        };

        colors = new Color[]
        {
            Color.red,
            Color.green,
            Color.blue
        };
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        GeneratePlaneUVs();

        mesh.RecalculateNormals();
    }

    public void GeneratePlaneUVs()
    {
        uvs = new Vector2[vertices.Length];
        Bounds bounds = mesh.bounds;
        float bound = Mathf.Max(bounds.size.x, bounds.size.y);
        int i = 0;
        while (i < uvs.Length)
        {
            Vector3 vert = vertices[i];
            uvs[i] = new Vector2(vertices[i].x / bound, vertices[i].y / bound);
            i++;
        }
        mesh.uv = uvs;
    }

    public virtual void SetStage(float frac)
    {
        frac -= growthDelay;
        if (frac < 0) frac = 0;
        else if (frac > 1) frac = 1;

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = adultVertices[i] * frac;
        }
        UpdateMesh();
    }

    public void CreateOutlineMesh()
    {
        GameObject outlineObj = Instantiate(new GameObject(), transform);
        outlineObj.name = transform.name + " outline";

        MeshFilter meshFilter = outlineObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = outlineObj.AddComponent<MeshRenderer>();
        outlineObj.transform.localScale = Vector3.one * (1 + outlineThickness);
        outlineObj.transform.localPosition += Vector3.back * 0.1f;
        meshRenderer.material = outlineMat;
        meshFilter.mesh = mesh;
    }
}
