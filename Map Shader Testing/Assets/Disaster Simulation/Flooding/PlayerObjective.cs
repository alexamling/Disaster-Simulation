using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjective : MonoBehaviour
{
    [Range(0,1)]
    public float status = 1;
    [HideInInspector]
    public Outline outline;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        active = true;
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
            outline.OutlineWidth = 2.5f * Mathf.Sin(Time.time * (25f * (1.0f - status))) + 2.5f;
        if (status <= 0.001f)
        {
            outline.OutlineWidth = 5.0f;
            outline.OutlineColor = Color.black;
            active = false;
        }
    }
}
