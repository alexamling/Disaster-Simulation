using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public FireManager fireManager;
    public Material material;

    [Range(0,250)]
    public float scale;

    public int LOD = 4;
    private int scaleDif;

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
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        if (fireManager)
        {
            heightMap = fireManager.heightMap;
            texture = fireManager.fireMap;

            fireManager.targetMat = meshRenderer.material;

            scaleDif = fireManager.mapWidth / heightMap.width;

            width = fireManager.mapWidth / LOD;
            height = fireManager.mapHeight / LOD;

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
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                vertecies[index] = new Vector3(-x, heightMap.GetPixel(x * LOD / scaleDif, y * LOD / scaleDif).r * scale, y);

                uvs[index] = new Vector2((float)x / width, (float)y / height);

                if (x < width - 1 && y < height - 1)
                {
                    AddTriangle(index, index + width + 1, index + width);
                    AddTriangle(index + width + 1, index, index + 1);
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
