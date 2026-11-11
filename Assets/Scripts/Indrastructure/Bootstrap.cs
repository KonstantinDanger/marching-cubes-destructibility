using System.Collections;
using UnityEngine;

namespace Scripts.Infrastructure
{
    [DisallowMultipleComponent]
    public class Bootstrap : MonoBehaviour 
    {
        [SerializeField] private EntryPoint _entryPoint;

        private IEnumerator Start()
        {
            yield return _entryPoint.Initialize();
        }
    }
}
