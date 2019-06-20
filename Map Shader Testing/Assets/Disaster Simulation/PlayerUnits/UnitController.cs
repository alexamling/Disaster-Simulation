using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public PlayerUnit unit;

    public Material mapMaterial;

    private Manager manager;
    private TerrainGenerator terrainGenerator;
    private new Renderer renderer;

    private List<PlayerUnit> units;
    private float[] values;

    #region Input Variables
    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        values = new float[0];

        units = new List<PlayerUnit>();

        manager = gameObject.GetComponent<Manager>();
        terrainGenerator = gameObject.GetComponent<TerrainGenerator>();
        renderer = gameObject.GetComponent<Renderer>();
        cam = Camera.main;
    }

    public void SpawnUnit(GameObject GO, Vector3 position)
    {
        units.Add(Instantiate(GO, position, Quaternion.identity).GetComponent<PlayerUnit>());
    }

    public void SpawnUnit(GameObject GO)
    {
        float xPos = -Random.Range(0, manager.mapWidth) * manager.mapWidth / manager.heightMap.width;
        float zPos = Random.Range(0, manager.mapHeight) * manager.mapWidth / manager.heightMap.width;

        RaycastHit hit;

        if(Physics.Raycast(new Ray(new Vector3(xPos, 10000.0f, zPos), Vector3.down), out hit))
        {
            float yPos = hit.point.y;
            SpawnUnit(GO, new Vector3(xPos, yPos, zPos));
        }
        else
        {
            Debug.Log("Miss");
        }

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 newDest = Vector3.zero;

        if(units.Count * 5 > values.Length)
        {
            values = new float[units.Count * 5];
        }

        if (Input.GetMouseButtonDown(0))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                newDest = hit.point;
            }
        }

        if (mapMaterial && units.Count > 0)
        {
            Vector3 pos;

            for (int i = 0; i < units.Count; i++)
            {
                pos = units[i].transform.position;
                values[i * 5] = pos.x;
                values[i * 5 + 1] = pos.y;
                values[i * 5 + 2] = pos.z;
                values[i * 5 + 3] = 5;
                values[i * 5 + 4] = 2;
                
                if (newDest != Vector3.zero)
                    if(units[i].agent.isOnNavMesh)
                        units[i].agent.SetDestination(newDest);
            }


            mapMaterial.SetInt("_NumInteractions", units.Count);
            mapMaterial.SetFloatArray("_InteractionPoints", values);

            mapMaterial.SetTexture("_FireMap", manager.output);

            renderer.material = mapMaterial;
        }
    }
}
