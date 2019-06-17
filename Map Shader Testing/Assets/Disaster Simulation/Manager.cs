using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{

    [Range(64, 8192)]
    public int mapWidth = 4096;
    [Range(64, 8192)]
    public int mapHeight = 4096;

    public Texture2D heightMap;

    public RenderTexture output;

    public Material targetMat;

    public bool isLoaded;

    public abstract IEnumerator Load();
}
