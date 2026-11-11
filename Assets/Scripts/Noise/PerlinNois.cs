using UnityEngine;

namespace Scripts.Noise
{
    [CreateAssetMenu(menuName = "Noise/Perlin", fileName = "Noise_Perlin")]
    public class PerlinNois : Noise
    {
        public override float Evaluate(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}