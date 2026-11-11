using UnityEngine;

namespace Scripts.Infrastructure
{
    [DisallowMultipleComponent]
    public class Bootstrap : MonoBehaviour 
    {
        [SerializeField] private EntryPoint _entryPoint;

        private void Start()
        {
            _entryPoint.Initialize();
        }
    }
}
