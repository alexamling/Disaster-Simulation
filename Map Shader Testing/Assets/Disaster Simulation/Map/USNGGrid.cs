using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIAnchor
{
    public GameObject anchor;
    public RectTransform UIElement;
    public bool xAxis;
    public bool yAxis;
}

public class USNGGrid : MonoBehaviour
{
    public UIAnchor[] elements;
    [HideInInspector]
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos;
        Vector2 newPos;

        foreach (UIAnchor element in elements)
        {
            screenPos = element.UIElement.position;
            newPos = cam.WorldToScreenPoint(element.anchor.transform.position);
            if (element.xAxis)
                screenPos.x = newPos.x;
            if (element.yAxis)
                screenPos.y = newPos.y;

            element.UIElement.position = screenPos;
        }


    }

    void PlaceDivisions()
    {

    }
}
