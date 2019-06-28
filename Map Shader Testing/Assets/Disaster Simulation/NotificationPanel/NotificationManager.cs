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
    public GameObject notificationPanel;

    public Notification notificationPrefab;

    public List<Notification> notifications;

    int numEvents;
    float spacingBetweenNotifications;
    float lastNotificationPos;
    float notificationHeight;

    // Start is called before the first frame update
    void Start()
    {
        notifications = new List<Notification>();
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
            AddNotification("Test", 0);
        }
    }

    public void AddNotification(string message, int severity) //, PlayerObjective objective)
    {
        Notification newNotification = Instantiate(notificationPrefab, notificationPanel.transform);
        newNotification.text.text = message;
        newNotification.severity = severity;
        //newNotification.objective = objective;
        newNotification.rectTransform.SetTop(lastNotificationPos);
        newNotification.rectTransform.SetBottom(notificationPanel.GetComponent<RectTransform>().rect.height - (lastNotificationPos + notificationHeight));
        lastNotificationPos = lastNotificationPos + notificationHeight + spacingBetweenNotifications;
        Debug.Log(lastNotificationPos);
        notifications.Add(newNotification);
    }
}
