using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MapType { HeightMap, FuelMap, WaterMap};

public class FireManager : Manager
{
    [Header("Shader Data")]
    #region Shader Variables
    public ComputeShader trackingShader;
    Renderer renderer;
    int baseMapKernel;
    int heatMapKernel;
    int fireMapKernel;
    
    [Range(0, 1)]
    public float WindStrength = .15f;
    public Vector2 WindDirection = new Vector2(1,2);

    public List<Vector4> fireLocations;

    private float[] locationArray;
    private int numFires = 0;

    public Texture2D fireEffect;
    #endregion


    [Header("Data Maps")]
    #region Data Map Variables
    public Texture2D baseFuelMap;
    public Texture2D baseWaterMap;
    public Texture2D waterBodyMap;
    
    private RenderTexture heatMap;
    private RenderTexture fuelMap;
    public RenderTexture waterMap;
    #endregion

    int texturesLoading = 0;

    void Start()
    {
    }
    
    public override IEnumerator Load()
    {
        renderer = gameObject.GetComponent<Renderer>();

        texturesLoading = 1;

        locationArray = new float[128];
        fireLocations = new List<Vector4>();

        output = new RenderTexture(mapWidth, mapHeight, 24);
        output.enableRandomWrite = true;
        output.Create();

        heatMap = new RenderTexture(mapWidth, mapHeight, 24);
        heatMap.enableRandomWrite = true;
        heatMap.Create();

        fuelMap = new RenderTexture(mapWidth, mapHeight, 24);
        fuelMap.enableRandomWrite = true;
        fuelMap.Create();

        waterMap = new RenderTexture(mapWidth, mapHeight, 24);
        waterMap.enableRandomWrite = true;
        waterMap.Create();

        UpdateLocationArray();

        texturesLoading = 0;

        // populate the data maps with provided data, or generate random data
        if (!heightMap)
        {
            texturesLoading++;
            Debug.Log(texturesLoading);
            yield return StartCoroutine(GeneratePerlinNoise(mapWidth, mapHeight, MapType.HeightMap));
        }
        
        if (!baseFuelMap)
        {
            texturesLoading++;
            Debug.Log(texturesLoading);
            yield return StartCoroutine(GeneratePerlinNoise(mapWidth, mapHeight, MapType.FuelMap));
        }

        Graphics.Blit(baseFuelMap, fuelMap);

        if (!baseWaterMap)
        {
            texturesLoading++;
            Debug.Log(texturesLoading);
            yield return StartCoroutine(GeneratePerlinNoise(mapWidth, mapHeight, MapType.WaterMap));
        }
        
        Graphics.Blit(baseWaterMap, waterMap);

        while (texturesLoading > 0)
            yield return null;

        // load shader kernels
        baseMapKernel = trackingShader.FindKernel("LoadBaseMap");
        heatMapKernel = trackingShader.FindKernel("GenerateHeatMap");
        fireMapKernel = trackingShader.FindKernel("GenerateFireMap");
        
        // set firemaps
        trackingShader.SetTexture(baseMapKernel, "FireMap", output);
        trackingShader.SetTexture(heatMapKernel, "FireMap", output);
        trackingShader.SetTexture(fireMapKernel, "FireMap", output);

        // set heatmaps
        trackingShader.SetTexture(heatMapKernel, "HeatMap", heatMap);
        trackingShader.SetTexture(fireMapKernel, "HeatMap", heatMap);

        // set fire pattern texture
        trackingShader.SetTexture(baseMapKernel, "FirePattern", fireEffect);

        // set fuelmaps
        trackingShader.SetTexture(heatMapKernel, "FuelMap", fuelMap);
        trackingShader.SetTexture(fireMapKernel, "FuelMap", fuelMap);

        // set heightmap
        trackingShader.SetTexture(heatMapKernel, "HeightMap", heightMap);

        // set watermap
        trackingShader.SetTexture(baseMapKernel, "WaterBodyMap", waterBodyMap);
        trackingShader.SetFloat("waterBodyScale", (float)waterBodyMap.width / heightMap.width);
        trackingShader.SetTexture(baseMapKernel, "WaterMap", waterMap);
        trackingShader.SetTexture(heatMapKernel, "WaterMap", waterMap);

        // run the shader basemap setup
        trackingShader.Dispatch(baseMapKernel, mapWidth / 8, mapHeight / 8, 1);

        isLoaded = true;
    }

    void Update()
    {
        if (!isLoaded)
            return;

        trackingShader.SetFloat("WindStrength", WindStrength);
        trackingShader.SetFloats("WindOffset", new float[] { WindDirection.x, WindDirection.y });

        trackingShader.Dispatch(heatMapKernel, mapWidth / 8, mapHeight / 8, 1); // update the heatmap
        trackingShader.Dispatch(fireMapKernel, mapWidth / 8, mapHeight / 8, 1); // update the firemap
    }

    /// <summary>
    /// Update the values that are passed into the comput shader
    /// </summary>
    void UpdateLocationArray()
    {
        numFires = fireLocations.Count;

        for (int i = 0; i < numFires; i++)
        {
            locationArray[i * 4] = Mathf.FloorToInt(fireLocations[i].x);        // x pos
            locationArray[i * 4 + 1] = Mathf.FloorToInt(fireLocations[i].y);    // y pos
            locationArray[i * 4 + 2] = 1 / fireLocations[i].z;                  // scale
            locationArray[i * 4 + 3] = fireLocations[i].w;                      // intensity
        }

        trackingShader.SetFloats("FireData", locationArray);
        trackingShader.SetInt("NumFires", numFires);
    }

    /// <summary>
    /// Used to generate placeholder data maps
    /// </summary>
    /// <param name="width">width of texture</param>
    /// <param name="height">height of texture</param>
    /// <param name="octaves">number of times to pass over the texture with perlin noise</param>
    /// <returns>a grayscale texture with perlin noise values</returns>
    IEnumerator GeneratePerlinNoise(int width, int height, MapType type, int octaves = 3)
    {
        float offsetX = Random.Range(0, 100f);
        float offsetY = Random.Range(0, 100f);

        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        float maxValue = 0;
        float minValue = 10000000;

        // noise loop
        for (int y = 0; y < height; y++)
        {
            for (int x = 0;x < width; x++)
            {
                float value = 0;
                for (int o = 1; o <= octaves; o++)
                {
                    value += Mathf.PerlinNoise((x * Mathf.Pow(.05f, o + .0f)) + offsetX, (y * Mathf.Pow(.05f, o + .0f)) + offsetY) * o;
                }
                colors[y * width + x] =  new Color(value,value,value,0);

                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;

            }
            if (y % 16 == 0)
            {
                yield return null;
            }
        } 

        // normalization loop
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = Mathf.InverseLerp(minValue, maxValue, colors[y * width + x].r);

                colors[y * width + x] = new Color(value, value, value, 0);

            }
            if (y % 128 == 0)
            {
                yield return null;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        if (type == MapType.HeightMap)
            heightMap = texture;
        else if (type == MapType.FuelMap)
            baseFuelMap = texture;
        else if (type == MapType.WaterMap)
            baseWaterMap = texture;

        texturesLoading--;
    }
}

