using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryNodeTest : MonoBehaviour
{
    public int identifier;
    //public string[] description;
    public string[] choices;

    int localPart;
    int localMax;
    public SecondaryNodeTest[] secondNodes;
    //TertiaryNodeTest[] nodes;

    public SecondaryNodeTest(string[] chces, int id, int part, int max, List<string[]> injectParts)
    {
        localMax = max;
        localPart = part;
        choices = chces;
        identifier = id;
        List<string> localValues = new List<string>();
        if (part < max)
        {
            for(int x = 0; x < injectParts[localPart].Length; x++)
            {

            }
        }
    }

    //void Start()
    //{
    //    displayString = description[0];
    //    int nodeAmount = int.Parse(description[1]);
    //    nodes = new TertiaryNodeTest[nodeAmount];
    //    for(int x = 0; x < nodeAmount; x++)
    //    {
    //        nodes[x].identifier = x;
    //        nodes[x].displayString = description[x];
    //    }
    //}
}
