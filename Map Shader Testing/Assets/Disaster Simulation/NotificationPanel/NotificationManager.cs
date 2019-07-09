using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct InfoPanel
{
    public GameObject panel;
    public Text text;
}

/// <summary>
/// This manager tracks the generation and manipulation of the notification panel
/// Written by Alexander Amling
/// </summary>

public class NotificationManager : Manager
{
    public InfoPanel notificationPanel;

    public InfoPanel currentObjectivePanel;

    public Notification notificationPrefab;

    public List<Notification> notifications;

    public PlayerObjective objectivePrefab;

    int numNotifications;
    float spacingBetweenNotifications;
    float lastNotificationPos;
    float notificationHeight;
    public PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        notifications = new List<Notification>();
        currentObjectivePanel.panel.SetActive(false);

        spacingBetweenNotifications = 5;
        lastNotificationPos = 10;
        notificationHeight = 65;
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
            newPos.z = Random.Range(-512, 512);
            newPos.y = heightMap.GetPixel((int)newPos.x, (int)newPos.z).r;
            newObjective.transform.position = newPos;
            
            AddNotification("Test " + ++numNotifications , 0, newObjective);
        }
    }

    public void AddNotification(string message, int severity, PlayerObjective objective)
    {
        Notification newNotification = Instantiate(notificationPrefab, notificationPanel.panel.transform);
        newNotification.text.text = message;
        newNotification.severity = severity;
        newNotification.objective = objective;
        objective.notification = newNotification;
        newNotification.manager = this;
        notifications.Add(newNotification);
    }

    public override IEnumerator Load()
    {
        throw new System.NotImplementedException();
    }
}
