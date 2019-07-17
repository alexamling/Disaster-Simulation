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
    public GameObject resourceBar;
    private Text[] resourceValues = new Text[5];

    public PlayerControls controller;

    // Start is called before the first frame update
    void Start()
    {
        resourceValues = resourceBar.GetComponentsInChildren<Text>();
        GameObject[] children = new GameObject[5]; 
        for (int i = 0; i < availibleUnits.Length; i++)
        {
            resourceValues[i] = resourceBar.transform.GetChild(i).GetChild(i).GetComponent<Text>();
            resourceValues[i].text = "" + availibleUnits[i];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendTeam()
    {
        int val;
        for (int i = 0; i < unitCounts.Length; i++)
        {
            val = Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat);
            controller.selectedObjective.units[i] = val;
            availibleUnits[i] -= val;
            resourceValues[i].text = "" + val;
        }
    }

}
