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
            _filter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
            _renderer.sharedMaterial = baseMaterial;
        }

        public void ModifyDensity(Vector3 pos, bool belowIsoSurface, object sender)
        {
            if (sender is ProceduralDensityTerrain)
                return;

            _terrain.ModifyDensity(pos, belowIsoSurface);
        }

        public void UpdateMesh()
        {
            if (_mesh.vertexCount == 0)
            {
                _meshCollider.enabled = false;

                return;
            }

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
            _meshCollider.enabled = true;
        }
    }
}