using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : Manager
{
    public GameObject WaterQuad;

    private float waterLevel;

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

        mat.SetFloat("maxHeight", maxHeight);

        mat.SetInt("colorCount", colors.Length);
        mat.SetColorArray("colors", colors);
        mat.SetFloatArray("colorStartHeights", colorStartHeights);
    }

    // Update is called once per frame
    void Update()
    {
        waterLevel = 2.5f * Mathf.Sin(Time.time * .1f);
        WaterQuad.transform.position += new Vector3(0, waterLevel, 0) * Time.deltaTime;
        mat.SetFloat("_WaterLevel", WaterQuad.transform.position.y);
    }

    public override IEnumerator Load()
    {
        yield return null;
    }
}
