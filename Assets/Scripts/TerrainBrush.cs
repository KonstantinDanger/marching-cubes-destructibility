using System.Collections.Generic;
using System.Linq;
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

        //if (Input.GetKey(KeyCode.Mouse2))
        //{ 
        //    Vector3 closest = FindClosestVertice(ray, out MeshCollider col, out int index);

        //    if (closest == Vector3.zero)
        //        return;

        //    //AddVerticeAmountY(col, index);

        //    DrawContactPoint(closest);

        //    ResetTimer();
        //}

        if(Input.GetKey(KeyCode.Mouse1))
        {
            Vector3[] selectedVertices = SelectVertices(ray, out Chunk chunk);

            if (selectedVertices == null)
                return;

            for (int i = 0; i < selectedVertices.Length; i++)
            {
                Vector3 selectedPos = selectedVertices[i];
                chunk.ModifyVertexHeight(selectedPos.x, selectedPos.z, -_loweringStrength, this);
            }

            ResetTimer();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3[] selectedVertices = SelectVertices(ray, out Chunk chunk);

            if (selectedVertices == null)
                return;

            for (int i = 0; i < selectedVertices.Length; i++)
            {
                Vector3 selectedPos = selectedVertices[i];
                chunk.ModifyVertexHeight(selectedPos.x, selectedPos.z, _raisingStrength, this);
            }

            ResetTimer();
        }
    }

    private Vector3[] SelectVertices(Ray ray, out Chunk chunk)
    {
        chunk = null;

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return null;

        if (!hit.collider.TryGetComponent(out chunk))
            return null;

        List<Vector3> selected = new();

        Vector3[] vertices = chunk.Vertices.ToArray();
        Vector3 contactPoint = hit.point;
        int index = 0;

        foreach (Vector3 vertice in vertices)
        {
            float distanceToVertice = (vertice - contactPoint).magnitude;

            if (distanceToVertice <= _brushRange)
            {
                selected.Add(vertice);
            }

            index++;
        }

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

    private Vector3 FindClosestVertice(Ray ray, out Chunk chunk, out int index)
    {
        chunk = null;
        index = -1;

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return Vector3.zero;

        if (!hit.collider.TryGetComponent(out chunk))
            return Vector3.zero;

        Vector3[] vertices = chunk.Vertices.ToArray();
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

    private void OnDrawGizmos()
    {
        Color color = Color.blue;
        color.a = 0.6f;
        Gizmos.color = color;
        
        if(_mouseWorldPosition != Vector3.zero)
            Gizmos.DrawSphere(_mouseWorldPosition, _brushRange);
    }
}
