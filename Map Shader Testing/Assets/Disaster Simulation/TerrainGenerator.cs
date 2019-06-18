using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    private Manager manager;
    private Material material;

    public NavMeshSurface surface;

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
    void Awake()
    {
        detailModifier = (int)Mathf.Pow(2, LOD);

        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        manager = gameObject.GetComponent<Manager>();

        material = gameObject.GetComponent<Renderer>().material;

        Debug.Log("Awake");
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return StartCoroutine(manager.Load());

        Debug.Log("loaded");

        heightMap = manager.heightMap;
        texture = manager.output;

        manager.targetMat = meshRenderer.material;


        width = manager.mapWidth / detailModifier;
        height = manager.mapHeight / detailModifier;

        scaleDif = (float)manager.mapWidth / heightMap.width;

        vertecies = new Vector3[width * height];
        uvs = new Vector2[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];

        yield return StartCoroutine(GenerateMesh());
    }

    void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    IEnumerator GenerateMesh()
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
            if (y % 64 == 0)
            {
                Debug.Log("z");
                yield return null;
            }
        }
        mesh = new Mesh();
        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        surface.BuildNavMesh();

        if (gameObject.GetComponent<UnitController>())
        {
            for (int i = 0; i < 10; i++)
                gameObject.GetComponent<UnitController>().SpawnUnit();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
