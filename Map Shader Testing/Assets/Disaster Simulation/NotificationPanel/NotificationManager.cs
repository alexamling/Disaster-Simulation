using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This manager tracks the generation and manipulation of the notification panel
/// Written by Alexander Amling
/// </summary>

public class NotificationManager : MonoBehaviour
{
    public InfoPanel notificationPanel;

    public InfoPanel currentObjectivePanel;

    public Notification notificationPrefab;

    public List<Notification> notifications;

    public PlayerObjective objectivePrefab;
    
    public PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        notifications = new List<Notification>();
        currentObjectivePanel.panel.SetActive(false);
    }
    
    void Update()
    {
    // test input to add new notification 
    // TODO: remove this
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerObjective newObjective = Instantiate(objectivePrefab);
            Vector3 newPos; 
            newPos.x = Random.Range(-512, 512);
            newPos.z = Random.Range(-450, 450);
            newPos.y = 5;
            newObjective.transform.position = newPos;
        }
    }
}
