using UnityEngine;

public class SectorRange : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float angle;
    [SerializeField] int segments;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] PolygonCollider2D polygonCollider;

    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Sector Mesh";

        meshFilter.mesh = mesh;
        meshRenderer.sortingOrder = 1;
    }

    public void Init(int radius, float angle)
    {
        this.radius = GameplayUtils.ToWorldDistance(radius);
        this.angle = angle;

        BuildArea();
    }

    void BuildArea()
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float startAngle = -angle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + angle * i / segments;

            float rad = currentAngle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius / transform.lossyScale.x;
        }

        int index = 0;

        for (int i = 0; i < segments; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = i + 1;
            triangles[index++] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Vector2[] points = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            points[i] = vertices[i];
        }

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, points);
    }
}
