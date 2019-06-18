using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public PlayerUnit unit;

    private Manager manager;
    private TerrainGenerator terrainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        manager = gameObject.GetComponent<Manager>();
        terrainGenerator = gameObject.GetComponent<TerrainGenerator>();
    }

    public void SpawnUnit(Vector3 position)
    {
        Instantiate(unit, position, Quaternion.identity);
    }

    public void SpawnUnit()
    {
        float xPos = Random.Range(0, manager.mapWidth * Mathf.Pow(.5f, terrainGenerator.LOD));
        float zPos = Random.Range(0, manager.mapHeight * Mathf.Pow(.5f, terrainGenerator.LOD));
        float yPos = manager.heightMap.GetPixel((int)xPos, (int)zPos).r * terrainGenerator.scale;

        SpawnUnit(new Vector3(-xPos, yPos, zPos));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
