using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ManageUnits : MonoBehaviour
{
    public GameObject[] unitCounts = new GameObject[5];

    public PlayerControls controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendTeam()
    {
        for (int i = 0; i < unitCounts.Length; i++)
        {
            controller.selectedObjective.units[i] = Int32.Parse(unitCounts[i].GetComponent<Text>().text, CultureInfo.InvariantCulture.NumberFormat);
            
        }
    }

}
