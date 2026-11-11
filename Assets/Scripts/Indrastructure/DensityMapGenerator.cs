using Scripts.Noise;
using System;
using UnityEngine;

namespace Scripts.MarchingCubes
{
    public class DensityMapGenerator
    {
        private readonly MapSize3D _mapSize;
        private readonly Noise3D _noise;
        private readonly MapGenerationData _generationData;

        public DensityMapGenerator(MapSize3D mapSize, Noise3D noise, MapGenerationData generationData)
        {
            _mapSize = mapSize;
            _noise = noise;
            _generationData = generationData;
        }

        public float[,,] GetMap()
        {
            int width = _mapSize.Width + 1;
            int height = _mapSize.Height + 1;
            int depth = _mapSize.Depth + 1;

            float[,,] densityMap = new float[width, height, depth];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float xNorm = (float)x / width * _generationData.NoiseFrequency;
                        float yNorm = (float)y / height * _generationData.NoiseFrequency;
                        float zNorm = (float)z / depth * _generationData.NoiseFrequency;

                        float noiseValue = _noise.Evaluate(xNorm * 0.99f, yNorm * 0.99f, zNorm * 0.99f);
                        float density = noiseValue > 0.5f ? 1 : 0;
                        densityMap[x, y, z] = density;
                    }
                }
            }

            return densityMap;
        }
    }
}
