using UnityEngine;

namespace Assets.Scripts.Indrastructure
{
    public static class Helpers
    {
        public static GameObject CreateDummyPointAt(Vector3 position, float size, Color color, Transform parent, string pointName = "Sample point")
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = pointName; 
            go.transform.parent = parent;
            go.GetComponent<MeshRenderer>().material.color = color;
            go.transform.localScale *= size;
            go.transform.position = new(position.x, position.y, position.z);

            if (go.TryGetComponent(out Collider col))
                Object.Destroy(col);

            if (go.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            return go;
        }
    }
}
