using Scripts.Heightmap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Scripts.Infrastructure
{
    [DisallowMultipleComponent]
    public class HeightmapEntryPoint : EntryPoint
    {
        [SerializeField] private Transform _mapPivot;
        [SerializeField] private MapSize _mapSize;
        [SerializeField] private Image _heightMapImage;
        [SerializeField] private MapGenerationData _generationData;
        [SerializeField] private Noise.Noise _noise;
        [SerializeField] private Material _baseGroundMaterial;

        private List<GameObject> _gridPreview = new();
        private Texture2D _texture;

        public override void Initialize()
        {
            InitializeGeneration();
        }

        private void Update()
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
                InitializeGeneration();
        }

        private void InitializeGeneration()
        {
            foreach (var item in _gridPreview)
            {
                Destroy(item);
            }

            HeightmapGenerator heightmapGenerator = new(_mapSize, _generationData, _noise);
            HeightMapMeshGenerator meshGenerator = new(_mapPivot.gameObject, _mapSize, _baseGroundMaterial);

            float[,] heightMap = heightmapGenerator.GetMap();

            _texture = GetHeightMapImage(heightMap);

            _heightMapImage.material.mainTexture = _texture;
            _heightMapImage.SetMaterialDirty();

            SpawnDummyPointsForHeightMap(heightMap);
            meshGenerator.Generate(heightMap);
        }

        private void SpawnDummyPointsForHeightMap(float[,] heightMap)
        {
            GameObject holder = new("Points holder");
            holder.transform.parent = _mapPivot;

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int z = 0; z < heightMap.GetLength(1); z++)
                {
                    float y = heightMap[x, z];

                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    go.name = $"Dummy vertice [{x}, {y}, {z}]";
                    go.transform.parent = holder.transform;
                    Color color = Color.red;
                    color.a = 0.3f;
                    go.GetComponent<MeshRenderer>().material.color = color;
                    go.transform.localScale /= 5;
                    go.transform.position = new(x,y,z);

                    if(go.TryGetComponent(out Collider col))
                        Destroy(col);

                    if (go.TryGetComponent(out MeshRenderer renderer))
                    {
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        renderer.receiveShadows = false;
                    }

                    _gridPreview.Add(go);
                }
            }
        }

        private Texture2D GetHeightMapImage(float[,] heightMap)
        {
            Texture2D texture = new (heightMap.GetLength(0), heightMap.GetLength(1));

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int z = 0; z < heightMap.GetLength(1); z++)
                {
                    float y = heightMap[x,z] / (float)_mapSize.MaxHeight;
                    Color color = new (y, y, y, 1f);
                    texture.SetPixel(x, z, color);
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }
    }
}
