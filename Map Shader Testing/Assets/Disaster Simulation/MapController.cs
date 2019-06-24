﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapData
{
    public Texture2D heightMap;
    public Texture2D baseFuelMap;
    public Texture2D baseWaterMap;
    public Texture2D waterBodyMap;
    public Texture2D firePattern;
}

[System.Serializable]
public struct ShaderCollection
{
    public ComputeShader fireTrackingShader;
    public ComputeShader floodTrackingShader;
    public ComputeShader viewMapShader;
}

[System.Serializable]
public struct DebuggingSetup
{
    public bool debugMode;
    public Material renderTarget;
    public Texture2D replacement;
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
    [Range(0, 1.1f)]
    public float duration;
    [Range(0, .005f)]
    public float startTime;

    [Space(15)]
    public ParticleSystem fireParticles;

    [Header("Debugging")]
    public DebuggingSetup debuggingVariables;

    // map display
    private TerrainGenerator terrainGenerator;
    // managers
    private FloodManager floodManager;
    private FireManager fireManager;
    private UnitManager unitManager;

    private Texture2D fireSnapshot;
    private Texture2D viewSnapshot;

    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        fireSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, true);
        viewSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, false);

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
            fireManager = gameObject.AddComponent<FireManager>();

            fireManager.trackingShader = shaders.fireTrackingShader;

            fireManager.fireEffect = dataMaps.firePattern;
            
            fireManager.mapWidth = mapWidth;
            fireManager.mapHeight = mapHeight;

            // load availible data maps
            if (dataMaps.heightMap)
                fireManager.heightMap = dataMaps.heightMap;
            if (dataMaps.baseFuelMap)
                fireManager.baseFuelMap = dataMaps.baseFuelMap;
            if (dataMaps.baseWaterMap)
                fireManager.baseWaterMap = dataMaps.baseWaterMap;
            if (dataMaps.waterBodyMap)
                fireManager.waterBodyMap = dataMaps.waterBodyMap;
        }

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();
        terrainGenerator.mapController = this;
        terrainGenerator.LOD = LOD;
        terrainGenerator.heightMap = dataMaps.heightMap;
        terrainGenerator.scale = 200;

        unitManager = gameObject.AddComponent<UnitManager>();
        unitManager.mapWidth = mapWidth;
        unitManager.mapHeight = mapHeight;
        unitManager.heightMap = dataMaps.heightMap;
        unitManager.viewMapShader = shaders.viewMapShader;

        StartCoroutine(Load());
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
            unitManager.SpawnUnit(playerUnits[0]);
        }

        if (fireEnabled)
        for (int i = 0; i < 25; i++)
        {
            fireManager.StartFire();
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
        if (Time.frameCount % 360 == 0)
        {
            UpdateMap();
        }
        if (debuggingVariables.debugMode)
        {
            RenderTexture.active = fireManager.output;
            debuggingVariables.replacement.ReadPixels(new Rect(0, 0, mapWidth, mapHeight), 0, 0);
            debuggingVariables.replacement.Apply();
            RenderTexture.active = null;
            mapMaterial.SetTexture("_FireMap", debuggingVariables.replacement);
            debuggingVariables.renderTarget.mainTexture = fireManager.output;
            var shape = fireParticles.shape;
            shape.texture = debuggingVariables.replacement;
        }
    }

    void UpdateMap()
    {

        unitManager.viewMapShader.SetFloat("duration", duration);
        unitManager.viewMapShader.SetFloat("startTime", startTime);


        Graphics.CopyTexture(unitManager.output, viewSnapshot);
        mapMaterial.SetTexture("_ViewMap", viewSnapshot);

        if (fireEnabled)
        {
            RenderTexture.active = fireManager.output;
            debuggingVariables.replacement.ReadPixels(new Rect(0, 0, mapWidth, mapHeight), 0, 0);
            debuggingVariables.replacement.Apply();
            RenderTexture.active = null;
            mapMaterial.SetTexture("_FireMap", debuggingVariables.replacement);
            debuggingVariables.renderTarget.mainTexture = fireManager.output;
            var shape = fireParticles.shape;
            shape.texture = debuggingVariables.replacement;
        }

        if (renderer)
            renderer.material = mapMaterial;
        else
            renderer = gameObject.GetComponent<Renderer>();
    }
}
