using Assets.Scripts.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    public class MarchingCubesMeshGenerator
    {
        private readonly MapSize3D _mapSize;
        private readonly Material _baseMaterial;

        public MarchingCubesMeshGenerator(MapSize3D mapSize, Material baseMaterial)
        {
            _mapSize = mapSize;
            _baseMaterial = baseMaterial;
        }

        public void Generate(float[,,] densityMap)
        {
            Transform parentObject = new GameObject("Chunk holder").transform;

            int width = densityMap.GetLength(0)  - 1;
            int height = densityMap.GetLength(1) - 1;
            int depth = densityMap.GetLength(2)  - 1;

            for (int x = 0; x < width; x += _mapSize.ChunkWidth)
            {
                for (int y = 0; y < height; y += _mapSize.ChunkHeight)
                {
                    for (int z = 0; z < depth; z += _mapSize.ChunkDepth)
                    {
                        CreateChunk(densityMap, _mapSize, x, y, z, parentObject);
                    }
                }
            }
        }

        private Chunk3D CreateChunk(float[,,] densityMap, MapSize3D mapSize, int startX, int startY, int startZ, Transform parentObject)
        {
            GameObject chunkObject = new($"Chunk at [{startX}, {startY}, {startZ}]");
            Chunk3D chunk = chunkObject.AddComponent<Chunk3D>();
            MeshRenderer renderer = chunkObject.AddComponent<MeshRenderer>();
            MeshFilter filter = chunkObject.AddComponent<MeshFilter>();
            renderer.material = _baseMaterial;

            Mesh mesh = new();
            List<Vector3> vertices = new();
            List<int> triangles = new();
            Vector3Int[] cube = new Vector3Int[8];

            int finalX = startX + mapSize.ChunkWidth;
            int finalY = startY + mapSize.ChunkHeight;
            int finalZ = startZ + mapSize.ChunkDepth;

            for (int x = startX; x < finalX; x++)
            {
                for (int y = startY; y < finalY; y++)
                {
                    for (int z = startZ; z < finalZ; z++)
                    {
                        CreateCube(densityMap, cube, x, y, z, vertices, triangles);
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            filter.mesh = mesh;

            return chunk;
        }

        private void CreateCube(float[,,] densityMap, Vector3Int[] cube, int x, int y, int z, List<Vector3> vertices, List<int> triangles)
        {
            int cubeIndex = 0;
            MarchCube(cube, x, y, z);

            for (int i = 0; i < cube.Length; i++)
            {
                float density = densityMap[cube[i].x, cube[i].y, cube[i].z];
                //Vector3 pos = new(cube[i].x, cube[i].y, cube[i].z);
                //Helpers.CreateDummyPointAt(pos, 0.4f, Color.yellow, go.transform);

                if (density < _mapSize.IsoLevel)
                    continue;

                cubeIndex |= (1 << i);
            }

            int[] cachedIndices = new int[12];

            for (int i = 0; i < cachedIndices.Length; i++)
                cachedIndices[i] = -1;

            for (int i = 0; i < 16; i++)
            {
                int edgeIndex = TriangulationTable.Triangulation[cubeIndex, i];

                if (edgeIndex == -1)
                    break;

                if (cachedIndices[edgeIndex] == -1)
                {
                    int cornerA = TriangulationTable.CornerIndexFromEdge[edgeIndex, 0];
                    int cornerB = TriangulationTable.CornerIndexFromEdge[edgeIndex, 1];

                    Vector3 edgeCenterPos;

                    float isoLevel = _mapSize.IsoLevel;
                    float isoLevelA = densityMap[cube[cornerA].x, cube[cornerA].y, cube[cornerA].z];
                    float isoLevelB = densityMap[cube[cornerB].x, cube[cornerB].y, cube[cornerB].z];

                    if (Mathf.Abs(isoLevel - isoLevelA) < 0.00001f)
                        edgeCenterPos = cube[cornerA];
                    else if (Mathf.Abs(isoLevel - isoLevelB) < 0.00001f)
                        edgeCenterPos = (cube[cornerB]);
                    else if (Mathf.Abs(isoLevelA - isoLevelB) < 0.00001f)
                        edgeCenterPos = (cube[cornerA]);
                    else
                    {
                        float t = (isoLevel - isoLevelA) / (isoLevelB - isoLevelA);

                        edgeCenterPos = Vector3.Lerp(cube[cornerA], cube[cornerB], t);
                    }

                    // For low-poly shape:
                    //edgeCenterPos = Vector3.Lerp(cube[cornerA], cube[cornerB], _mapSize.IsoLevel); 

                    vertices.Add(edgeCenterPos);

                    cachedIndices[edgeIndex] = vertices.Count - 1;
                }

                triangles.Add(cachedIndices[edgeIndex]);
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
