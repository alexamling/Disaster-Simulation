﻿using System.Collections;
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
    private new MeshCollider collider;

    private Vector3[] vertecies;
    private int[] triangles;
    private Vector2[] uvs;

    private int triangleIndex;

    public Texture2D heightMap;
    private int width;
    private int height;
    private int verteciesPerLine;
    #endregion

    public IEnumerator Load()
    {
        surface = gameObject.AddComponent<NavMeshSurface>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        collider = gameObject.AddComponent<MeshCollider>();

        width = mapController.mapWidth;
        height = mapController.mapHeight;

        spaceBetweenPoints = (LOD == 0) ? 1 : LOD * 2;
        verteciesPerLine = ((width - 1) / spaceBetweenPoints) + 1;

        worldToMapScale = (float)heightMap.width / width;

        vertecies = new Vector3[verteciesPerLine * verteciesPerLine];
        uvs = new Vector2[verteciesPerLine * verteciesPerLine];
        triangles = new int[(verteciesPerLine - 1) * (verteciesPerLine - 1) * 6];

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
        int index = 0;
        for (int y = 0; y < height; y +=  spaceBetweenPoints)
        {
            for (int x = 0; x < width; x += spaceBetweenPoints)
            {
                vertecies[index] = new Vector3(
                    -x + (width * .5f), 
                    heightMap.GetPixel(
                        Mathf.FloorToInt(x * worldToMapScale), 
                        Mathf.FloorToInt(y * worldToMapScale)
                        ).r * scale + Random.Range(.0f,.01f),
                    y - (height * .5f)
                );

                uvs[index] = new Vector2((float)x / width, (float)y / height);

                if (x < width - spaceBetweenPoints && y < height - spaceBetweenPoints)
                {
                    AddTriangle(index, index + verteciesPerLine + 1, index + verteciesPerLine);
                    AddTriangle(index + verteciesPerLine + 1, index, index + 1);
                }
                index++;

                if(Time.frameCount % 256 == 0)
                    yield return null;
            }
        }

        mesh = new Mesh();
        mesh.vertices = vertecies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        collider.sharedMesh = mesh;

        surface.BuildNavMesh();
    }
}