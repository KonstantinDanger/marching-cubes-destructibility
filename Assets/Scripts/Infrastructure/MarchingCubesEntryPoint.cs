using Assets.Scripts.Infrastructure;
using Scripts.MarchingCubes;
using Scripts.Noise;
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
        [SerializeField] private Material _baseMaterial;

        public override IEnumerator Initialize()
        {
            DensityMapGenerator mapGenerator = new(_mapSize, _noise, _generationData);
            float[,,] densityMap = mapGenerator.GetMap();

            MarchingCubesMeshGenerator meshGenerator = new(_mapSize, _baseMaterial);

            meshGenerator.Generate(densityMap);

            yield return VisualizeDensityMap(densityMap);
        }

        private IEnumerator VisualizeDensityMap(float[,,] densityMap)
        {
            if (densityMap.Length == 0)
                yield break;

            GameObject densityParentObject = new ("Density holder");
            densityParentObject.transform.parent = _gridParent;

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

                color = new(density, density, density, 0.9f);

                var go = Helpers.CreateDummyPointAt(position, 0.2f, color, _gridParent, $"Density point, [{density}]");
                go.transform.parent = densityParentObject.transform;
            }

            yield return null;
        }
    }
}
