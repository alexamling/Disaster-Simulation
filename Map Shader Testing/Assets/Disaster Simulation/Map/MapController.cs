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
    public GameObject floodLocationRoot;
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

    [Space(5)]

    [Header("Evacuation Variables")]
    public GameObject evacLocationRoot;
    private Transform[] evacLocations;

    [Space(5)]

    [Header("PersonalIncidents Variables")]
    public GameObject personalLocationRoot;
    private Transform[] personalLocations;

    [Space(5)]

    [Header("Accident Variables")]
    public GameObject accidentLocationRoot;
    private Transform[] accidentLocations;



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

    [Header("UI Variables")]
    public GameObject iconRoot;

    [HideInInspector]
    public float score;

    // Adds managers and passes values to them
    void Start()
    {
        //initialize locations
        fireLocations = fireLocationRoot.GetComponentsInChildren<Transform>();
        accidentLocations = accidentLocationRoot.GetComponentsInChildren<Transform>();
        evacLocations = evacLocationRoot.GetComponentsInChildren<Transform>();
        personalLocations = personalLocationRoot.GetComponentsInChildren<Transform>();

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

            floodLocations = floodLocationRoot.GetComponentsInChildren<Transform>();

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

            //fireLocations = fireLocationRoot.GetComponentsInChildren<Transform>();

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
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnFloodObjective();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnFireObjective();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnEvacObjective();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnAccidentObjective();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SpawnPersonalObjective();
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
        PlayerObjective objective = Instantiate(objectiveReader.floodList[Random.Range(0, objectiveReader.floodList.Count)]);

        Transform placementValues = floodLocations[Random.Range(0, floodLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnFireObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.fireList[Random.Range(0, objectiveReader.fireList.Count)]);
        Transform placementValues = fireLocations[Random.Range(0, fireLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnEvacObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.evacList[Random.Range(0, objectiveReader.evacList.Count)]);
        Transform placementValues = evacLocations[Random.Range(0, evacLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnAccidentObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.accidentList[Random.Range(0, objectiveReader.accidentList.Count)]);
        Transform placementValues = accidentLocations[Random.Range(0, accidentLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    void SpawnPersonalObjective()
    {
        PlayerObjective objective = Instantiate(objectiveReader.personalList[Random.Range(0, objectiveReader.personalList.Count)]);
        Transform placementValues = personalLocations[Random.Range(0, personalLocations.Length)].transform;

        Vector2 pos = Random.insideUnitCircle * placementValues.localScale.x;

        objective.transform.position = new Vector3(pos.x + placementValues.position.x, 0, pos.y + placementValues.position.z);

        SpawnObjective(objective);
    }

    private void SpawnObjective(PlayerObjective objective)
    {

        Notification newNotification = Instantiate(playerControls.notificationPrefab, playerControls.notificationPanel.panel.transform);
        newNotification.text.text = objective.notificationTitle;
        newNotification.severity = 0;
        newNotification.objective = objective;
        newNotification.manager = playerControls;
        objective.notification = newNotification;
        objective.iconRoot = iconRoot;
        playerControls.notifications.Add(newNotification);

        objective.objectiveState = ObjectiveState.Requesting;
    }
}
