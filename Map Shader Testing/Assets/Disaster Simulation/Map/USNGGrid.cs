﻿using System.Collections;
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
    public RectTransform parent;
    public RectTransform markerPrefab;
    public UIAnchor[] elements;
    [HideInInspector]
    public Camera cam;
    List<UIAnchor> xElements;
    List<UIAnchor> yElements;
    RectTransform[] xMarkers;
    RectTransform[] yMarkers;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        xElements = new List<UIAnchor>();
        yElements = new List<UIAnchor>();

        foreach(UIAnchor element in elements)
        {
            if (element.xAxis)
                xElements.Add(element);
            else
                yElements.Add(element);
        }

        Debug.Log(xElements.Count + " " + yElements.Count);

        xMarkers = new RectTransform[(xElements.Count - 1) * 9];
        for(int i = 0; i < xMarkers.Length; i++)
        {
            xMarkers[i] = Instantiate(markerPrefab);
            xMarkers[i].parent = parent;
        }

        yMarkers = new RectTransform[(yElements.Count - 1) * 9];
        for (int i = 0; i < yMarkers.Length; i++)
        {
            yMarkers[i] = Instantiate(markerPrefab);
            yMarkers[i].parent = parent;
        }
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

        PlaceDivisions();
    }

    void PlaceDivisions()
    {
        Vector3 newPos;
        for(int e = 0; e < xElements.Count - 1; e++)
        {
            for(int l = 0; l < 9; l++)
            {
                newPos = 
                    cam.WorldToScreenPoint(
                        Vector3.Lerp(
                            xElements[e].anchor.transform.position,
                            xElements[e+1].anchor.transform.position,
                            (l + 1) / 10.0f
                            )
                        );
                newPos.y = 1080;
                newPos.z = 0;
                xMarkers[e * 9 + l].position = newPos;
            }
        }

        for (int e = 0; e < yElements.Count - 1; e++)
        {
            for (int l = 0; l < 9; l++)
            {
                newPos =
                    cam.WorldToScreenPoint(
                        Vector3.Lerp(
                            yElements[e].anchor.transform.position,
                            yElements[e + 1].anchor.transform.position,
                            (l + 1) / 10.0f
                            )
                        );
                newPos.x = 0;
                newPos.z = 0;
                yMarkers[e * 9 + l].position = newPos;
            }
        }
    }
}
