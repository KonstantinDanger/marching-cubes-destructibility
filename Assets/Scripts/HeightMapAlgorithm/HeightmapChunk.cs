using System;
using UnityEngine;

namespace Scripts.Heightmap
{
    [DisallowMultipleComponent]
    public class HeightmapChunk : MonoBehaviour
    {
        private ProceduralHeightMapTerrain _terrain;

        private MeshCollider _meshCollider;
        private MeshRenderer _renderer;
        private MeshFilter _filter;
        private Mesh _mesh;

        public Vector3[] Vertices => _mesh.vertices;

        public int StartX { get; private set; }
        public int StartZ { get; private set; }

        public void Initialize(ProceduralHeightMapTerrain terrain, Mesh mesh, Material baseMaterial, int startX, int startZ)
        {
            _terrain = terrain;

            _renderer = gameObject.AddComponent<MeshRenderer>();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _filter = gameObject.AddComponent<MeshFilter>();

            _mesh = mesh;
            _filter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _renderer.material = baseMaterial;

            StartX = startX;
            StartZ = startZ;
        }

        public void UpdateChunk(int localX, int localZ, int chunkDepth, float height)
        {
            int index = localX * (chunkDepth + 1) + localZ;

            if (index < 0 || index >= _mesh.vertices.Length)
                return;

            Vector3[] vertices = _mesh.vertices;

            vertices[index].y = height;

            _mesh.vertices = vertices;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }

        public void UpdateChunk(Vector3[] vertices)
        {
            _mesh.vertices = vertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }

        /// <summary>
        /// Should NOT be called from terrain
        /// </summary>
        /// <param name="globalX"></param>
        /// <param name="globalZ"></param>
        /// <param name="heightAmount"></param>
        public void ModifyVertexHeight(float globalX, float globalZ, float heightAmount, object sender)
        {
            if (sender.GetType() == typeof(ProceduralHeightMapTerrain))
                return;

            _terrain.ModifyVertexHeight(globalX, globalZ, heightAmount);
        }
    }
}