using UnityEngine;

[CreateAssetMenu(menuName = "Map Size")]
public class MapSize : ScriptableObject
{
    [field: SerializeField, Min(0f)] public int Width { get; private set; }
    [field: SerializeField, Min(0f)] public int Length { get; private set; }
    [field: SerializeField, Min(0f)] public int MaxHeight { get; private set; }
}
