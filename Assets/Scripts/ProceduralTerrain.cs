using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class ProceduralTerrain : MonoBehaviour
{
    private float[,] _heightMap;
    private MapSize _mapSize;
    private List<Chunk> _chunks;

    public void Initialize(float[,] heightMap, MapSize mapSize, List<Chunk> chunks)
    {
        _heightMap = heightMap;
        _mapSize = mapSize;
        _chunks = chunks;
    }

    public void ModifyVertexHeight(float globalX, float globalZ, float heightAmount)
    {
        int roundedX = Mathf.RoundToInt(globalX);
        int roundedZ = Mathf.RoundToInt(globalZ);

        _heightMap[roundedX, roundedZ] += heightAmount;

        List<Chunk> chunks = SelectChunks(_chunks, _mapSize, roundedX, roundedZ);

        if (chunks == null)
            return;

        foreach (Chunk chunk in chunks)
            UpdateChunk(chunk, ref _heightMap);
    }

    private static List<Chunk> SelectChunks(List<Chunk> allChunks, MapSize mapSize, int globalX, int globalZ)
    {
        List<Chunk> chunks = new();

        int localX = globalX % mapSize.ChunkWidth;
        int localZ = globalZ % mapSize.ChunkDepth;

        int chunkWidth = mapSize.ChunkWidth;
        int chunkDepth = mapSize.ChunkDepth;

       //localX == 0 || localZ == 0 || localX == chunkWidth + 1 || localZ == chunkDepth + 1;

        chunks.Add(allChunks[GetChunkIndex(mapSize, globalX, globalZ)]);

        if (localX == 0)
            chunks.Add(allChunks[GetChunkIndex(mapSize, globalX - 1, globalZ)]);
        
        if (localZ == 0)
            chunks.Add(allChunks[GetChunkIndex(mapSize, globalX, globalZ  - 1)]);
        
        if (localX == 0 && localZ == 0)
            chunks.Add(allChunks[GetChunkIndex(mapSize, globalX - 1, globalZ - 1)]);
        
        return chunks;
    }

    private static int GetChunkIndex(MapSize mapSize, int roundedX, int roundedZ)
    {
        int chunkX = roundedX / mapSize.ChunkWidth;
        int chunkZ = roundedZ / mapSize.ChunkDepth;
        int chunkRows = mapSize.Width / mapSize.ChunkWidth;

        return chunkX * chunkRows + chunkZ;
    }

    private static void UpdateChunk(Chunk chunk, ref float[,] heightMap)
    {
        Vector3[] vertices = chunk.Vertices.ToArray();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            vertex.y = heightMap[Mathf.RoundToInt(vertex.x), Mathf.RoundToInt(vertex.z)];
            vertices[i] = vertex;
        }

        chunk.UpdateChunk(vertices);
    }
}
