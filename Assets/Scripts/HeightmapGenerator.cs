using Unity.VisualScripting;
using UnityEngine;

public class HeightmapGenerator
{
    private readonly MapSize _mapSize;
    private readonly MapGenerationData _generationData;
    private readonly Vector3 _startingPoint;

    public HeightmapGenerator(MapSize maxSize, Vector3 startingPoint, MapGenerationData generationData)
    {
        _mapSize = maxSize;
        _startingPoint = startingPoint;
        _generationData = generationData;
    }

    public float[,] GetMap(bool centerAroundStartingPoint = true)
    {
        float[,] heightMap = new float[_mapSize.Width, _mapSize.Length];

        for (int x = 0; x < _mapSize.Width; x++)
        {
            for (int z = 0; z < _mapSize.Length; z++)
            {
                float xNorm = (float)x / _mapSize.Width * _generationData.HeightFrequency; 
                float zNorm = (float)z / _mapSize.Length * _generationData.HeightFrequency; 

                float y = Mathf.Clamp01(Mathf.PerlinNoise(xNorm, zNorm)) * _mapSize.MaxHeight;

                heightMap[x, z] = y;
            }
        }

        return heightMap;
    }
}
