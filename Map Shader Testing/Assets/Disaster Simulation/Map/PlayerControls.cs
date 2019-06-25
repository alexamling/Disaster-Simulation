using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public PlayerObjective selectedObjective;

    Ray ray;
    Camera cam;
    RaycastHit hit;
    PlayerObjective other;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        // update the highlighted objective
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit");
            try
            {
                Debug.Log("Try");
                other = hit.collider.gameObject.GetComponent<PlayerObjective>();
                other.hover = true;
                if (Input.GetMouseButtonDown(0))
                {
                    other.GetComponent<PlayerObjective>().selected = true;
                    if (selectedObjective)
                        selectedObjective.selected = false;
                    selectedObjective = other;
                }
            }
            catch
            {
                Debug.Log("Catch");
            }


        }
    }
}
