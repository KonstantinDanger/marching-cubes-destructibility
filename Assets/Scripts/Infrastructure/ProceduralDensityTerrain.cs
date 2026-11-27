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
            int globalX = Mathf.RoundToInt(pos.x);
            int globalY = Mathf.RoundToInt(pos.y);
            int globalZ = Mathf.RoundToInt(pos.z);

            if (OutOfBounds(_mapSize, globalX, globalY, globalZ))
                return;

            _densityMap[globalX, globalY, globalZ] = belowIsoSurface ? _mapSize.IsoLevel - 1 : _mapSize.IsoLevel + 1;

            Vector3Int globalPos = new(globalX, globalY, globalZ);

            UpdateChunks(globalPos);
        }

        private void UpdateChunks(Vector3Int globalPos)
        {
            Chunk3D startingChunk = _chunks[GetChunkIndex(globalPos)];

            int localX = globalPos.x % _mapSize.ChunkWidth;
            int localY = globalPos.y % _mapSize.ChunkHeight;
            int localZ = globalPos.z % _mapSize.ChunkDepth;

            Vector3Int start = startingChunk.StartPos;
            Vector3Int shift = start;

            ModifyAxesShifts(localX, localY, localZ, ref shift);

            List<Vector3Int> neighbourChunkPositions = new()
            {
                new (start.x, start.y, shift.z),

                new (start.x, shift.y, start.z),
                new (start.x, shift.y, shift.z),

                new (shift.x, start.y, start.z),
                new (shift.x, start.y, shift.z),

                new (shift.x, shift.y, start.z),
                new (shift.x, shift.y, shift.z),
            };

            int index = -1;

            try
            {
                foreach (Vector3Int pos in neighbourChunkPositions)
                {
                    if (pos != start)
                    {
                        index = GetChunkIndex(pos);
                        
                        if (index >= _chunks.Count || index < 0)
                            continue;

                        Chunk3D chunk = _chunks[index];
                        _meshGenerator.SetupMesh(chunk.Mesh, _densityMap, chunk.StartPos, chunk.EndPos);
                        chunk.UpdateMesh();
                    }
                }
            }
            catch
            {
                UnityEngine.Debug.Log("Incorrect index: " + index + " | chunks total:" + _chunks.Count);
            }

            _meshGenerator.SetupMesh(startingChunk.Mesh, _densityMap, startingChunk.StartPos, startingChunk.EndPos);
            startingChunk.UpdateMesh();
        }

        private void ModifyAxesShifts(int localX, int localY, int localZ, ref Vector3Int shift)
        {
            if (localX == 0)
                shift.x -= _mapSize.ChunkWidth;
            else if (localX == 1)
                shift.x += _mapSize.ChunkWidth;

            if (localY == 0)
                shift.y -= _mapSize.ChunkHeight;
            else if (localY == 1)
                shift.y += _mapSize.ChunkHeight;

            if (localZ == 0)
                shift.z -= _mapSize.ChunkDepth;
            else if (localZ == 1)
                shift.z += _mapSize.ChunkDepth;
        }

        private int GetChunkIndex(Vector3Int globalPos)
        {
            int chunkWidth = _mapSize.ChunkWidth;
            int chunkHeight = _mapSize.ChunkHeight;
            int chunkDepth = _mapSize.ChunkDepth;

            int chunkX = globalPos.x / chunkWidth;
            int chunkY = globalPos.y / chunkHeight;
            int chunkZ = globalPos.z / chunkDepth;

            int chunksNumY = (_mapSize.Height + chunkHeight - 1) / chunkHeight;
            int chunksNumZ = (_mapSize.Depth + chunkDepth - 1) / chunkDepth;

            int chunkIndex = chunkZ + chunkY * chunksNumZ + chunkX * chunksNumY * chunksNumZ;
            return chunkIndex;
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