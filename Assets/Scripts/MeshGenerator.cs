using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    private readonly GameObject _meshHolder;

    public MeshGenerator(GameObject meshHolder)
    {
        _meshHolder = meshHolder;
    }

    public void Generate(float[,] heightMap)
    {
        if (!_meshHolder.TryGetComponent(out MeshRenderer _))
        {
            _meshHolder.AddComponent<MeshRenderer>();
        }

        if (!_meshHolder.TryGetComponent(out MeshFilter filter))
        {
            filter = _meshHolder.AddComponent<MeshFilter>();
        }

        filter.mesh = GenerateMesh(heightMap);
    }

    private static Mesh GenerateMesh(float[,] heightMap)
    {
        Mesh mesh = new();

        const int len = 10;
        List<Vector3> vertices = new();
        List<int> triangles = new();

        for (int i = 0; i < len; i++)
        {
            vertices.Add(new(i, 0, 0));
            vertices.Add(new(i, 0, 1));

            vertices.Add(new(i + 1, 0, 0));
            vertices.Add(new(i + 1, 0, 1));
        }

        for (int i = 0; i < len; i++)
        {
            int offsetStep = i * 4;
            triangles.Add(offsetStep);
            triangles.Add(1 + offsetStep);
            triangles.Add(2 + offsetStep);

            triangles.Add(2 + offsetStep);
            triangles.Add(1 + offsetStep);
            triangles.Add(3 + offsetStep);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
