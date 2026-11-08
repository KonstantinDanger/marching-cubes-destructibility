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

        int offsetX = 0 ;
        int offsetZ = 0;

        if (centerAroundStartingPoint)
        {
            Mathf.RoundToInt(offsetX /= 2);
            Mathf.RoundToInt(offsetZ /= 2);
        }

        for (int x = offsetX; x < _mapSize.Width + offsetX; x++)
        {
            for (int z = offsetZ; z < _mapSize.Length + offsetZ; z++)
            {
                float xNorm = (float)x / _mapSize.Width * _generationData.HeightFrequency; 
                float zNorm = (float)z / _mapSize.Length * _generationData.HeightFrequency; 

                float y = Mathf.Clamp01(Mathf.PerlinNoise(xNorm, zNorm)) * _mapSize.MaxHeight;

                UnityEngine.Debug.Log("y " + y);
                heightMap[x, z] = y;
            }
        }

        return heightMap;
    }
}
