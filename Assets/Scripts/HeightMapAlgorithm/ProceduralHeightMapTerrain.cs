using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Heightmap
{
    [DisallowMultipleComponent]
    public class ProceduralHeightMapTerrain : MonoBehaviour
    {
        private float[,] _heightMap;
        private MapSize2D _mapSize;
        private List<HeightmapChunk> _chunks;

        public void Initialize(float[,] heightMap, MapSize2D mapSize, List<HeightmapChunk> chunks)
        {
            _heightMap = heightMap;
            _mapSize = mapSize;
            _chunks = chunks;
        }

        public void ModifyVertexHeight(float globalX, float globalZ, float heightAmount)
        {
            int roundedX = Mathf.RoundToInt(globalX);
            int roundedZ = Mathf.RoundToInt(globalZ);

            if (roundedX < 0 || roundedZ < 0 || roundedZ >= _mapSize.Depth || roundedX >= _mapSize.Width)
                return;

            _heightMap[roundedX, roundedZ] += heightAmount;

            List<HeightmapChunk> chunks = SelectChunks(_chunks, _mapSize, roundedX, roundedZ);

            if (chunks == null)
                return;

            foreach (HeightmapChunk chunk in chunks)
            {
                int localX = roundedX - chunk.StartX;
                int localZ = roundedZ - chunk.StartZ;
                chunk.UpdateChunk(localX, localZ, _mapSize.ChunkDepth, _heightMap[roundedX, roundedZ]);
            }
        }

        private static List<HeightmapChunk> SelectChunks(List<HeightmapChunk> allChunks, MapSize2D mapSize, int globalX, int globalZ)
        {
            List<HeightmapChunk> chunks = new();

            int localX = globalX % mapSize.ChunkWidth;
            int localZ = globalZ % mapSize.ChunkDepth;

            TryAddChunk(allChunks, chunks, mapSize, globalX, globalZ);

            if (localX == 0)
                TryAddChunk(allChunks, chunks, mapSize, globalX - 1, globalZ);

            if (localZ == 0)
                TryAddChunk(allChunks, chunks, mapSize, globalX, globalZ - 1);

            if (localX == 0 && localZ == 0)
                TryAddChunk(allChunks, chunks, mapSize, globalX - 1, globalZ - 1);

            return chunks;
        }

        private static void TryAddChunk(List<HeightmapChunk> allChunks, List<HeightmapChunk> chunks, MapSize2D mapSize, int globalX, int globalZ)
        {
            int index = GetChunkIndex(mapSize, globalX, globalZ);

            if (index < 0 || index > allChunks.Count)
                return;

            chunks.Add(allChunks[index]);
        }

        private static int GetChunkIndex(MapSize2D mapSize, int roundedX, int roundedZ)
        {
            int chunkX = Mathf.FloorToInt(roundedX / mapSize.ChunkWidth);
            int chunkZ = Mathf.FloorToInt(roundedZ / mapSize.ChunkDepth);
            int chunkColumns = mapSize.Depth / mapSize.ChunkDepth;

            return chunkX * chunkColumns + chunkZ;
        }
    }
}
