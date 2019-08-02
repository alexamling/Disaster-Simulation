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

    public PlayerControls manager;
    
    void Start()
    {
        objective.revealed = false;
        rectTransform = gameObject.GetComponent<RectTransform>();
        manager = FindObjectOfType<PlayerControls>();
    }

    /// <summary>
    /// when a notification is clocked on, the camera should refocus according to the stat of the objective
    /// </summary>
    public void Clicked()
    {
        // highlight the clicked notification
        manager.HighlightSelectedObjective(objective);
        manager.objectiveMessage.panel.SetActive(false);

        if (objective.revealed)
        {
            manager.currentObjectivePanel.panel.SetActive(false);
            manager.Display(objective);
        }
        else
        {
            objective.onMap = true;
            manager.currentObjectivePanel.panel.SetActive(true);
            manager.currentObjectivePanel.text.text = text.text;
            Vector2 objectivePos = USNGGrid.ToUSNG(objective.transform.position);
            manager.objectiveLocationPanel.text.text = "Located at: " + (int)objectivePos.x + ", " + (int)objectivePos.y;
            manager.FocusOn(new Vector2(0, 0), 60);
        }
    }

    /// <summary>
    /// This method is used to clean up objectives and all objects attached to them, as well as make appropriate changes to the score
    /// </summary>
    public void Close()
    {
        if (objective.objectiveState != ObjectiveState.Resolved)
        {
            if (!objective.needsResponse)
            {
                manager.manager.score += objective.originalScore;
            }

            manager.objectiveMessage.panel.SetActive(false);

            if (objective.needsResponse && objective.status >= 1)
            {
                manager.manager.score += objective.score;
            }

            manager.ignoredObjectivesActual++;
        }

        manager.currentObjectivePanel.panel.SetActive(false);

        Destroy(objective.gameObject);
        Destroy(gameObject);
        Destroy(objective.icon);
    }

    /// <summary>
    /// used to focus the camera on whatever objective is tied to this notification
    /// </summary>
    public void FocusOnObjective()
    {
        gameObject.GetComponent<Image>().color = Color.grey;
        Vector3 objectivePos = objective.transform.position;
        manager.selectedObjective = objective;
        manager.FocusOn(new Vector2(objectivePos.x, objectivePos.z), 20);
    }
}