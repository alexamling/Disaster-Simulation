﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjective : MonoBehaviour
{
    [Range(0,1)]
    public float status = 1;
    [HideInInspector]
    public Outline outline;

    public bool selected;
    public bool hover;

    private bool active;

    private float mix;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        active = true;
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {


        if (active)
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
            

        if (status <= 0.001f && active)
        {
            outline.OutlineWidth = 5.0f;
            outline.OutlineColor = Color.black;
            active = false;
        }
        if (selected)
        {
            outline.OutlineWidth = 15.0f;
            outline.OutlineColor = new Color(1, .5f, 0);
        }
        else if (hover)
        {
            outline.OutlineWidth = 10.0f;
            outline.OutlineColor = Color.yellow;
            hover = false;
        }
    }
}
