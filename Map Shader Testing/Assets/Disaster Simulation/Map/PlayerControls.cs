using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct InfoPanel
{
    public GameObject panel;
    public Text text;
}

/// <summary>
/// This class manages the player's ability to interact with the game
/// </summary>

public class PlayerControls : MonoBehaviour
{
    public MapController manager;
    public PlayerObjective selectedObjective;
    public GameObject[] coolDowns;
    [HideInInspector]
    public RadialMenu radialMenu;
    public GameObject cameraPos;

    // lists of buttons for the radial menu
    // TODO: improve on this/replace several lists with seperate menus
    public List<Button> options;

    [Header("UI Variables")]
    #region UI Variables
    int numNotifications;
    public InfoPanel notificationPanel;
    public InfoPanel currentObjectivePanel;
    public InfoPanel objectiveLocationPanel;
    public InfoPanel objectiveMessage;
    public Notification notificationPrefab;
    public PlayerObjective objectivePrefab;
    public List<Notification> notifications;
    #endregion

    #region Raycasting Variables
    Ray ray;
    Camera cam;
    RaycastHit hit;
    GameObject other;
    PlayerObjective playerObjective;
    GraphicRaycaster rayCaster;
    List<RaycastResult> raycastResults;
    PointerEventData pointerEventData;
    EventSystem eventSystem;
    #endregion

    #region Camera Variables
    bool clicked;
    bool offMap;
    float newFov;
    Vector3 screenPos;
    Vector3 clickedPos;
    Vector3 swapPos;
    Vector3 newCamPos;
    float panningBorderWidth;
    #endregion


    [Space(10)]
    public ParticleSystem pingParticle;

    void Start()
    {
        newFov = 60;
        //panningBorderWidth = 10;
        numNotifications = 0;
        newCamPos = cameraPos.transform.position;

        cam = Camera.main;
        rayCaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        notifications = new List<Notification>();
        raycastResults = new List<RaycastResult>();

        currentObjectivePanel.panel.SetActive(false);
        objectiveMessage.panel.SetActive(false);

        radialMenu.Display(options);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && selectedObjective)
        {
            ResetFocus();
            CloseInfoMenu();
        }

        /*
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerObjective newObjective = Instantiate(objectivePrefab);
            Vector3 newPos;
            newPos.x = Random.Range(-512, 512);
            newPos.z = Random.Range(-512, 512);
            newPos.y = 5; // heightMap.GetPixel((int)newPos.x, (int)newPos.z).r;
            newObjective.transform.position = newPos;
            AddNotification("Test " + ++numNotifications, 0, newObjective);
        }
        */

        if (Input.GetKeyDown(KeyCode.G) && manager.terrainGenerator)
        {
            StartCoroutine(manager.terrainGenerator.Load());
        }
        
        screenPos = Input.mousePosition;
        
        if (Input.GetMouseButtonDown(1))
        {
            clickedPos = screenPos;

        }
        else if (Input.GetMouseButton(1))
        {
            swapPos = (clickedPos - screenPos) * newFov * .015f;
            clickedPos = screenPos;
            newCamPos.x += swapPos.x;
            newCamPos.z += swapPos.y;
        }

        /*
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
        */

        // lerp the camera towards the new location
        newCamPos = Vector3.ClampMagnitude(newCamPos,750);
        cameraPos.transform.position = Vector3.Lerp(cameraPos.transform.position, newCamPos, .1f);
        
        // raycast to UI
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        raycastResults.Clear();
        rayCaster.Raycast(pointerEventData, raycastResults);

        // check if you're hovering over the notification panel
        offMap = false;
        foreach (RaycastResult r in raycastResults)
        {
            offMap = true;
        }

        #region Zoom with Scroll Wheel
        if (!offMap)
        {
            newFov -= Input.GetAxis("Mouse ScrollWheel") * 20;
            newFov = Mathf.Clamp(newFov, 2.5f, 65);
        }
        #endregion

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newFov, .2f);
        
        
        ray = cam.ScreenPointToRay(Input.mousePosition);
        
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

            if (clicked && !offMap)
            {
                #region Ping Map
                Collider[] colliders = Physics.OverlapSphere(hit.point, 45);

                for(int i = 0; i < colliders.Length; i++)
                {
                    playerObjective = colliders[i].GetComponent<PlayerObjective>();

                    if (playerObjective)
                    {
                        playerObjective.revealed = true;
                        playerObjective.notification.text.fontStyle = FontStyle.BoldAndItalic;
                        playerObjective.notification.Display();
                        selectedObjective = playerObjective;
                    }
                }

                Instantiate(pingParticle, hit.point + Vector3.up, Quaternion.identity);                
                #endregion
            }

        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < coolDowns.Length; i++)
        {
            if (coolDowns[i].GetComponent<Image>().fillAmount < 1)
            {
                coolDowns[i].GetComponent<Image>().fillAmount += (1.0f / 750.0f);
            }
        }
    }

    public void AddNotification(string message, int severity, PlayerObjective objective)
    {
        Notification newNotification = Instantiate(notificationPrefab, notificationPanel.panel.transform);
        newNotification.text.text = message;
        newNotification.severity = severity;
        newNotification.objective = objective;
        objective.notification = newNotification;
        notifications.Add(newNotification);
    }

    public void CloseInfoMenu()
    {
        if (selectedObjective)
        {
            selectedObjective.selected = false;
            selectedObjective = null;
        }

        objectiveMessage.panel.SetActive(false);
    }

    public void FocusOn(Vector2 pos, float fov)
    {
        newCamPos = new Vector3(pos.x, 0, pos.y);
        newFov = fov;
    }

    public void ResetFocus()
    {
        selectedObjective = null;
        FocusOn(Vector2.zero, 60);
    }
}
