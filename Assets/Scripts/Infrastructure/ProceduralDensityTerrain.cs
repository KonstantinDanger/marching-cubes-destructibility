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
            int chunkIndex = -1;

            try
            {
                int chunkWidth = _mapSize.ChunkWidth;
                int chunkHeight = _mapSize.ChunkHeight;
                int chunkDepth = _mapSize.ChunkDepth;

                int globalX = Mathf.FloorToInt(pos.x);
                int globalY = Mathf.FloorToInt(pos.y);
                int globalZ = Mathf.FloorToInt(pos.z);

                int chunkX = globalX / chunkWidth;
                int chunkY = globalY / chunkHeight;
                int chunkZ = globalZ / chunkDepth;

                int chunksNumX = (_mapSize.Width + chunkWidth - 1) / chunkWidth;
                int chunksNumY = (_mapSize.Height + chunkHeight - 1) / chunkHeight;
                int chunksNumZ = (_mapSize.Depth + chunkDepth - 1) / chunkDepth;

                if (OutOfBounds(_mapSize, globalX, globalY, globalZ))
                    return;

                // TODO: Handle chunk seams coords modification

                _densityMap[globalX, globalY, globalZ] = belowIsoSurface ? 0 : _mapSize.IsoLevel;

                //chunkIndex = chunkX * chunkHeight * chunkDepth + chunkY * chunkDepth + chunkZ;
                chunkIndex = chunkZ + chunkY * chunksNumZ + chunkX * chunksNumY * chunksNumZ;

                Chunk3D chunk = _chunks[chunkIndex];

                _meshGenerator.SetupMesh(chunk.Mesh, _densityMap, chunk.StartPos, chunk.EndPos);
                chunk.UpdateMesh();
            }
            catch (ArgumentOutOfRangeException e)
            {

                UnityEngine.Debug.Log($"Invalid index: {chunkIndex}");
            }
        }

        private static bool OutOfBounds(MapSize3D mapSize, int x, int y, int z)
            => x < 0
            || y < 0
            || z < 0
            || x >= mapSize.Width
            || y >= mapSize.Height
            || z >= mapSize.Depth;
    }
}