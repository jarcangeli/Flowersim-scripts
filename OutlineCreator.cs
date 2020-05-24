using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OutlineCreator : MonoBehaviour
{
    public float lineWidth;
    public Material material;
    int nLines = 0;
    public float zOff = 0f;
    public int endCapVerts = 1;
    public List<LineRenderer> lines;
    Dictionary<int, int> lookup;

    LineRenderer AddLine(Transform parent)
    {
        // Create first line
        LineRenderer line = new GameObject().AddComponent<LineRenderer>();
        line.transform.name = "Line " + nLines;
        line.positionCount = 0;
        line.material = material;
        line.numCapVertices = endCapVerts;
        line.startWidth = line.endWidth = lineWidth;
        line.transform.SetParent(parent, false);
        line.useWorldSpace = false;
        lines.Add(line);
        ++nLines;
        return line;
    }

    public void CreateOutline()
    {
        Transform lineContainer = new GameObject().transform;
        lineContainer.SetParent(transform, false);
        lineContainer.transform.name = "Line container";

        // Get triangles and vertices from mesh
        int[] triangles = GetComponent<MeshFilter>().mesh.triangles;
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        // move lines center to center of mesh
        Vector3 averageVert = Vector3.zero;
        foreach ( Vector3 vert in vertices) { averageVert += vert; }
        averageVert /= vertices.Length;
        lineContainer.transform.localPosition += averageVert;

        // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
        Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int e = 0; e < 3; e++)
            {
                int vert1 = triangles[i + e];
                int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                if (edges.ContainsKey(edge))
                {
                    edges.Remove(edge);
                }
                else
                {
                    edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                }
            }
        }

        // Create edge lookup Dictionary
        lookup = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> edge in edges.Values)
        {
            if (lookup.ContainsKey(edge.Key) == false)
            {
                lookup.Add(edge.Key, edge.Value);
            }
        }

        // This vector3 gets added to each line position, so it sits in front of the mesh
        // Change the -0.1f to a positive number and it will sit behind the mesh
        Vector3 bringFoward = new Vector3(0f, 0f, zOff);

        // Loop through edge vertices in order
        int startVert = 0;
        int nextVert = startVert;
        bool lastLine = false;
        while (true)
        {
            // Add to line
            //line.positionCount++;
            //line.SetPosition(line.positionCount - 1, vertices[nextVert] + bringFoward);
            // Add a line
            LineRenderer line = AddLine(lineContainer);
            line.positionCount = 2;
            line.SetPosition(0, vertices[nextVert] + bringFoward);

            // Get next vertex
            nextVert = lookup[nextVert];
            line.SetPosition(1, vertices[nextVert] + bringFoward);

            if (lastLine) return;
            
            if (nextVert == startVert)
            {
                lastLine = true;
            }
        }
    }

    public void UpdateLines(float frac)
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        
        int startVert = 0;
        int nextVert = lookup[startVert];
        for (int i = 0; i < lines.Count; ++i)
        {
            Vector3 v1 = vertices[startVert];
            Vector3 v2 = vertices[nextVert];

            LineRenderer line = lines[i];
            line.startWidth = lineWidth * frac;
            line.endWidth = lineWidth * frac;

            line.SetPosition(0, v1);
            line.SetPosition(1, v2);

            startVert = nextVert;
            nextVert = lookup[nextVert];
        }
    }

    }