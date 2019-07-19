using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObjectiveState { Inactive, Requesting, Active, Resolved };

/// <summary>
/// This class is for each of the objects that the player will directly interact with during the game
/// the objects manage their own outline based on their status
/// Written by Alexander Amling
/// </summary>

public class PlayerObjective: MonoBehaviour
{
    [Range(0,1)]
    public float status = 0.99f;
    public float score;
    [HideInInspector]
    public Outline outline;
    public string[] tipString = new string[] { "Nothing to report", "Nothing to report", "Nothing to report" }; //Plan, Log, Ops

    public bool selected;
    public bool hover;
    public bool revealed;

    public GameObject iconPrefab;
    public GameObject icon;
    private Image iconImage;
    private Canvas canvas;
    private Camera cam;

    public Vector2 location;

    public float[] immediateResponseModifiers;
    public float[] delayedResponseModifiers;
    public int[] units; //0EMS, 1Fire Department, 2Military, 3Police, 4Volunteers

    public string notificationTitle;
    public string fullMessage;

    public Notification notification;

    public ObjectiveState objectiveState = ObjectiveState.Inactive;

    public List<PlayerObjective> relatedObjectives;

    public float scoreDeprecator;

    public float StatusDeprecator;

    public float timeLimit;

    public bool hasImmediateResponded = false;

    public bool active { get { return objectiveState == ObjectiveState.Active || objectiveState == ObjectiveState.Requesting; } }

    private float mix;
    
    protected void Start()
    {
        selected = false;
        //outline = gameObject.AddComponent<Outline>();

        scoreDeprecator = score / ((1 / Time.fixedDeltaTime) * timeLimit);
        StatusDeprecator = 0 + (scoreDeprecator - 0) * (1 - 0) / (score - 0);
        
        canvas = FindObjectOfType<Canvas>();
        cam = FindObjectOfType<Camera>();
        icon = Instantiate(iconPrefab, canvas.transform);
        iconImage = icon.GetComponentInChildren<Image>();

        //DEBUG
        revealed = true;
        //units = new int[] { 1, 3};
    }
    
    protected void Update()
    { 
        icon.transform.position = cam.WorldToScreenPoint(transform.position);

        if (active && revealed) // shift the color based on status green -> yellow -> orange -> red
        {
            float value = .5f * Mathf.Sin(Time.time * (10f * (1.0f - status) - .01f)) + .5f;
            Color c = new Color();
            if (status > .5)
            {
                mix = Mathf.InverseLerp(.5f, 1, status);
                //outline.OutlineColor = mix * Color.green + (1 - mix) * Color.yellow;
                c = value * (mix * Color.green + (1 - mix) * Color.yellow);
            }
            else if (status > .25)
            {
                mix = Mathf.InverseLerp(.25f, .5f, status);
                //outline.OutlineColor = mix * Color.yellow + (1 - mix) * new Color(1, .5f, 0);
                c = value * (mix * Color.yellow + (1 - mix) * new Color(1, .5f, 0));
            }
            else
            {
                mix = Mathf.InverseLerp(.0f, .25f, status);
                //outline.OutlineColor = mix * new Color(1, .5f, 0) + (1 - mix) * Color.red;
                c = value * (mix * new Color(1, .5f, 0) + (1 - mix) * Color.red);
            }
            c.a = 1.0f;
            iconImage.color = c;
            //status -= .001f;
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
        if (active)
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
                        float incrimentorImm = (0 + ((immediateResponseModifiers[i] / 10.0f) - 0) * (1 - 0) / (10.0f - 0)) * units[i];
                        status += incrimentorImm;
                        score += Mathf.Clamp((incrimentorImm * 9.25f), 0.0f, scoreDeprecator);
                        
                    }
                }
                for (int i = 0; i < units.Length; i++)
                {
                    //score += delayedResponseModifiers[units[i]] / 100.0f;
                    float incrimentorDel = (0 + ((delayedResponseModifiers[i] / 100.0f) - 0) * (1 - 0) / (100.0f - 0) * units[i]);
                    status += incrimentorDel;
                    score += Mathf.Clamp((incrimentorDel * 25.0f), 0.0f, scoreDeprecator);
                    Debug.Log(Mathf.Clamp((incrimentorDel * 25.0f), 0.0f, scoreDeprecator));
                }
            }

            if (status >= 1)
            {
                iconImage.color = Color.green;
                status = 1;
                objectiveState = ObjectiveState.Resolved;
            }

            if (status <= 0.001f) // turn the outline solid black when the status is low enough
            {
                iconImage.color = Color.black;
                objectiveState = ObjectiveState.Resolved;
            }
        }
    }
}
