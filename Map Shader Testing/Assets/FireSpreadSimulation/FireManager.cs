using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    [Header("Shader Data")]
    #region Shader Variables
    public ComputeShader trackingShader;
    int baseMapKernel;
    int heatMapKernel;
    int fireMapKernel;

    [Range(64, 4096)]
    public int mapWidth = 64;
    [Range(64,4096)]
    public int mapHeight = 64;
    
    [Range(0, 1)]
    public float WindStrength;
    public Vector2 WindDirection;

    public List<Vector4> fireLocations;

    private float[] locationArray;
    private int numFires = 0;

    public Texture2D fireEffect;
    public Texture2D heightMap;

    public Texture2D baseFuelMap;

    private RenderTexture fireMap;
    private RenderTexture heatMap;
    private RenderTexture fuelMap;

    public Material targetMat;
    #endregion

    [Header("Tree Data")]
    #region Countermeasure Variables
    RenderTexture Humidity;
    #endregion

    void Start()
    {
        locationArray = new float[128];
        baseFuelMap = GeneratePerlinNoise(mapWidth, mapHeight);

        fireMap = new RenderTexture(mapWidth, mapHeight, 24);
        fireMap.enableRandomWrite = true;
        fireMap.Create();

        heatMap = new RenderTexture(mapWidth, mapHeight, 24);
        heatMap.enableRandomWrite = true;
        heatMap.Create();

        fuelMap = new RenderTexture(mapWidth, mapHeight, 24);
        fuelMap.enableRandomWrite = true;
        fuelMap.Create();

        UpdateLocationArray();
        
        baseMapKernel = trackingShader.FindKernel("LoadBaseMap");
        heatMapKernel = trackingShader.FindKernel("GenerateHeatMap");
        fireMapKernel = trackingShader.FindKernel("GenerateFireMap");

        // set firemaps
        trackingShader.SetTexture(baseMapKernel, "FireMap", fireMap);
        trackingShader.SetTexture(heatMapKernel, "FireMap", fireMap);
        trackingShader.SetTexture(fireMapKernel, "FireMap", fireMap);

        // set heatmaps
        trackingShader.SetTexture(heatMapKernel, "HeatMap", heatMap);
        trackingShader.SetTexture(fireMapKernel, "HeatMap", heatMap);

        // set fire pattern texture
        trackingShader.SetTexture(baseMapKernel, "FirePattern", fireEffect);

        // set fuelmap
        trackingShader.SetTexture(baseMapKernel, "BaseFuelMap", baseFuelMap);
        trackingShader.SetTexture(baseMapKernel, "FuelMap", fuelMap);
        trackingShader.SetTexture(heatMapKernel, "FuelMap", fuelMap);
        trackingShader.SetTexture(fireMapKernel, "FuelMap", fuelMap);

        trackingShader.Dispatch(baseMapKernel, mapWidth / 8, mapHeight / 8, 1);
    }
    
    void Update()
    {
        trackingShader.SetFloat("WindStrength", WindStrength);
        trackingShader.SetFloats("WindOffset", new float[] { WindDirection.x, WindDirection.y });

        trackingShader.Dispatch(heatMapKernel, mapWidth / 8, mapHeight / 8, 1); // run the compute shader
        trackingShader.Dispatch(fireMapKernel, mapWidth / 8, mapHeight / 8, 1); 

        targetMat.SetTexture("_MainTex", fireMap); // assign the render texture to the material
    }

    /// <summary>
    /// Update the values that are passed into the comput shader
    /// </summary>
    void UpdateLocationArray()
    {
        if(true) //fireLocations.Count != numFires)
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
    }
    
    /// <summary>
    /// Used to Generate a placeholder foliage map
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="octaves"></param>
    /// <returns></returns>
    Texture2D GeneratePerlinNoise(int width, int height, int octaves = 3)
    {
        float offsetX = Random.Range(0, 100f);
        float offsetY = Random.Range(0, 100f);

        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];

        float maxValue = 0;

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
            }
        }

        // normalization loop
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = Mathf.InverseLerp(-.5f, maxValue, colors[y * width + x].r);

                colors[y * width + x] = new Color(value, value, value, 0);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
}

