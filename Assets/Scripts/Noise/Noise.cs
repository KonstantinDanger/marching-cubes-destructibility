using UnityEngine;

namespace Scripts.Noise
{
    public abstract class Noise : ScriptableObject
    {
        public abstract float Evaluate(float x, float y);
    }
}
