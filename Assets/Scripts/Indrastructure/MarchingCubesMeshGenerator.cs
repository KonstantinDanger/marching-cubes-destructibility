using Assets.Scripts.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    public class MarchingCubesMeshGenerator
    {
        private readonly MapSize3D _mapSize;

        public MarchingCubesMeshGenerator(MapSize3D mapSize)
        {
            _mapSize = mapSize;
        }

        public void Generate(float[,,] densityMap)
        {
            GameObject go = new("Vertex holder");

            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            MeshFilter filter = go.AddComponent<MeshFilter>();

            //int width = _mapSize.Width + 1;
            //int height = _mapSize.Height + 1;
            //int depth = _mapSize.Depth + 1;

            Mesh mesh = new();
            List<Vector3> vertices = new();
            List<int> triangles = new();
            Vector3Int[] cube = new Vector3Int[8];
            int triangulationBits = 0;

            int width = 2;
            int height = 2;
            int depth = 2;

            int startX = 0;
            int startY = 0;
            int startZ = 0;

            for (int x = startX; x < width; x++)
            {
                for (int y = startY; y < height; y++)
                {
                    for (int z = startZ; z < depth; z++)
                    {
                        vertices.Add(new(x, y, z));

                        MarchCube(cube, x, y, z);

                        triangulationBits = 0;

                        for (int i = 0; i < cube.Length; i++)
                        {
                            int density = Mathf.RoundToInt(densityMap[cube[i].x, cube[i].y, cube[i].z]);
                            //Vector3 pos = new Vector3(cube[i].x, cube[i].y, cube[i].z);
                            //Helpers.CreateDummyPointAt(pos, 0.5f, Color.yellow, null);

                            if (density == 0)
                                continue;

                            triangulationBits |= (1 << i);
                        }

                        // 1) Get an array of edge indexes using
                        // "triangulationBits" variable and triangulation table (on the Internet)
                        // 2) Get the centered positions of each edge (using interpolation?)
                        // and add that position to vertices[]
                        // 3) Make a polygons based on vertices

                        int triangulationIndex = Convert.ToInt32(triangulationBits);

                        UnityEngine.Debug.Log("triangulationIndex " + triangulationIndex);

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
