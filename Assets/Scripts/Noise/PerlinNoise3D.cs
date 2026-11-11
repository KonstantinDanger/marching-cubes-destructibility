using UnityEngine;

namespace Scripts.Noise
{
    [CreateAssetMenu(menuName = "Noise/3D/Perlin", fileName = "Noise_3D_Perlin")]
    public class PerlinNoise3D : Noise3D
    {
        public override float Evaluate(float x, float y, float z)
        {
            float ab = Mathf.PerlinNoise(x, y);
            float bc = Mathf.PerlinNoise(y, z);
            float ac = Mathf.PerlinNoise(x, z);

            float ba = Mathf.PerlinNoise(y, x);
            float cb = Mathf.PerlinNoise(z, y);
            float ca = Mathf.PerlinNoise(z, x);

            float abc = ab + bc + ac + ba + cb + ca;

            return abc / 6.0f;
        }
    }
}
