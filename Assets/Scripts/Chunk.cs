using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Chunk : MonoBehaviour
{
    private float[,] _heightMap;
    private MeshCollider _meshCollider;
    private MeshRenderer _renderer;
    private MeshFilter _filter;
    private Mesh _mesh;

    public IReadOnlyList<Vector3> Vertices => _mesh.vertices;

    public void Initialize(float[,] heightMap, Mesh mesh, Material baseMaterial)
    {
        _heightMap = heightMap;

        _renderer = gameObject.AddComponent<MeshRenderer>();
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _filter = gameObject.AddComponent<MeshFilter>();

        _mesh = mesh;
        _filter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
        _renderer.material = baseMaterial;
    }

    public void ModifyHeight(int vertexIndex, float heightAmount)
    {
        Vector3[] vertices = _mesh.vertices;
        vertices[vertexIndex].y += heightAmount;

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = _mesh;
    }
}
