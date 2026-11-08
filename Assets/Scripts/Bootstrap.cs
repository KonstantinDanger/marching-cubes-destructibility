using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private Transform _mapPivot;
    [SerializeField] private MapSize _mapSize;
    [SerializeField] private Image _heightMapImage;
    [SerializeField] private MapGenerationData _generationData;

    private List<GameObject> _gridPreview = new();
    private Texture2D _texture;

    private void Start()
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

        HeightmapGenerator heightmapGenerator = new(_mapSize, _mapPivot.position, _generationData);
        MeshGenerator meshGenerator = new(_mapPivot.gameObject);

        float[,] heightMap = heightmapGenerator.GetMap(centerAroundStartingPoint: true);

        _texture = GetHeightMapImage(heightMap);

        _heightMapImage.material.mainTexture = _texture;
        _heightMapImage.SetMaterialDirty();

        SpawnDummyPointForHeightMap(heightMap);
        meshGenerator.Generate(heightMap);
    }

    private void SpawnDummyPointForHeightMap(float[,] heightMap)
    {
        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++)
            {
                float y = heightMap[x, z];

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = $"Dummy vertice [{x}, {y}, {z}]";
                Color color = Color.red;
                color.a = 0.3f;
                go.GetComponent<MeshRenderer>().material.color = color;
                go.transform.localScale /= 5;
                go.transform.position = new(x,y,z);
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
