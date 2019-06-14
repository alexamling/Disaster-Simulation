using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : Manager
{
    public GameObject WaterQuad;

    private float waterLevel;

    // Start is called before the first frame update
    void Start()
    {
        output = new RenderTexture(mapWidth, mapHeight, 24);
        output.enableRandomWrite = true;
        output.Create();
    }

    // Update is called once per frame
    void Update()
    {
        WaterQuad.transform.position += new Vector3(0, 25 * Mathf.Sin(Time.time), 0) * Time.deltaTime;
    }
}
