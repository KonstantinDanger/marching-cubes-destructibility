using UnityEngine;

[CreateAssetMenu(menuName = "Map Generation Data")]
public class MapGenerationData : ScriptableObject {

    [field: SerializeField, Min(0f)] public float NoiseFrequency { get; private set; }
}
