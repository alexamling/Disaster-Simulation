using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public Manager manager;
    public Material material;

    [Range(0,250)]
    public float scale = 35;

    public int LOD = 32;
    private int detailModifier;
    private float scaleDif;

    #region Mesh Variables
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertecies;
    private int[] triangles;
    private Vector2[] uvs;

    private int triangleIndex;

    private Texture2D heightMap;
    private int width;
    private int height;

    private RenderTexture texture;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        detailModifier = (int)Mathf.Pow(2, LOD);

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (manager)
        {
            heightMap = manager.heightMap;
            texture = manager.output;

            manager.targetMat = meshRenderer.material;


            width = manager.mapWidth / detailModifier;
            height = manager.mapHeight / detailModifier;

            scaleDif = (float)manager.mapWidth / heightMap.width;

            Debug.Log(width + " " + height + " " + scaleDif);
        }

        vertecies = new Vector3[width * height];
        uvs = new Vector2[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];

        GenerateMesh();

    }

    void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    void GenerateMesh()
    {
        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                int index = y * this.width + x;

                vertecies[index] = new Vector3(-x, heightMap.GetPixel(Mathf.FloorToInt(x * detailModifier / scaleDif), Mathf.FloorToInt(y * detailModifier / scaleDif)).r * scale + Random.Range(.0f,.01f), y);

                uvs[index] = new Vector2((float)x / this.width, (float)y / this.height);

                if (x < this.width - 1 && y < this.height - 1)
                {
                    AddTriangle(index, index + this.width + 1, index + this.width);
                    AddTriangle(index + this.width + 1, index, index + 1);
                }
            }
        }
        mesh = new Mesh();
        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
