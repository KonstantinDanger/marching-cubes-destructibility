using UnityEngine;

namespace Scripts.Noise
{
    [CreateAssetMenu(menuName = "Noise/3D/Fractal", fileName = "Noise_3D_FractalNoise")]
    public class FractalNoise3D : Noise3D
    {
        [SerializeField] private Noise3D _noise3D;
        [Header("Params")]
        [SerializeField, Min(0)] private int _octaves = 1;
        [SerializeField, Min(0f)] private float _persistence = 0.5f;
        [SerializeField, Min(0f)] private float _lacunarity = 2.0f;

        public override float Evaluate(float x, float y, float z)
        {
            float total = 0f;
            float frequency = 1f;
            float amplitude = 1f;
            float maxValue = 0.000001f;

            for (int i = 0; i < _octaves; i++)
            {
                total += _noise3D.Evaluate(x * frequency, y * frequency, z * frequency) * amplitude;

                maxValue += amplitude;
                amplitude *= _persistence;
                frequency *= _lacunarity;
            }

            return total / maxValue;
        }
    }
}
