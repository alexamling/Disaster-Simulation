using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{

    public PlayerObjective selectedObjective;
    public RectTransform infoOptions;

    Ray ray;
    Camera cam;
    RaycastHit hit;
    PlayerObjective other;
    Vector3 screenPos;

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
            try
            {
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
                if (Input.GetMouseButtonDown(0))
                {
                    CloseInfoMenu();
                }
                Debug.Log("Catch");
            }

            if (selectedObjective)
            {
                screenPos = cam.WorldToScreenPoint(selectedObjective.transform.position);
                infoOptions.position = new Vector3(screenPos.x, screenPos.y, 100);
            }
            else
                infoOptions.position = new Vector3(-1000, 0, 0);
        }
    }

    public void CloseInfoMenu()
    {
        selectedObjective.selected = false;
        selectedObjective = null;
    }
}
