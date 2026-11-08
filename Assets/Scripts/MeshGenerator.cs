using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    private readonly GameObject _meshHolder;
    private readonly MapSize _mapSize;
    private readonly Material _baseMaterial;

    public MeshGenerator(GameObject meshHolder, MapSize mapSize, Material baseMaterial)
    {
        _meshHolder = meshHolder;
        _mapSize = mapSize;
        _baseMaterial = baseMaterial;
    }

    public void Generate(float[,] heightMap)
    {
        for (int x = 0; x < _mapSize.Width; x += _mapSize.ChunkWidth)
        {
            for (int z = 0; z < _mapSize.Depth; z += _mapSize.ChunkDepth)
            {
                CreateChunk(heightMap, x, z);
            }
        }
    }

    private void CreateChunk(float[,] heightMap, int startingX, int startingZ)
    {
        Transform chunksContainer = _meshHolder.transform.Find("Chunks");

        if (!chunksContainer)
        {
            chunksContainer = new GameObject("Chunks").transform;
            chunksContainer.parent = _meshHolder.transform;
        }

        Mesh mesh = new();
        GameObject chunkHolder = new("Chunk holder", new Type[]
        {
            typeof(MeshRenderer),
            typeof(MeshFilter),
            typeof(MeshCollider)
        });

        chunkHolder.transform.parent = chunksContainer;

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uv = new();

        int finalPosX = startingX + _mapSize.ChunkWidth + 1;
        int finalPosZ = startingZ + _mapSize.ChunkDepth + 1;

        for (int x = startingX; x < finalPosX; x++)
        {
            for (int z = startingZ; z < finalPosZ; z++)
            {
                float y = heightMap[x, z];

                vertices.Add(new Vector3(x, y, z));

                uv.Add(new Vector2(
                    (float)(x - startingX) / (_mapSize.ChunkWidth), 
                    (float)(z - startingZ) / (_mapSize.ChunkWidth)));
            }
        }

        int chunkDepth = _mapSize.ChunkDepth;
        int chunkWidth = _mapSize.ChunkWidth;

        for (int x = 0; x < chunkDepth; x++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                SetTriangle(triangles, x * (chunkDepth + 1) + z, chunkDepth + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.OptimizeReorderVertexBuffer();

        chunkHolder.GetComponent<MeshCollider>().sharedMesh = mesh;
        chunkHolder.GetComponent<MeshFilter>().mesh = mesh;
        chunkHolder.GetComponent<MeshRenderer>().material = _baseMaterial;
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
}
