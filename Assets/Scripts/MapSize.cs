using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Map Size")]
public class MapSize : ScriptableObject
{
    [field: Header("World")]
    [field: SerializeField, Min(0f)] public int Width { get; private set; }
    [field: SerializeField, Min(0f)] public int Depth { get; private set; }
    [field: SerializeField, Min(0f)] public int MaxHeight { get; private set; }

    [field: Header("Chunk")]
    [field: SerializeField, Min(0f)] public int ChunkWidth { get; private set; }
    [field: SerializeField, Min(0f)] public int ChunkDepth { get; private set; }

    private void OnValidate()
    {
        if (!IsPowerOfTwo(Width))
            Width = NearestPowerOf2(Width);

        if (!IsPowerOfTwo(Depth))
            Depth = NearestPowerOf2(Depth);

        if (!IsPowerOfTwo(MaxHeight))
            MaxHeight = NearestPowerOf2(MaxHeight);

        if (!IsPowerOfTwo(ChunkWidth)) 
            ChunkWidth = NearestPowerOf2(ChunkWidth);

        if (!IsPowerOfTwo(ChunkDepth))
            ChunkDepth = NearestPowerOf2(ChunkDepth);

        ChunkDepth = Mathf.Clamp(ChunkDepth, 2, Depth);
        ChunkWidth = Mathf.Clamp(ChunkWidth, 2, Width);
    }

    private bool IsPowerOfTwo(int num) 
        => num != 0 && (num & (num - 1)) == 0;

    private static int NearestPowerOf2(int n)
    {
        if (n <= 0) return 1;

        int lower = (int)Math.Pow(2, Math.Floor(Math.Log(n, 2)));
        int upper = lower * 2;

        return (n - lower < upper - n) ? lower : upper;
    }
}
