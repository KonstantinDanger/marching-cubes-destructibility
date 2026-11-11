using UnityEngine;

namespace Scripts.Noise
{
    public abstract class Noise2D : ScriptableObject
    {
        public abstract float Evaluate(float x, float y);
    }
}
