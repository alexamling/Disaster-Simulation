using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveState { Inactive, Requesting, Active, Resolved };

/// <summary>
/// This class is for each of the objects that the player will directly interact with during the game
/// the objects manage their own outline based on their status
/// Written by Alexander Amling
/// </summary>

[Serializable]
public class PlayerObjective: MonoBehaviour
{
    [Range(0,1)]
    public float status = 1;
    public float score;
    [HideInInspector]
    public Outline outline;

    public bool selected;
    public bool hover;
    public bool revealed;

    public Vector2 location;

    public float[] immediateResponseModifiers;
    public float[] delayedResponseModifiers;
    public int[] units; //0EMS, 1Fire Department, 2Military, 3Police, 4Volunteers

    public string notificationTitle;
    public string fullMessage;

    public Notification notification;

    public ObjectiveState objectiveState;

    public List<PlayerObjective> relatedObjectives;

    public float scoreDeprecator;

    public float StatusDeprecator;

    public float timeLimit;

    public bool hasImmediateResponded = false;

    private float response;

    private bool active;

    private float mix;
    
    protected void Start()
    {
        objectiveState = ObjectiveState.Inactive;
        selected = false;
        active = true;
        outline = gameObject.AddComponent<Outline>();

        scoreDeprecator = score / ((1 / Time.fixedDeltaTime) * timeLimit);
        StatusDeprecator = 0 + (scoreDeprecator - 0) * (1 - 0) / (score - 0);

        //DEBUG
        revealed = true;
        //units = new int[] { 1, 3};
    }
    
    protected void Update()
    { 
        if (active && revealed) // shift the color based on status green -> yellow -> orange -> red
        {
            /*outline.OutlineWidth = 2.5f * Mathf.Sin(Time.time * (25f * (1.0f - status))) + 2.5f;
            if (status > .5)
            {
                mix = Mathf.InverseLerp(.5f, 1, status);
                outline.OutlineColor = mix * Color.green + (1 - mix) * Color.yellow;
            }
            else if (status > .25)
            {
                mix = Mathf.InverseLerp(.25f, .5f, status);
                outline.OutlineColor = mix * Color.yellow + (1 - mix) * new Color(1, .5f, 0);
            }
            else
            {
                mix = Mathf.InverseLerp(.0f, .25f, status);
                outline.OutlineColor = mix * new Color(1, .5f, 0) + (1 - mix) * Color.red;
            }*/
        }
            

        if (status <= 0.001f) // turn the outline solid black when the status is low enough
        {
            /*outline.OutlineWidth = 5.0f;
            outline.OutlineColor = Color.black;
            active = false;*/
        }

        if (selected)
        {
            //outline.OutlineWidth = 5.0f;
            //outline.OutlineColor = new Color(1, .5f, 0);
        }
        else if (hover)
        {
            //outline.OutlineWidth = 7.5f;
            //outline.OutlineColor = Color.yellow;
            //hover = false;
        }
    }

    void FixedUpdate()
    {
        if (objectiveState != ObjectiveState.Inactive && revealed) // shift the color based on status green -> yellow -> orange -> red
        {
            if (score >= 0)
            {
                score -= scoreDeprecator;

                status -= StatusDeprecator;
            }

            if (units.Length > 0 && status < 1) //if units have been assigned
            {
                if (!hasImmediateResponded)
                {
                    hasImmediateResponded = true;
                    for (int i = 0; i < units.Length; i++)
                    {
                        //score += immediateResponseModifiers[units[i]] / 10.0f;
                        status += (0 + ((immediateResponseModifiers[i] / 10.0f) - 0) * (1 - 0) / (10.0f - 0)) * units[i];
                    }
                }
                for (int i = 0; i < units.Length; i++)
                {
                    //score += delayedResponseModifiers[units[i]] / 100.0f;
                    status += (0 + ((delayedResponseModifiers[i] / 100.0f) - 0) * (1 - 0) / (100.0f - 0) * units[i]);
                }
            }
        }
    }
}
