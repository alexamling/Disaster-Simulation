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

public struct ShaderCollection
{
    public ComputeShader fireTrackingShader;
    public ComputeShader floodTrackingShader;
}

public class MapController : MonoBehaviour
{
    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;
    [Range(0,32)]
    public int LOD;
    [Space(5)]
    public bool floodEnabled;
    public bool fireEnabled;
    [Space(5)]
    public MapData dataMaps;
    [Space(5)]
    public ShaderCollection shaders;
    [Space(5)]
    public GameObject waterPrefab;
    [Space(5)]
    public Material mapMaterial;
    [Space(10)]
    public List<GameObject> BuildingsOfInterest;
    [Space(5)]
    public AnimationCurve OutlineColorCurve;
    [Space(10)]
    public List<GameObject> playerUnits;

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

            floodManager.waterObject = Instantiate(waterPrefab);
        }

        if (fireEnabled)
        {
            fireManager.trackingShader = shaders.fireTrackingShader;

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
        terrainGenerator.scale = 200;
        StartCoroutine(Load());

        unitController = gameObject.AddComponent<UnitController>();
    }

    IEnumerator Load()
    {
        yield return StartCoroutine(terrainGenerator.Load());
        gameObject.GetComponent<Renderer>().material = mapMaterial;
        if (fireEnabled)
            yield return StartCoroutine(fireManager.Load());
        if (floodEnabled)
            yield return StartCoroutine(floodManager.Load());

        for (int i = 0; i < 10; i++)
        {
            unitController.SpawnUnit(playerUnits[0]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (floodEnabled)
        {
            float mix;
            float value;
            for (int i = 0; i < BuildingsOfInterest.Count; i++)
            {
                if (BuildingsOfInterest[i].GetComponent<PlayerObjective>().status <= 0.001f)
                    continue;

                value = Mathf.InverseLerp(0,150,BuildingsOfInterest[i].transform.position.y - floodManager.waterLevel);
                
                BuildingsOfInterest[i].GetComponent<PlayerObjective>().status = Mathf.Clamp(value, 0, 1);

                if(value > .5)
                {
                    mix = Mathf.InverseLerp(.5f, 1, value);
                    BuildingsOfInterest[i].GetComponent<Outline>().OutlineColor = mix * Color.green + (1 - mix) * Color.yellow;
                }
                else if (value > .25)
                {
                    mix = Mathf.InverseLerp(.25f, .5f, value);
                    BuildingsOfInterest[i].GetComponent<Outline>().OutlineColor = mix * Color.yellow + (1 - mix) * new Color(1, .5f, 0);
                }
                else
                {
                    mix = Mathf.InverseLerp(.0f, .25f, value);
                    BuildingsOfInterest[i].GetComponent<Outline>().OutlineColor = mix * new Color(1, .5f, 0) + (1 - mix) * Color.red;
                }
            }
        }
    }
}
