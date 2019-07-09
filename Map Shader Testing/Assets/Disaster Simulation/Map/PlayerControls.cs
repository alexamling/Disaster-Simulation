﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages the player's ability to interact with the game
/// </summary>

public class PlayerControls : MonoBehaviour
{
    public MapController manager;
    public PlayerObjective selectedObjective;
    public RadialMenu radialMenu;
    public GameObject cameraPos;

    // lists of buttons for the radial menu
    // TODO: improve on this/replace several lists with seperate menus
    public List<Button> options;

    Ray ray;
    Camera cam;
    RaycastHit hit;
    GameObject other;
    bool clicked;
    float newFov;
    Vector3 screenPos;
    Vector3 newCamPos;
    float panningBorderWidth;

    GraphicRaycaster rayCaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [Space(10)]
    public ParticleSystem pingParticle;

    void Start()
    {
        cam = Camera.main;
        newFov = 60;
        panningBorderWidth = 32;
        newCamPos = cameraPos.transform.position;

        rayCaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        radialMenu.Display(options);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && manager.terrainGenerator)
        {
            StartCoroutine(manager.terrainGenerator.Load());
        }

        #region Zoom with Scroll Wheel
        newFov -= Input.GetAxis("Mouse ScrollWheel") * 10;
        newFov = Mathf.Clamp(newFov, 10, 65);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newFov, .2f);
        #endregion

        screenPos = Input.mousePosition;

        Debug.Log(Input.mousePosition);

        #region Camera Panning
        if(screenPos.x > cam.scaledPixelWidth - panningBorderWidth)
        {
            newCamPos += Vector3.right * 3;
        }
        else if (screenPos.x < panningBorderWidth)
        {
            newCamPos += Vector3.left * 3;
        }
        else if (screenPos.y > cam.scaledPixelHeight - panningBorderWidth)
        {
            newCamPos += Vector3.forward * 3;
        }
        else if (screenPos.y < panningBorderWidth)
        {
            newCamPos += Vector3.back * 3;
        }
        #endregion

        // lerp the camera towards the new location
        cameraPos.transform.position = Vector3.Lerp(cameraPos.transform.position, newCamPos, .2f);
        
        // Raycast from screen to world
        ray = cam.ScreenPointToRay(Input.mousePosition);
        // update the highlighted objective
        if (Physics.Raycast(ray, out hit))
        {
            other = hit.collider.gameObject;

            clicked = Input.GetMouseButtonDown(0);

            #region Set Radial Menu Position
            try
            {
                other.GetComponent<PlayerObjective>().hover = true;
                if (clicked)
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        other.GetComponent<PlayerObjective>().selected = true;
                        if (selectedObjective)
                            selectedObjective.selected = false;
                        selectedObjective = other.GetComponent<PlayerObjective>();
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
            #endregion

            if (clicked)
            {
                #region Ping Map
                Collider[] colliders = Physics.OverlapSphere(hit.point, 15);

                for(int i = 0; i < colliders.Length; i++)
                {
                    PlayerObjective playerObjective = colliders[i].GetComponent<PlayerObjective>();

                    if (playerObjective)
                    {
                        playerObjective.revealed = true;
                        playerObjective.notification.text.fontStyle = FontStyle.BoldAndItalic;
                    }
                }

                Instantiate(pingParticle, hit.point + Vector3.up, Quaternion.identity);                
                #endregion
            }

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

    public void FocusOn(Vector2 pos, float fov)
    {
        newCamPos = new Vector3(pos.x, 0, pos.y);
        newFov = fov;
    }
}
