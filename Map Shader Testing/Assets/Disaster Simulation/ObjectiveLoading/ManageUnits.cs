using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ManageUnits : MonoBehaviour
{
    public GameObject[] unitCounts = new GameObject[5];
    public int[] availibleUnits = new int[5];
    private bool isUnitsEmpty = true;
    public GameObject resourceBar;
    public GameObject[] elementsUI = new GameObject[12];
    public Text[] resourceValues = new Text[5];

    public PlayerControls controller;
    private MapController mapController;

    // Start is called before the first frame update
    void Start()
    {
        resourceValues = resourceBar.GetComponentsInChildren<Text>();
        GameObject[] children = new GameObject[5]; 
        for (int i = 0; i < availibleUnits.Length; i++)
        {
            resourceValues[i] = resourceBar.transform.GetChild(i).GetChild(0).GetComponent<Text>();
            resourceValues[i].text = "" + availibleUnits[i];
        }

        mapController = GameObject.Find("Main Camera").GetComponent<MapController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (isUnitsEmpty)
        {
            elementsUI[10].GetComponent<Button>().interactable = false;
        }
        else if (!isUnitsEmpty)
        {
            elementsUI[10].GetComponent<Button>().interactable = true;
        }

        for (int i = 0; i < unitCounts.Length; i++)
        {
            if (Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat) != 0)
            {
                isUnitsEmpty = false;
                break;
            }
            else if (Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat) == 0)
            {
                isUnitsEmpty = true;
            }
        }
    }

    public void ignoreResponse()
    {
        if (controller.selectedObjective.needsResponse)
        {

        }

        else if (!controller.selectedObjective.needsResponse)
        {
            mapController.score += controller.selectedObjective.originalScore;
        }

        controller.selectedObjective.notification.Close();
    }

    public void sendTeam()
    {
        int val;
        for (int i = 0; i < unitCounts.Length; i++)
        {
            val = Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat);
            controller.selectedObjective.units[i] = val;
            availibleUnits[i] -= val;
            resourceValues[i].text = "" + availibleUnits[i];
        }

        controller.selectedObjective.objectiveState = ObjectiveState.Responding;
        ToggleUI(controller.selectedObjective);
    }

    public void ToggleUI(PlayerObjective selectedObject)
    {
        if (selectedObject.objectiveState == ObjectiveState.Requesting)
        {
            for (int i = 0; i < elementsUI.Length - 1; i++)
            {
                elementsUI[i].SetActive(true);
            }
            elementsUI[12].GetComponent<Text>().text = "Units Requested";
        }
        else if (selectedObject.objectiveState == ObjectiveState.Responding)
        {
            for (int i = 0; i < elementsUI.Length - 1; i++)
            {
                elementsUI[i].SetActive(false);
            }
            elementsUI[12].GetComponent<Text>().text = "Units Sent";
        }
        for (int i = 0; i < unitCounts.Length; i++)
        {
            unitCounts[i].GetComponent<Text>().text = selectedObject.units[i].ToString();

            //Remember to update with populated values as default
            unitCounts[i].GetComponent<Counter>().value = 0;
        }
    }

    public void restoreUnits(PlayerObjective obj)
    {
        for (int i = 0; i < unitCounts.Length; i++)
        {
            availibleUnits[i] += obj.units[i];
            resourceValues[i].text = "" + availibleUnits[i];
        }

        if (obj.score > 0.0f)
        {
            mapController.score += obj.score;
        }
        else
        {
            mapController.score += 0;
        }
    }

}
