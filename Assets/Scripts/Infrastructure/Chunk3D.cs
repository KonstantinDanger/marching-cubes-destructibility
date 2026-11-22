using System.Collections.Generic;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    [DisallowMultipleComponent]
    public class Chunk3D : MonoBehaviour
    {
        private ProceduralDensityTerrain _terrain;

        private MeshRenderer _renderer;
        private MeshCollider _meshCollider;
        private MeshFilter _filter;
        private Mesh _mesh;

        public Vector3[] Vertices => _mesh.vertices;
        public Mesh Mesh => _mesh;

        public Vector3Int StartPos { get; private set; }

        public Vector3Int EndPos { get; private set; }

        public void Initialize(ProceduralDensityTerrain terrain, Mesh mesh, Material baseMaterial, Vector3Int startPos, Vector3Int endPos)
        {
            StartPos = startPos;
            EndPos = endPos;

            _terrain = terrain;

            _renderer = gameObject.AddComponent<MeshRenderer>();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _filter = gameObject.AddComponent<MeshFilter>();

            _mesh = mesh;
            _filter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _renderer.material = baseMaterial;
        }

        public void ModifyDensity(Vector3 pos, bool belowIsoSurface, object sender)
        {
            if (sender.GetType() == typeof(ProceduralDensityTerrain))
                return;

            _terrain.ModifyDensity(pos, belowIsoSurface);
        }

        public void UpdateMesh()
        {
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }
    }
}