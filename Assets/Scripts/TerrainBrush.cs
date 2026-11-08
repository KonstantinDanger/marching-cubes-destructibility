using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TerrainBrush : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _holdUpdateTime;
    [SerializeField, Min(0f)] private float _brushRange;
    [SerializeField, Min(0f)] private float _loweringStrength;
    [SerializeField, Min(0f)] private float _raisingStrength;

    private float _timer = 0f;
    private Vector3 _mouseWorldPosition;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            _mouseWorldPosition = hit.point;
        else
            _mouseWorldPosition = Vector3.zero;

        _timer += Time.deltaTime;

        if (!IsReadyToBrush())
            return;

        if (Input.GetKey(KeyCode.Mouse2))
        { 
            Vector3 closest = FindClosestVertice(ray, out MeshCollider col, out int index);

            if (closest == Vector3.zero)
                return;

            //AddVerticeAmountY(col, index);

            DrawContactPoint(closest);

            ResetTimer();
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
            Vector3[] selectedVertices = SelectVertices(ray, out MeshCollider col, out int[] indices);

            if (selectedVertices == null)
                return;

            for (int i = 0; i < selectedVertices.Length; i++)
                AddVerticeAmountY(col, indices[i], -_loweringStrength);

            ResetTimer();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3[] selectedVertices = SelectVertices(ray, out MeshCollider col, out int[] indices);

            if (selectedVertices == null)
                return;

            for (int i = 0; i < selectedVertices.Length; i++)
                AddVerticeAmountY(col, indices[i], _raisingStrength);

            ResetTimer();
        }
    }

    private void AddVerticeAmountY(MeshCollider col, int index, float addAmount)
    {
        Mesh mesh = col.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        vertices[index].y += addAmount;
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        col.sharedMesh = null;
        col.sharedMesh = mesh;
    }

    private Vector3[] SelectVertices(Ray ray, out MeshCollider col, out int[] indices)
    {
        List<int> indicesList = new();
        indices = null;
        col = null;

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return null;

        if (!hit.collider.TryGetComponent(out col))
            return null;

        List<Vector3> selected = new();

        Vector3[] vertices = col.sharedMesh.vertices;
        Vector3 contactPoint = hit.point;
        int index = 0;

        foreach (Vector3 vertice in vertices)
        {
            float distanceToVertice = (vertice - contactPoint).magnitude;

            if (distanceToVertice <= _brushRange)
            {
                selected.Add(vertice);
                indicesList.Add(index);
            }

            index++;
        }

        indices = indicesList.ToArray();

        return selected.ToArray();
    }

    private static void DrawContactPoint(Vector3 closest)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale /= 2;
        go.GetComponent<MeshRenderer>().material.color = Color.yellow;
        go.transform.position = closest;
        Destroy(go, 3f);
    }

    private Vector3 FindClosestVertice(Ray ray, out MeshCollider col, out int index)
    {
        col = null;
        index = -1;

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return Vector3.zero;

        if (!hit.collider.TryGetComponent(out col))
            return Vector3.zero;

        Vector3[] vertices = col.sharedMesh.vertices;
        Vector3 closest = vertices[0];
        Vector3 startingPoint = hit.point;
        index = 0; 

        for (int i = 0; i < vertices.Length - 1; i++)
        {
            Vector3 next = vertices[i + 1];
            float distanceToCurrent = (closest - startingPoint).magnitude;
            float distanceToNext = (next - startingPoint).magnitude;

            if (distanceToNext < distanceToCurrent)
            {
                closest = next;
                index = i + 1;
            }
        }

        return closest;
    }

    private void ResetTimer()
    {
        _timer = 0;
    }

    private bool IsReadyToBrush()
    {
        return _timer >= _holdUpdateTime;
    }

    private void OnDrawGizmosSelected()
    {
        
    }

    private void OnDrawGizmos()
    {
        Color color = Color.blue;
        color.a = 0.6f;
        Gizmos.color = color;
        
        if(_mouseWorldPosition != Vector3.zero)
            Gizmos.DrawSphere(_mouseWorldPosition, _brushRange);
    }
}
