using UnityEngine;

[CreateAssetMenu(menuName = "Map Size/MapSize_3D")]
public sealed class MapSize3D : MapSize2D
{
    [field: SerializeField, Min(0f)] public int ChunkHeight { get; private set; }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!IsPowerOfTwo(ChunkHeight))
            ChunkHeight = NearestPowerOf2(ChunkHeight);

        ChunkHeight = Mathf.Clamp(ChunkHeight, 2, Height);
    }
}
