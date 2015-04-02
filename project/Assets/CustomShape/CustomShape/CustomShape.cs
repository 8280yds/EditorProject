using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomShape : MonoBehaviour
{
    public ColorPoint[] colorPoints;
    public Triangle[] triangles;

    private Mesh mesh;

    void OnEnable()
    {
        updateMesh();
    }

    void Reset()
    {
        updateMesh();
    }

    public void updateMesh()
    {
        if (mesh == null)
        {
            mesh = GetComponent<MeshFilter>().mesh = new Mesh();
            mesh.hideFlags = HideFlags.HideAndDontSave;
        }

        if (colorPoints == null)
        {
            colorPoints = new ColorPoint[0];
        }

        if(triangles == null)
        {
            triangles = new Triangle[0];
        }

        mesh.Clear();
        setMeshData();
    }

    private void setMeshData()
    {
        int len = colorPoints.Length;
        Vector3[] vertices = new Vector3[len];
        Color[] colors = new Color[len];

        for (int i = 0; i < len; i++)
        {
            vertices[i] = colorPoints[i].point;
            colors[i] = colorPoints[i].color;
        }

        len = triangles.Length;
        int[] triangleInts = new int[3 * len];

        for (int i = 0; i < len; i++)
        {
            triangleInts[3 * i] = triangles[i].a;
            triangleInts[3 * i + 1] = triangles[i].b;
            triangleInts[3 * i + 2] = triangles[i].c;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangleInts;
    }

}
