using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    public class ProceduralDensityTerrain : MonoBehaviour
    {
        private float[,,] _densityMap;
        private List<Chunk3D> _chunks;
        private MapSize3D _mapSize;
        private MarchingCubesMeshGenerator _meshGenerator;

        public void Initialize(float[,,] densityMap, List<Chunk3D> chunks, MapSize3D mapSize, MarchingCubesMeshGenerator meshGenerator)
        {
            _densityMap = densityMap;
            _chunks = chunks;
            _mapSize = mapSize;
            _meshGenerator = meshGenerator;
        }

        public void ModifyDensity(Vector3 pos, bool belowIsoSurface)
        {
            int chunkWidth = _mapSize.ChunkWidth;
            int chunkHeight = _mapSize.ChunkHeight;
            int chunkDepth = _mapSize.ChunkDepth;

            int globalX = Mathf.FloorToInt(pos.x);
            int globalY = Mathf.FloorToInt(pos.y);
            int globalZ = Mathf.FloorToInt(pos.z);

            if (OutOfBounds(_mapSize, globalX, globalY, globalZ))
                return;

            int chunkX = globalX / chunkWidth;
            int chunkY = globalY / chunkHeight;
            int chunkZ = globalZ / chunkDepth;

            int chunksNumX = (_mapSize.Width + chunkWidth - 1) / chunkWidth;
            int chunksNumY = (_mapSize.Height + chunkHeight - 1) / chunkHeight;
            int chunksNumZ = (_mapSize.Depth + chunkDepth - 1) / chunkDepth;

            _densityMap[globalX, globalY, globalZ] = belowIsoSurface ? _mapSize.IsoLevel - 1 : _mapSize.IsoLevel + 1;

            UpdateChunks(pos, new(globalX, globalY, globalZ), new(chunkX, chunkY, chunkZ), new(chunksNumX, chunksNumY, chunksNumZ));
        }

        private void UpdateChunks(Vector3 modificationPos, Vector3Int globalPos, Vector3Int chunkPos, Vector3Int chunksNums)
        {
            int chunkIndex = -1;

            try
            {
                List<Chunk3D> chunks = new();

                int localX = Mathf.FloorToInt(modificationPos.x % _mapSize.ChunkWidth);
                int localY = Mathf.FloorToInt(modificationPos.y % _mapSize.ChunkHeight);
                int localZ = Mathf.FloorToInt(modificationPos.z % _mapSize.ChunkDepth);

                //bool pointOnEdgeOrCorner = localX == 0 || localY == 0 || localZ == 0 || localX == _mapSize.ChunkWidth || localY == _mapSize.ChunkHeight || localZ == _mapSize.ChunkDepth;
                //bool pointOnTheFace = !pointOnEdgeOrCorner;

                //if (pointOnEdgeOrCorner)
                //{
                //    // Update all 4 neighbour chunks
                //}
                //else if ()
                // else if point is on the (edge || corner) -> index = ...; chunks.Add(_chunks[chunkIndex]);  
                // else if point is on the face ->  index = ...; chunks.Add(_chunks[chunkIndex]);

                // Add targeted chunk in any case
                chunkIndex = chunkPos.z + chunkPos.y * chunksNums.z + chunkPos.x * chunksNums.y * chunksNums.z;
                chunks.Add(_chunks[chunkIndex]);

                foreach (var chunk in chunks)
                {
                    _meshGenerator.SetupMesh(chunk.Mesh, _densityMap, chunk.StartPos, chunk.EndPos);
                    chunk.UpdateMesh();
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log($"Invalid index: {chunkIndex}");
            }
        }

        private static bool OutOfBounds(MapSize3D mapSize, int x, int y, int z)
            => x < 0
            || y < 0
            || z < 0
            || x > mapSize.Width
            || y > mapSize.Height
            || z > mapSize.Depth;
    }
}