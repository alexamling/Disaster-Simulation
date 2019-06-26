using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPanel;

    public Notification notificationPrefab;

    public List<Notification> notifications;

    int numEvents;
    float spacingBetweenNotifications;
    float lastNotificationPos;


    // Start is called before the first frame update
    void Start()
    {
        notifications = new List<Notification>();
        lastNotificationPos = 0;
        spacingBetweenNotifications = 10;
    }

    // Update is called once per frame
    void Update()
    {
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
        newNotification.rectTransform.SetTop(lastNotificationPos + newNotification.rectTransform.rect.height + spacingBetweenNotifications);
        notifications.Add(newNotification);
    }
}
