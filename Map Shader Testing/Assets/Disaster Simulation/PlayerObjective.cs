using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is for each of the objects that the player will directly interact with during the game
/// the objects manage their own outline based on their status
/// Written by Alexander Amling
/// </summary>

public class PlayerObjective : MonoBehaviour
{
    [Range(0,1)]
    public float status = 1;
    [HideInInspector]
    public Outline outline;

    public bool selected;
    public bool hover;
    public bool revealed;

    public float[] responseModifiers;

    public Notification notification;

    private float response;

    private bool active;

    private float mix;
    
    protected void Start()
    {
        selected = false;
        active = true;
        outline = gameObject.AddComponent<Outline>();
    }
    
    protected void Update()
    { 
        if (active && revealed) // shift the color based on status green -> yellow -> orange -> red
        {
            outline.OutlineWidth = 2.5f * Mathf.Sin(Time.time * (25f * (1.0f - status))) + 2.5f;
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
            }
        }
            

        if (status <= 0.001f && active) // turn the outline solid black when the status is low enough
        {
            outline.OutlineWidth = 5.0f;
            outline.OutlineColor = Color.black;
            active = false;
        }

        if (selected)
        {
            outline.OutlineWidth = 5.0f;
            outline.OutlineColor = new Color(1, .5f, 0);
        }
        else if (hover)
        {
            outline.OutlineWidth = 7.5f;
            outline.OutlineColor = Color.yellow;
            hover = false;
        }
    }
}
