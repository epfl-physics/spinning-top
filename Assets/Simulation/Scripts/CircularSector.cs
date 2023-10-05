using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircularSector : MonoBehaviour
{
    [SerializeField] private float angle = 15;
    [SerializeField, Min(0)] private float radius = 1;
    [SerializeField] private Vector3 normal = Vector3.back;
    [SerializeField] private Vector3 e1 = Vector3.right;
    [SerializeField] private Color color = Color.black;
    private Vector3 e2;

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private List<Vector3> vertexBuffer;
    private List<int> triangleBuffer;

    private void OnValidate()
    {
        if (angle >= 360) angle -= 360;
        if (angle < 0) angle += 360;

        // First two basis vectors
        normal = normal.normalized;
        e1 = e1.normalized;
    }

    public void Redraw()
    {
        if (mesh == null)
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Sector Mesh";
        }

        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        // Compute the third basis vector
        e2 = Vector3.Cross(e1, normal);

        int angleFloor = Mathf.FloorToInt(angle);
        int numVertices = angleFloor + 3;
        int numTriangles = angleFloor + 1;

        vertexBuffer = new List<Vector3>(numVertices);
        triangleBuffer = new List<int>(3 * numTriangles);

        // Center of the circle
        vertexBuffer.Add(Vector3.zero);
        AddVertexAtAngle(0);

        // Vertices at [0, 1, ..., angleFloor] degrees
        for (int i = 1; i < numTriangles; i++)
        {
            AddVertexAtAngle(i);
            if (angleFloor > 0) AddTriangle(i + 1);
        }

        // Final vertex at the full angle
        AddVertexAtAngle(angle);
        AddTriangle(numVertices - 1);

        mesh.Clear();
        mesh.SetVertices(vertexBuffer);
        mesh.SetTriangles(triangleBuffer, 0);

        mesh.RecalculateNormals();
        mesh.Optimize();

        // Update color
        meshRenderer.sharedMaterial.color = color;
    }

    private void AddVertexAtAngle(float angle)
    {
        float theta = angle * Mathf.Deg2Rad;
        Vector3 vertex = radius * (Mathf.Cos(theta) * e1 + Mathf.Sin(theta) * e2);
        vertexBuffer.Add(vertex);
    }

    private void AddTriangle(int vertexIndex)
    {
        triangleBuffer.Add(0);
        triangleBuffer.Add(vertexIndex);
        triangleBuffer.Add(vertexIndex - 1);
    }

    public void Clear()
    {
        mesh.Clear();
        vertexBuffer.Clear();
        triangleBuffer.Clear();
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }

    public void SetAngleInRadians(float angle)
    {
        this.angle = Mathf.Rad2Deg * angle;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    public void SetNormal(Vector3 normal)
    {
        this.normal = normal;
    }

    public void SetE1(Vector3 e1)
    {
        this.e1 = e1;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }
}
