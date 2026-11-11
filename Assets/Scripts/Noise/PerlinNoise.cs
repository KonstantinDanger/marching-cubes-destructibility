using UnityEngine;

namespace Scripts.Noise
{
    [CreateAssetMenu(menuName = "Noise/2D/Perlin", fileName = "Noise_2D_Perlin")]
    public class PerlinNoise : Noise2D
    {
        public override float Evaluate(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}