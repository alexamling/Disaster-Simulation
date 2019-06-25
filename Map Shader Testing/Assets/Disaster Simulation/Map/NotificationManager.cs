using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification
{
    int severity;
    string text;
    GameObject objective;
    float height;

    Notification()
    {

    }

}

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPanel;

    public GameObject notificationPrefab;

    public List<Notification> notifications;

    int numEvents;
    float spacingBetweenNotifications;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNotification(string message, int severity)
    {

    }
}
