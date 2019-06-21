using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Manager
{
    public ComputeShader viewMapShader;
    private int generateTextureKernel;
    private int viewMapKernel;

    public RenderTexture viewPattern;

    public float radius = 75;
    public float falloff = 50;

    public PlayerUnit unit;

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
        
        cam = Camera.main;

        output = new RenderTexture(mapWidth, mapHeight, 24);
        output.enableRandomWrite = true;
        output.Create();

        viewPattern = new RenderTexture(mapWidth, mapHeight, 24);
        viewPattern.enableRandomWrite = true;
        viewPattern.Create();

        generateTextureKernel = viewMapShader.FindKernel("GenerateTexture");
        viewMapKernel = viewMapShader.FindKernel("GenerateViewMap");

        viewMapShader.SetTexture(generateTextureKernel, "Pattern", viewPattern);
        viewMapShader.SetFloat("radius", radius);
        viewMapShader.SetFloat("falloff", falloff);
        viewMapShader.SetTexture(viewMapKernel, "Pattern", viewPattern);
        viewMapShader.SetTexture(viewMapKernel, "Result", output);

        viewMapShader.Dispatch(generateTextureKernel, mapWidth / 8, mapHeight / 8, 1);
    }

    public void SpawnUnit(GameObject GO, Vector3 position)
    {
        units.Add(Instantiate(GO, position, Quaternion.identity).GetComponent<PlayerUnit>());
    }

    public void SpawnUnit(GameObject GO)
    {
        float xPos = -Random.Range(0, mapWidth) * mapWidth / heightMap.width;
        float zPos = Random.Range(0, mapHeight) * mapWidth / heightMap.width;

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

        if (units.Count > 0)
        {
            Vector3 pos;

            for (int i = 0; i < units.Count; i++)
            {
                pos = units[i].transform.position;
                values[i * 4] = pos.x;
                values[i * 4 + 1] = pos.z;
                
                if (newDest != Vector3.zero)
                    if(units[i].agent.isOnNavMesh)
                        units[i].agent.SetDestination(newDest);
            }
            
            viewMapShader.SetInt("numViewPoints", units.Count);
            viewMapShader.SetFloats("viewPointData", values);

            viewMapShader.Dispatch(viewMapKernel, mapWidth / 8, mapHeight / 8, 1);
        }
    }

    public override IEnumerator Load()
    {
        throw new System.NotImplementedException();
    }
}
