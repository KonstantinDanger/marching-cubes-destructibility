using UnityEngine;

namespace Scripts.Infrastructure
{
    [DisallowMultipleComponent]
    public abstract class EntryPoint : MonoBehaviour
    {
        public abstract void Initialize();
    }
}
