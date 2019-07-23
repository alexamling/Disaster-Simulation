using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest3
{
    // Local values used to 
    int part;
    
    public SecondaryNodeTest3[] otherNodes;
    public string[] nodesDisplay;
    public string mainDisplay;

    public string[] choices;
    string[] options;
    string[] results;
    string[] identifiers;

    public NodeTest3(List<List<string[]>> allItems)
    {
        part = 0;
        mainDisplay = allItems[part][0][0];

        //Debug.Log(nodesDisplay.Length);
        choices = allItems[part][1];

        options = new string[3];
        results = new string[3];
        identifiers = new string[3];

        //string[] local;
        //for(int x = 0; x < choices.Length; x++)
        //{
        //    local = choices[x].Split('#');
        //    options[x] = local[0];
        //    results[x] = local[1];
        //    identifiers[x] = local[2];
        //}

        //part++;\
        //Debug.Log(allItems.Count);

        //for(int x = 0; x < allItems.Count; x++)
        //{
        //    Debug.Log("Level: " + (x + 1));
        //    for(int y = 0; y < allItems[x].Count; y++)
        //    {
        //        Debug.Log("Section: " + (y + 1));
        //        for(int z = 0; z < allItems[x][y].Length; z++)
        //        {
        //            Debug.Log(allItems[x][y][z]);
        //        }
        //    }
        //}

        if (part < allItems.Count)
        {
            otherNodes = new SecondaryNodeTest3[allItems[part+1].Count];
            for (int x = 0; x < otherNodes.Length; x++)
            {

                otherNodes[x] = new SecondaryNodeTest3(allItems, part+1, x * 2, allItems[part][part+1][x], mainDisplay);

            }

            //Debug.Log(otherNodes.Length);
        }
    }

    public void Print()
    {
        //Debug.Log(mainDisplay);
        for(int x = 0; x < choices.Length; x++)
        {
            //Debug.Log(choices[x]);
            //Debug.Log(options[x]);
            //Debug.Log(results[x]);
            //Debug.Log(identifiers[x]);
        }
        //Debug.Log("----");
        //Debug.Log("----");
        for (int x = 0; x < otherNodes.Length; x++)
        {
            otherNodes[x].Print();
        }
    }
}
