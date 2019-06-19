using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainGenerator : MonoBehaviour
{
    public MapController mapController;

    private NavMeshSurface surface;

    [Range(0,250)]
    public float scale = 35;

    public int LOD = 32;
    private int spaceBetweenPoints = 1;
    private float worldToMapScale;

    #region Mesh Variables
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;

    private Vector3[] vertecies;
    private int[] triangles;
    private Vector2[] uvs;

    private int triangleIndex;

    public Texture2D heightMap;
    private int gridWidth;
    private int gridHeight;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        surface = gameObject.AddComponent<NavMeshSurface>();

        spaceBetweenPoints = (int)Mathf.Pow(2, LOD);

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        collider = gameObject.AddComponent<MeshCollider>();
    }

    public IEnumerator Load()
    {
        gridWidth = mapController.mapWidth / spaceBetweenPoints;
        gridHeight = mapController.mapHeight / spaceBetweenPoints;

        worldToMapScale = (float)mapController.mapWidth / heightMap.width;

        vertecies = new Vector3[gridWidth * gridHeight];
        uvs = new Vector2[gridWidth * gridHeight];
        triangles = new int[(gridWidth - 1) * (gridHeight - 1) * 6];

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
        for (int y = 0; y < this.gridHeight; y++)
        {
            for (int x = 0; x < this.gridWidth; x++)
            {
                int index = y * this.gridWidth + x;

                vertecies[index] = new Vector3(
                    -x, 
                    heightMap.GetPixel(
                        Mathf.FloorToInt(x * spaceBetweenPoints / worldToMapScale), 
                        Mathf.FloorToInt(y * spaceBetweenPoints / worldToMapScale)
                        ).r * scale + Random.Range(.0f,.01f),
                    y
                );

                uvs[index] = new Vector2((float)x / this.gridWidth, (float)y / this.gridHeight);

                if (x < this.gridWidth - 1 && y < this.gridHeight - 1)
                {
                    AddTriangle(index, index + this.gridWidth + 1, index + this.gridWidth);
                    AddTriangle(index + this.gridWidth + 1, index, index + 1);
                }
            }
            yield return null;
        }

        mesh = new Mesh();
        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        //collider.sharedMesh = mesh;

        //surface.BuildNavMesh();
    }
}
