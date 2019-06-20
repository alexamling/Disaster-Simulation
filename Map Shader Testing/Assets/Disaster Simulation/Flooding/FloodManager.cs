using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : Manager
{
    public GameObject waterObject;

    public float waterLevel;

    public Material mat;

    [Header("Shader Variables")]
    #region Shader Variables
    public Color[] colors;
    public float[] colorStartHeights;
    public float maxHeight;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        output = new RenderTexture(mapWidth, mapHeight, 24);
        output.enableRandomWrite = true;
        output.Create();

        mat = gameObject.GetComponent<Renderer>().material;
        maxHeight = gameObject.GetComponent<TerrainGenerator>().scale;

        /* 
         
        mat.SetFloat("maxHeight", maxHeight);

        mat.SetInt("colorCount", colors.Length);
        mat.SetColorArray("colors", colors);
        mat.SetFloatArray("colorStartHeights", colorStartHeights);
         */
    }

    // Update is called once per frame
    void Update()
    {
        waterLevel += 10f * Mathf.Sin(Time.time * .1f) * Time.deltaTime;
        waterObject.transform.position = new Vector3(0, waterLevel * .5f, 0);
        waterObject.transform.localScale = new Vector3(mapWidth, waterLevel, mapHeight);
    }

    public override IEnumerator Load()
    {
        yield return null;
    }
}
