using Assets.Scripts.Indrastructure;
using Scripts.MarchingCubes;
using Scripts.Noise;
using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Infrastructure
{
    [DisallowMultipleComponent]
    public class MarchingCubesEntryPoint : EntryPoint
    {
        [SerializeField] private Transform _gridParent;
        [SerializeField] private MapSize3D _mapSize;
        [SerializeField] private MapGenerationData _generationData;
        [SerializeField] private Noise3D _noise;
        [SerializeField] private float _visualizationTime;

        public override IEnumerator Initialize()
        {
            DensityMapGenerator mapGenerator = new(_mapSize, _noise, _generationData);
            float[,,] densityMap = mapGenerator.GetMap();

            yield return VisualizeDensityMap(densityMap);
        }

        private IEnumerator VisualizeDensityMap(float[,,] densityMap)
        {
            if (densityMap.Length == 0)
                yield break;

            float waitFor = _visualizationTime / densityMap.Length;
            Vector3 position;
            Color color;
            int height = densityMap.GetLength(1);
            int depth = densityMap.GetLength(2);

            for (int i = 0; i < densityMap.Length; i++)
            {
                int z = i % depth;
                int y = (i / depth) % height;
                int x = i / (height * depth);

                position = new(x, y, z);

                float density = densityMap[x, y, z];

                color = new(1 - density, 1 - density, 1 - density, 0.5f);

                Helpers.CreateDummyPointAt(position, 0.2f, color, _gridParent, $"Density point, [{density}]");

                yield return new WaitForSeconds(waitFor);
            }

            yield return null;
        }
    }
}
