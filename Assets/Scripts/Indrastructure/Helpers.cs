using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public static class Helpers
    {
        public static GameObject CreateDummyPointAt(Vector3 position, float size, Color color, Transform parent, string pointName = "Sample point")
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = pointName; 
            go.transform.parent = parent;
            go.transform.localScale *= size;
            go.transform.position = new(position.x, position.y, position.z);

            if (go.TryGetComponent(out Collider col))
                Object.Destroy(col);

            if (go.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                SetTransparency(renderer.material, color.a < 1.0f);
                renderer.material.color = color;
            }

            return go;
        }

        private static void SetTransparency(Material mat, bool isTransparent)
        {
            if (isTransparent)
            {
                mat.SetFloat("_Surface", 1.0f); // 0 = Opaque, 1 = Transparent

                mat.SetInt("_Blend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0); // Turn off depth writing for transparency

                // Enable keywords and set render queue
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                // Switch to Opaque Surface Type
                mat.SetFloat("_Surface", 0.0f);

                mat.SetInt("_Blend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt("_ZWrite", 1); // Turn on depth writing

                mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }
        }
    }
}
