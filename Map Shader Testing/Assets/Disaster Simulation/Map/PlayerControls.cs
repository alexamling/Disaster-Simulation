using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerControls : MonoBehaviour
{

    public PlayerObjective selectedObjective;
    public RadialMenu radialMenu;

    public List<Button> options;
    public List<Button> taskForceOptions;
    public List<Button> withdrawOptions;
    public List<Button> updateOptions;
    public List<Button> infoOptions;

    Ray ray;
    Camera cam;
    RaycastHit hit;
    PlayerObjective other;
    Vector3 screenPos;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        radialMenu.Display(options);
    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.P))
        {
            radialMenu.Display(taskForceOptions);
        }

        // update the highlighted objective
        if (Physics.Raycast(ray, out hit))
        {
            try
            {
                other = hit.collider.gameObject.GetComponent<PlayerObjective>();
                other.hover = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        other.GetComponent<PlayerObjective>().selected = true;
                        if (selectedObjective)
                            selectedObjective.selected = false;
                        selectedObjective = other;
                    }
                }
            }
            catch
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        CloseInfoMenu();
                    }
                }
            }

            if (selectedObjective)
            {
                screenPos = cam.WorldToScreenPoint(selectedObjective.transform.position);
                radialMenu.SetPosition(new Vector3(screenPos.x, screenPos.y, 100));
            }
            else
                radialMenu.SetPosition(new Vector3(-1000, 0, 0));
        }
    }

    public void CloseInfoMenu()
    {
        if (selectedObjective)
        {
            selectedObjective.selected = false;
            selectedObjective = null;
        }
    }
}
