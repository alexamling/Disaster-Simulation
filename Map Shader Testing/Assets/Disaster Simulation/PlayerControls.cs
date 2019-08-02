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
/// Written by Alexander Amling & Max Kaiser
/// This class manages the player's ability to interact with the game
/// </summary>

public class PlayerControls : MonoBehaviour
{
    public MapController manager;
    public PlayerObjective selectedObjective;
    PlayerObjective lastSelected = null;
    public Image progressBar;
    public Image[] coolDowns;
    public ManageUnits unitManager;
    [HideInInspector]
    public GameObject cameraPos;

    #region StatVariables

    public int sucessfulObjectivesCount = 0;
    public int failedObjectivesCount = 0;
    public int totalSentUnits = 0;
    public int totalRequestedUnits = 0;
    public int ignoredObjectivesActual = 0;
    public int ignoredObjectivesIdeal = 0;

    #endregion

    [Header("UI Variables")]
    #region UI Variables
    int numNotifications;
    public InfoPanel notificationPanel;
    public InfoPanel currentObjectivePanel;
    public InfoPanel objectiveLocationPanel;
    public InfoPanel objectiveMessage;
    public InfoPanel objectiveResult;
    public InfoPanel pausePanel;
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
        numNotifications = 0;
        newCamPos = cameraPos.transform.position;

        cam = Camera.main;
        rayCaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        notifications = new List<Notification>();
        raycastResults = new List<RaycastResult>();

        currentObjectivePanel.panel.SetActive(false);
        objectiveMessage.panel.SetActive(false);
        objectiveResult.panel.SetActive(false);
        pausePanel.panel.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedObjective)
            {
                ResetFocus();
                CloseInfoMenu();
            }
            else
            {
                Pause();
            }
        }
        
        screenPos = Input.mousePosition;

        // lerp the camera towards the new location
        newCamPos = Vector3.ClampMagnitude(newCamPos, 500);
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
            newFov = Mathf.Clamp(newFov, 1f, 65);
        }
        #endregion

        #region Move with Right-Click + Drag
        if (Input.GetMouseButtonDown(1))
        {
            clickedPos = screenPos;

        }
        else if (Input.GetMouseButton(1) && !offMap)
        {
            swapPos = (clickedPos - screenPos) * newFov * .015f;
            clickedPos = screenPos;
            newCamPos.x += swapPos.x;
            newCamPos.z += swapPos.y;
        }
        #endregion

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newFov, .2f);
        
        
        ray = cam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit))
        {
            other = hit.collider.gameObject;

            clicked = Input.GetMouseButtonDown(0);
            
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
            }

            if (clicked && !offMap)
            {
                #region Ping Map
                Collider[] colliders = Physics.OverlapSphere(hit.point, 30);

                float shortestDist = float.MaxValue;
                PlayerObjective closestObjective = null;

                // check collisions for the nearest player objective
                for (int i = 0; i < colliders.Length; i++)
                {
                    playerObjective = colliders[i].GetComponent<PlayerObjective>();

                    if (playerObjective == null)
                        continue;

                    if (Vector3.Distance(colliders[i].transform.position, hit.point) < shortestDist)
                    {
                        closestObjective = playerObjective;
                    }
                }

                if (closestObjective && closestObjective.onMap)
                {
                    closestObjective.revealed = true;
                    closestObjective.notification.text.fontStyle = FontStyle.BoldAndItalic;
                    Display(closestObjective);
                    selectedObjective = closestObjective;

                    //Unit Assigning UI Stuff
                    unitManager.ToggleUI(selectedObjective);
                }

                Instantiate(pingParticle, hit.point + Vector3.up, Quaternion.identity);                
                #endregion
            }

        }
    }

    /// <summary>
    /// This changes the image color of the currently selected objctive
    /// </summary>
    /// <param name="obj">objective that is tied to the desired notification</param>
    public void HighlightSelectedObjective(PlayerObjective obj)
    {
        // unhighlight the last selected objective
        if (lastSelected)
            lastSelected.notification.GetComponent<Image>().color = new Color32(51, 47, 41, 200);

        // highlight the new objective
        obj.notification.GetComponent<Image>().color = Color.grey;
        lastSelected = obj;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < coolDowns.Length; i++)
        {
            if (coolDowns[i].fillAmount < 1)
            {
                coolDowns[i].fillAmount += (1.0f / 1500.0f); //1/1500 = 30 seconds
            }
        }
        
        if (selectedObjective != null)
        {
            progressBar.fillAmount = selectedObjective.status;
            progressBar.color = selectedObjective.iconImage.color;
        }
    }

    /// <summary>
    /// This is a function to allow buttons to toggle the Pause gamestate
    /// </summary>
    public void Pause()
    {

        if (manager.gameTimer.gameState == GameState.Paused)
        {
            pausePanel.panel.SetActive(false);
            manager.gameTimer.gameState = GameState.Running;
        }
        else
        {
            pausePanel.panel.SetActive(true);
            manager.gameTimer.gameState = GameState.Paused;
        }

    }
    

    public void Display(PlayerObjective objective)
    {
        HighlightSelectedObjective(objective);
        objective.notification.FocusOnObjective();
        if (objective.active)
        {
            objectiveMessage.panel.SetActive(true);
            objectiveResult.panel.SetActive(false);
            objectiveMessage.text.text = objective.fullMessage;
        }
        if (objective.objectiveState == ObjectiveState.Resolved)
        {
            UpdateResult(objective);
        }
    }

    public void UpdateResult(PlayerObjective objective)
    {
        if (objective != selectedObjective)
            return;

        if (objective.status <= 0)
        {
            objectiveMessage.panel.SetActive(true);
            objectiveResult.panel.SetActive(true);
            //display failure message

            objectiveMessage.text.text = "Failure";
        }
        else
        {
            objectiveMessage.panel.SetActive(true);
            objectiveResult.panel.SetActive(true);
            //display sucess message

            objectiveMessage.text.text = "Sucess";
        }
    }

    public void CloseCurrentObjective()
    {
        objectiveMessage.panel.SetActive(false);
        selectedObjective.notification.Close();
    }

    public void CloseInfoMenu()
    {
        if (selectedObjective)
        {
            selectedObjective.selected = false;
            selectedObjective = null;
        }

        objectiveMessage.panel.SetActive(false);
        objectiveResult.panel.SetActive(false);
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
