using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapData
{
    public Texture2D heightMap;
    public Texture2D baseFuelMap;
    public Texture2D baseWaterMap;
    public Texture2D waterBodyMap;
}

public class MapController : MonoBehaviour
{
    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;
    [Range(1,10)]
    public int LOD;
    [Space(5)]
    public bool floodEnabled;
    public bool fireEnabled;
    [Space(5)]
    public MapData dataMaps;
    [Space(5)]
    public GameObject waterQuadPrefab;

    // map display
    private TerrainGenerator terrainGenerator;
    // managers
    private FloodManager floodManager;
    private FireManager fireManager;
    private UnitController unitController;

    // Map Compute Shader
    // Shader Kernel
    // Map Surface Shader

    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        if (floodEnabled)
        {
            floodManager = gameObject.AddComponent<FloodManager>();

            floodManager.mapWidth = mapWidth;
            floodManager.mapHeight = mapHeight;

            // load availible data maps
            if (dataMaps.heightMap)
                floodManager.heightMap = dataMaps.heightMap;

            floodManager.waterQuad = Instantiate(waterQuadPrefab);
        }

        if (fireEnabled)
        {
            fireManager = gameObject.AddComponent<FireManager>();

            fireManager.mapWidth = mapWidth;
            fireManager.mapHeight = mapHeight;

            // load availible data maps
            if (dataMaps.heightMap)
                fireManager.heightMap = dataMaps.heightMap;
            if (dataMaps.baseFuelMap)
                fireManager.heightMap = dataMaps.baseFuelMap;
            if (dataMaps.baseWaterMap)
                fireManager.heightMap = dataMaps.baseWaterMap;
            if (dataMaps.waterBodyMap)
                fireManager.heightMap = dataMaps.waterBodyMap;
        }

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();
        terrainGenerator.mapController = this;
        terrainGenerator.LOD = LOD;
        terrainGenerator.heightMap = dataMaps.heightMap;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return StartCoroutine(terrainGenerator.Load());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
