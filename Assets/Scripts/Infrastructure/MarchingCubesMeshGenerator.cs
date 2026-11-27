using System.Collections.Generic;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    public class MarchingCubesMeshGenerator
    {
        private readonly MapSize3D _mapSize;
        private readonly Material _baseMaterial;
        private readonly List<Chunk3D> _chunks = new();

        private readonly static Vector3Int[] CachedCube = new Vector3Int[8];
        private readonly static int[] CachedIndices = new int[12];

        public MarchingCubesMeshGenerator(MapSize3D mapSize, Material baseMaterial)
        {
            _mapSize = mapSize;
            _baseMaterial = baseMaterial;
        }

        public void Generate(float[,,] densityMap)
        {
            foreach (var chunk in _chunks)
                Object.Destroy(chunk.gameObject);

            _chunks.Clear();

            Transform parentObject = new GameObject("Chunk holder").transform;
            ProceduralDensityTerrain terrain = parentObject.gameObject.AddComponent<ProceduralDensityTerrain>();

            int width = densityMap.GetLength(0)  - 1;
            int height = densityMap.GetLength(1) - 1;
            int depth = densityMap.GetLength(2)  - 1;

            for (int x = 0; x < width; x += _mapSize.ChunkWidth)
            {
                for (int y = 0; y < height; y += _mapSize.ChunkHeight)
                {
                    for (int z = 0; z < depth; z += _mapSize.ChunkDepth)
                    {
                        Chunk3D chunk = CreateChunk(densityMap, _mapSize, x, y, z, terrain);
                        _chunks.Add(chunk);
                    }
                }
            }

            terrain.Initialize(densityMap, _chunks, _mapSize, this);
        }

        public void SetupMesh(Mesh mesh, float[,,] densityMap, Vector3Int start, Vector3Int end)
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            for (int x = start.x; x < end.x; x++)
            {
                for (int y = start.y; y < end.y; y++)
                {
                    for (int z = start.z; z < end.z; z++)
                    {
                        CreateCube(densityMap, x, y, z, vertices, triangles);
                    }
                }
            }

            mesh.Clear();

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        private Chunk3D CreateChunk(float[,,] densityMap, MapSize3D mapSize, int startX, int startY, int startZ, ProceduralDensityTerrain terrain)
        {
            GameObject chunkObject = new($"Chunk at [{startX}, {startY}, {startZ}] | index: {_chunks.Count}");
            chunkObject.transform.parent = terrain.transform;
            Chunk3D chunk = chunkObject.AddComponent<Chunk3D>();

            Mesh mesh = new() {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            int finalX = startX + mapSize.ChunkWidth;
            int finalY = startY + mapSize.ChunkHeight;
            int finalZ = startZ + mapSize.ChunkDepth;

            Vector3Int startPos = new (startX, startY, startZ);
            Vector3Int endPos = new(finalX, finalY, finalZ);

            SetupMesh(mesh, densityMap, startPos, endPos);

            chunk.Initialize(terrain, mesh, _baseMaterial, startPos, endPos);

            return chunk;
        }

        private void CreateCube(float[,,] densityMap, int x, int y, int z, List<Vector3> vertices, List<int> triangles)
        {
            int cubeIndex = 0;
            MarchCube(CachedCube, x, y, z);

            for (int i = 0; i < CachedCube.Length; i++)
            {
                float density = densityMap[CachedCube[i].x, CachedCube[i].y, CachedCube[i].z];
                //Vector3 pos = new(cube[i].x, cube[i].y, cube[i].z);
                //Helpers.CreateDummyPointAt(pos, 0.4f, Color.yellow, go.transform);

                if (density > _mapSize.IsoLevel)
                    continue;

                cubeIndex |= (1 << i);
            }

            for (int i = 0; i < CachedIndices.Length; i++)
                CachedIndices[i] = -1;

            for (int i = 0; i < 16; i++)
            {
                int edgeIndex = TriangulationTable.Triangulation[cubeIndex, i];

                if (edgeIndex == -1)
                    break;

                if (CachedIndices[edgeIndex] == -1)
                {
                    int cornerA = TriangulationTable.CornerIndexFromEdge[edgeIndex, 0];
                    int cornerB = TriangulationTable.CornerIndexFromEdge[edgeIndex, 1];

                    Vector3 edgeCenterPos;

                    float isoLevel = _mapSize.IsoLevel;
                    float isoLevelA = densityMap[CachedCube[cornerA].x, CachedCube[cornerA].y, CachedCube[cornerA].z];
                    float isoLevelB = densityMap[CachedCube[cornerB].x, CachedCube[cornerB].y, CachedCube[cornerB].z];

                    float t = Mathf.Clamp01((isoLevel - isoLevelA) / (isoLevelB - isoLevelA));

                    edgeCenterPos = Vector3.Lerp(CachedCube[cornerA], CachedCube[cornerB], t);
                    
                    // For low-poly shape:
                    //edgeCenterPos = Vector3.Lerp(cube[cornerA], cube[cornerB], 0.5f); 

                    vertices.Add(edgeCenterPos);
                    
                    CachedIndices[edgeIndex] = vertices.Count - 1;
                }

                triangles.Add(CachedIndices[edgeIndex]);
            }
        }

        private static void MarchCube(Vector3Int[] cube, int x, int y, int z)
        {
            cube[0] = new(x, y, z);
            cube[1] = new(x + 1, y, z);
            cube[2] = new(x + 1, y + 1, z);
            cube[3] = new(x, y + 1, z);
            cube[4] = new(x, y, z + 1);
            cube[5] = new(x + 1, y, z + 1);
            cube[6] = new(x + 1, y + 1, z + 1);
            cube[7] = new(x, y + 1, z + 1);
        }
    }
}
