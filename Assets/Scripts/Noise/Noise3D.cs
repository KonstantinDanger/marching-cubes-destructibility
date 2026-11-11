using UnityEngine;

namespace Scripts.Noise
{
    public abstract class Noise3D : Noise2D
    {
        public abstract float Evaluate(float x, float y, float z);

        public override float Evaluate(float x, float y)
        {
            return 0f;   
        }
    }
}
