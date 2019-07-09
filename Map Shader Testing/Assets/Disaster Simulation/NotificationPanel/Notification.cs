using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to simplify the process of displaying and modifying objective notifications
/// The static class included is untility intended to make this process easier
/// </summary>
public class Notification : MonoBehaviour
{
    public int severity;
    public Text text;
    public PlayerObjective objective;

    public RectTransform rectTransform;

    public NotificationManager manager;
    
    void Start()
    {
        objective.revealed = false;
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    
    void Update()
    {
        
    }

    public void Clicked()
    {
        if (objective.revealed)
        {
            manager.currentObjectivePanel.SetActive(false);
            FocusOnObjective();
        }
        else
        {
            manager.currentObjectivePanel.SetActive(true);
            manager.currentObjectivePanel.GetComponentInChildren<Text>().text = text.text;
            manager.playerControls.FocusOn(new Vector2(0, 0), 60);
        }
    }

    public void FocusOnObjective()
    {
        if (objective.revealed)
        {
            Vector3 objectivePos = objective.transform.position;
            manager.playerControls.FocusOn(new Vector2(objectivePos.x, objectivePos.z), 20);
        }
    }
}

// http://answers.unity.com/answers/1610964/view.html
/// <summary>
/// Utility class to allow for easier manipulation of rectTransforms.
/// Found on unity forums.
/// Written by: https://answers.unity.com/users/546375/eldoir.html
/// </summary>
public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}