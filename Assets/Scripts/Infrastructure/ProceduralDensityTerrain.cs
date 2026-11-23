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
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            int z = Mathf.RoundToInt(pos.z);

            if (OutOfBounds(_mapSize, x, y, z))
                return;

            // TODO: Handle chunk seams coords modification

            _densityMap[x, y, z] = belowIsoSurface ? 0 : _mapSize.IsoLevel;
            
            int localX = x % _mapSize.ChunkWidth;
            int localY = y % _mapSize.ChunkHeight;
            int localZ = z % _mapSize.ChunkDepth;

            int xy = localX * (_mapSize.ChunkHeight + 1) + localY;
            int chunkIndex = xy * (_mapSize.ChunkDepth + 1) + localZ;
            Chunk3D chunk = _chunks[chunkIndex]; 

            _meshGenerator.SetupMesh(chunk.Mesh, _densityMap, chunk.StartPos, chunk.EndPos);
            chunk.UpdateMesh();
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