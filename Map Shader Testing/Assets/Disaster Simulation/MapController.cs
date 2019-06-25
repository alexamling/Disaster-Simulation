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

    private ParticleSystem.ShapeModule shapeModule;

    // Start is called before the first frame update
    void Start()
    {
        shapeModule = fireParticles.shape;

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
        shapeModule.mesh = terrainGenerator.mesh;
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
}
