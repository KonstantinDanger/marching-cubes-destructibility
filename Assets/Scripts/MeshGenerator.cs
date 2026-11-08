using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    private readonly GameObject _meshHolder;

    private Mesh _cached;

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

    private Mesh GenerateMesh(float[,] heightMap)
    {
        if (_cached != null)
            _cached.Clear();

        _cached = new();

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uv = new();

        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        int counter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float y = heightMap[x, z];
                vertices.Add(new Vector3(x, y, z));

                uv.Add(new((float)x / width - 1, (float)z / depth - 1));
            }
        }

        for (int x = 0; x < width - 1; x++)
        {
            for (int z = 0; z < depth - 1; z++)
            { 
                SetTriangle(triangles, x * depth + z, depth);
                counter++;
            }
        }

        //for (int i = 0; i < len; i++)
        //{
        //    SetVertice(vertices, i);
        //}

        //for (int i = 0; i < len; i++)
        //{
        //    SetTriangle(triangles, i);
        //}

        _cached.vertices = vertices.ToArray();
        _cached.triangles = triangles.ToArray();
        _cached.uv = uv.ToArray();

        _cached.RecalculateNormals();
        _cached.RecalculateBounds();
        _cached.Optimize();
        _cached.OptimizeReorderVertexBuffer();
        return _cached;
    }

    private static void SetTriangle(List<int> triangles, int i, int depth)
    {
        triangles.Add(i);
        triangles.Add(i + 1);
        triangles.Add(i + depth);

        triangles.Add(i + depth);
        triangles.Add(i + 1);
        triangles.Add(i + depth + 1);
    }

    private static void SetVertice(List<Vector3> vertices, int x, int z, float y)
    {
        vertices.Add(new(x, y, 0));
        vertices.Add(new(x, y, z + 1));

        vertices.Add(new(x + 1, y, 0));
        vertices.Add(new(x + 1, y, z + 1));
    }
}
