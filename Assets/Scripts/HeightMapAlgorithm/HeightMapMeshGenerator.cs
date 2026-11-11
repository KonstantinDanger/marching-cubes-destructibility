using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Heightmap
{
    public class HeightMapMeshGenerator
    {
        private readonly GameObject _meshHolder;
        private readonly MapSize _mapSize;
        private readonly Material _baseMaterial;
        private readonly List<HeightmapChunk> _chunks = new();

        public HeightMapMeshGenerator(GameObject meshHolder, MapSize mapSize, Material baseMaterial)
        {
            _meshHolder = meshHolder;
            _mapSize = mapSize;
            _baseMaterial = baseMaterial;
        }

        public void Generate(float[,] heightMap)
        {
            Transform terrainObject = _meshHolder.transform.Find("Terrain");

            if (!terrainObject)
            {
                terrainObject = new GameObject("Terrain").transform;
                terrainObject.parent = _meshHolder.transform;
            }

            ProceduralHeightMapTerrain terrain = terrainObject.gameObject.AddComponent<ProceduralHeightMapTerrain>();

            for (int x = 0; x < _mapSize.Width; x += _mapSize.ChunkWidth)
                for (int z = 0; z < _mapSize.Depth; z += _mapSize.ChunkDepth)
                    _chunks.Add(CreateChunk(heightMap, x, z, terrain, terrainObject));

            terrain.Initialize(heightMap, _mapSize, _chunks);

        }

        private HeightmapChunk CreateChunk(float[,] heightMap, int startingX, int startingZ, ProceduralHeightMapTerrain terrain, Transform terrainObject)
        {
            Mesh mesh = new();
            GameObject chunkHolder = new("Chunk holder");
            HeightmapChunk chunk = chunkHolder.AddComponent<HeightmapChunk>();

            chunkHolder.transform.parent = terrainObject;

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
                        (float)(z - startingZ) / (_mapSize.ChunkDepth)));
                }
            }

            int chunkDepth = _mapSize.ChunkDepth;
            int chunkWidth = _mapSize.ChunkWidth;

            for (int x = 0; x < chunkWidth; x++)
            {
                for (int z = 0; z < chunkDepth; z++)
                {
                    SetTriangle(triangles, x * (chunkDepth + 1) + z, chunkDepth + 1);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            chunk.Initialize(terrain, mesh, _baseMaterial, startingX, startingZ);

            return chunk;
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
}
