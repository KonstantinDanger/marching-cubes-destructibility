using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Chunk : MonoBehaviour
{
    private ProceduralTerrain _terrain;

    private MeshCollider _meshCollider;
    private MeshRenderer _renderer;
    private MeshFilter _filter;
    private Mesh _mesh;

    public IReadOnlyList<Vector3> Vertices => _mesh.vertices;

    public void Initialize(ProceduralTerrain terrain, Mesh mesh, Material baseMaterial)
    {
        _terrain = terrain;

        _renderer = gameObject.AddComponent<MeshRenderer>();
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _filter = gameObject.AddComponent<MeshFilter>();

        _mesh = mesh;
        _filter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
        _renderer.material = baseMaterial;
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
        if (sender.GetType() == typeof(ProceduralTerrain))
            return;

        _terrain.ModifyVertexHeight(globalX, globalZ, heightAmount);
    }
}
