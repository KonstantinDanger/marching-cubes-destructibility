using Assets.Scripts.Infrastructure;
using System;
using System.Collections.Generic;
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
            GameObject go = new("Vertex holder");

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            MeshFilter filter = go.AddComponent<MeshFilter>();
            renderer.material = _baseMaterial;
            //int width = _mapSize.Width + 1;
            //int height = _mapSize.Height + 1;
            //int depth = _mapSize.Depth + 1;

            Mesh mesh = new();
            List<Vector3> vertices = new();
            List<int> triangles = new();
            Vector3Int[] cube = new Vector3Int[8];
            int cubeIndex = 0;

            int width = 8;
            int height = 8;
            int depth = 8;

            int startX = 0;
            int startY = 0;
            int startZ = 0;

            for (int x = startX; x < width; x++)
            {
                for (int y = startY; y < height; y++)
                {
                    for (int z = startZ; z < depth; z++)
                    {
                        //vertices.Add(new(x, y, z));

                        MarchCube(cube, x, y, z);

                        cubeIndex = 0;

                        for (int i = 0; i < cube.Length; i++)
                        {
                            int density = Mathf.RoundToInt(densityMap[cube[i].x, cube[i].y, cube[i].z]);
                            Vector3 pos = new(cube[i].x, cube[i].y, cube[i].z);
                            Helpers.CreateDummyPointAt(pos, 0.4f, Color.yellow, go.transform);

                            if (density == 0)
                                continue;

                            cubeIndex |= (1 << i);
                        }

                        for (int i = 0; i < 16; i++)
                        {
                            int edgeIndex = TriangulationTable.Triangulation[cubeIndex, i];

                            if (edgeIndex == -1)
                                break;

                            int index1 = TriangulationTable.CornerIndexFromEdge[edgeIndex];
                            int index2 = TriangulationTable.CornerIndexFromEdge[edgeIndex];

                            Vector3 edgeCenterPos = (cube[index1] + cube[index2]) / 2;
                            vertices.Add(edgeCenterPos);
                            triangles.Add(i);
                        }


                        // 1) Get an array of edge indexes using
                        // "triangulationBits" variable and triangulation table (on the Internet)
                        // 2) Get the centered positions of each edge (using interpolation?)
                        // and add that position to vertices[]
                        // 3) Make a polygons based on vertices

                        //int xy = x * (height - 1) + y;
                        //int xyz = xy * (depth - 1) + z;
                        //SetTriangle(triangles, xyz, depth - 1);
                    }
                }
            }

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        for (int z = 0; z < depth; z++)
            //        {
            //            vertices.Add(new(x, y, z));
            //        }
            //    }
            //}

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            filter.mesh = mesh;
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
