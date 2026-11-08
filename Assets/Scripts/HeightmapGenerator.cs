using UnityEngine;

public class HeightmapGenerator
{
    private readonly MapSize _mapSize;
    private readonly MapGenerationData _generationData;
    private readonly Noise _noise;

    public HeightmapGenerator(MapSize maxSize, MapGenerationData generationData, Noise noise)
    {
        _mapSize = maxSize;
        _generationData = generationData;
        _noise = noise;
    }

    public float[,] GetMap()
    {
        int width = _mapSize.Width + 1;
        int depth = _mapSize.Depth + 1;

        float[,] heightMap = new float[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float xNorm = (float)x / width * _generationData.NoiseFrequency; 
                float zNorm = (float)z / depth * _generationData.NoiseFrequency; 

                float y = Mathf.Clamp01(_noise.Evaluate(xNorm, zNorm)) * _mapSize.MaxHeight;

                heightMap[x, z] = y;
            }
        }

        return heightMap;
    }
}
