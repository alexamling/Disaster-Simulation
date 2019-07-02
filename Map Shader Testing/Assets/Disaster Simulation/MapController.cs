using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for controlling all of the acitons nessicary for the map to run
/// this includes all the behind the scenes data processing and tracking as well as organizing the presentation of data
/// Writeen by Alexander Amling
/// </summary>

    /*NOTES:
     * must be attatched to a camera object 
     * requires heightmap
     * generate data maps should be relocated from firemanager to here, or to the base manager class
     */

// struct to simplify the organization of data maps
[System.Serializable]
public struct MapData
{
    public Texture2D heightMap;
    public Texture2D baseFuelMap;
    public Texture2D baseWaterMap;
    public Texture2D waterBodyMap;
    public Texture2D firePattern;
}

// struct to simplify the organization of compute shaders
[System.Serializable]
public struct ShaderCollection
{
    public ComputeShader fireTrackingShader;
    public ComputeShader floodTrackingShader;
    public ComputeShader viewMapShader;
}

// struct to hold all variables needed for debugging
[System.Serializable]
public struct DebuggingSetup
{
    public bool debugMode;
    public Material renderTarget;
    public Texture2D replacement;
}

public class MapController : MonoBehaviour
{
    public Terrain terrain;
    private TerrainData terrainData;
    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;
    [Range(1, 50)]
    public float heightScale;
    [Space(5)]
    public bool floodEnabled;
    public bool fireEnabled;
    [Space(5)]
    public MapData dataMaps;
    [Space(5)]
    public ShaderCollection shaders;
    [Space(5)]
    public Material mapMaterial;
    [Space(10)]
    public List<GameObject> objectives;
    [Space(5)]
    public AnimationCurve outlineColorCurve;
    [Space(10)]
    public List<GameObject> playerUnits;
    [Range(0, 1.1f)]
    public float duration;
    [Range(0, .005f)]
    public float startTime;

    [Header("Flood Variables")]
    public GameObject waterPrefab;
    public AnimationCurve floodCurve;
    public float baseFloodHeight;
    public float maxFloodHeight;

    [Space(5)]

    [Header("Fire Variables")]
    public ParticleSystem fireParticles;
    public ParticleSystem explosionParticles;

    [Space(15)]

    [Header("Debugging")]
    public DebuggingSetup debuggingVariables;

    // map display
    [HideInInspector]
    public TerrainGenerator terrainGenerator;

    // managers
    [HideInInspector]
    public FloodManager floodManager;
    [HideInInspector]
    public FireManager fireManager;
    [HideInInspector]
    public UnitManager unitManager;

    private Texture2D fireSnapshot;
    private Texture2D viewSnapshot;

    private new Renderer renderer;

    private ParticleSystem.ShapeModule shapeModule;

    // Adds managers and passes values to them
    void Start()
    {
        shapeModule = fireParticles.shape;
        terrainData = terrain.terrainData;

        debuggingVariables.replacement = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, false);

        fireSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, true);
        viewSnapshot = new Texture2D(mapWidth, mapHeight, TextureFormat.RGBA32, false, false);

        if (floodEnabled)
        {
            floodManager = gameObject.AddComponent<FloodManager>();

            floodManager.mapWidth = mapWidth;
            floodManager.mapHeight = mapHeight;

            floodManager.floodCurve = floodCurve;

            floodManager.baseHeight = baseFloodHeight;
            floodManager.maxHeight = maxFloodHeight;

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
        terrainGenerator.width = mapWidth;
        terrainGenerator.height = mapHeight;
        //terrainGenerator.LOD = LOD;
        terrainGenerator.heightMap = dataMaps.heightMap;
        terrainGenerator.scale = heightScale;
        terrainGenerator.terrainData = terrainData;

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
        shapeModule.mesh = terrainGenerator.mesh;
        if (fireEnabled)
            yield return StartCoroutine(fireManager.Load());
        if (floodEnabled)
            yield return StartCoroutine(floodManager.Load());

        for (int i = 0; i < 0; i++)
        {
            unitManager.SpawnUnit(playerUnits[0]);
        }

        if (fireEnabled)
        for (int i = 0; i < 25; i++)
        {
            fireManager.StartFire();
        }
    }
    
    /// <summary>
    /// Used to pass data from the fire rendertexture to a texture2D, so that it can be used as a texture for particle effect emmision
    /// </summary>
    void OnPostRender()
    {
        if (Time.frameCount % 20 == 0)
            UpdateMap();

        if (debuggingVariables.debugMode)
        {
            RenderTexture.active = fireManager.output;
            debuggingVariables.replacement.ReadPixels(new Rect(0, 0, mapWidth, mapHeight), 0, 0);
            debuggingVariables.replacement.Apply();
            RenderTexture.active = null;
            mapMaterial.SetTexture("_FireMap", debuggingVariables.replacement);
            debuggingVariables.renderTarget.mainTexture = fireManager.output;
            shapeModule.texture = debuggingVariables.replacement;
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
            shapeModule.texture = debuggingVariables.replacement;
        }

        if (renderer)
            renderer.material = mapMaterial;
        else
            renderer = gameObject.GetComponent<Renderer>();
    }

    void Explosion(Vector2 pos)
    {
        // particle effect

        //  sound effect

        // add fire
        fireManager.StartFire(pos);
        // add damage

        // add notification
    }

    void FuelSpill(Vector2 pos)
    {
        // add to fuel map

        // add notification
    }
}
