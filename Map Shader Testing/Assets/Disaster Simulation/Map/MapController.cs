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

    [Header("Flood Variables")]
    public GameObject waterPrefab;
    public AnimationCurve floodCurve;
    public float baseFloodHeight;
    public float maxFloodHeight;
    public GameObject floodlocationRoot;
    private Transform[] floodLocations;
    [HideInInspector]
    public FloodManager floodManager;

    [Space(5)]

    [Header("Fire Variables")]
    public ParticleSystem fireParticles;
    public ParticleSystem explosionParticles;
    public GameObject fireLocationRoot;
    private Transform[] fireLocations;
    [HideInInspector]
    public FireManager fireManager;
    


    private Texture2D fireSnapshot;
    private Texture2D viewSnapshot;
    private Texture2D replacement;

    // map display
    [HideInInspector]
    public TerrainGenerator terrainGenerator;

    // managers
    [HideInInspector]
    public objectiveReader objectiveReader;


    private new Renderer renderer;

    private ParticleSystem.ShapeModule shapeModule;

    private PlayerControls playerControls;
    

    // Adds managers and passes values to them
    void Start()
    {
        shapeModule = fireParticles.shape;
        terrainData = terrain.terrainData;

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

            floodLocations = floodlocationRoot.GetComponentsInChildren<Transform>();

            // load availible data maps
            if (dataMaps.heightMap)
                floodManager.heightMap = dataMaps.heightMap;

            floodManager.waterObject = Instantiate(waterPrefab);
        }

        if (fireEnabled)
        {
            fireParticles.gameObject.SetActive(true);
            fireManager = gameObject.AddComponent<FireManager>();

            fireManager.trackingShader = shaders.fireTrackingShader;

            fireManager.fireEffect = dataMaps.firePattern;
            
            fireManager.mapWidth = mapWidth;
            fireManager.mapHeight = mapHeight;

            fireLocations = fireLocationRoot.GetComponentsInChildren<Transform>();

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
        else
        {
            fireParticles.gameObject.SetActive(false);
        }

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();
        terrainGenerator.width = mapWidth;
        terrainGenerator.height = mapHeight;
        //terrainGenerator.LOD = LOD;
        terrainGenerator.heightMap = dataMaps.heightMap;
        terrainGenerator.scale = heightScale;
        terrainGenerator.terrainData = terrainData;
        
        playerControls = FindObjectOfType<PlayerControls>();

        objectiveReader = FindObjectOfType<objectiveReader>();

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

        if (fireEnabled)
        for (int i = 0; i < 25; i++)
        {
            fireManager.StartFire();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnFloodObjective();
        }
    }

    /// <summary>
    /// Used to pass data from the fire rendertexture to a texture2D, so that it can be used as a texture for particle effect emmision
    /// </summary>
    void OnPostRender()
    {
        if (Time.frameCount % 20 == 0)
            UpdateMap();
    }

    void UpdateMap()
    {
        mapMaterial.SetTexture("_ViewMap", viewSnapshot);

        

        if (fireEnabled)
        {
            RenderTexture.active = fireManager.output;
            replacement.ReadPixels(new Rect(0, 0, mapWidth, mapHeight), 0, 0);
            replacement.Apply();
            RenderTexture.active = null;
            mapMaterial.SetTexture("_FireMap", replacement);
            shapeModule.texture = replacement;
        }

        if (renderer)
            renderer.material = mapMaterial;
        else
            renderer = gameObject.GetComponent<Renderer>();
    }

    void SpawnFloodObjective()
    {
        PlayerObjective objective = objectiveReader.floodList[Random.Range(0, objectiveReader.floodList.Count)];

        Transform placementValues = floodLocations[Random.Range(0, floodLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);
        
        Notification newNotification = Instantiate(playerControls.notificationPrefab, playerControls.notificationPanel.panel.transform);
        newNotification.text.text = objective.notificationTitle;
        newNotification.severity = 0;
        newNotification.objective = objective;
        newNotification.manager = playerControls;
        objective.notification = newNotification;
        playerControls.notifications.Add(newNotification);

        objective.objectiveState = ObjectiveState.Requesting;
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
