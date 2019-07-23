using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest : MonoBehaviour
{
    //SecondaryNodeTest[] nodes;
    //public string[] injectDescriptions;
    //public string[] displayNodeDescriptions;

    //string displayString;

    public string displayString;

    public SecondaryNodeTest[] secondNodes;
    int part;
    public int max;

    public NodeTest(List<string[]> injectParts)
    {
        part = 0;
        max = int.Parse(injectParts[part][1]);
        displayString = injectParts[part][0];
        List<string> localValues = new List<string>();
        secondNodes = new SecondaryNodeTest[injectParts[part].Length-2];
        part++;
        int index = 0;
        
        for (int x = 0; x < injectParts[part].Length; x++)
        {
            if(injectParts[part][x].Contains("^"))
            {
                for(int y = 1; y < int.Parse(injectParts[part][x][1]+"")+1; y++)
                {
                    localValues.Add(injectParts[part][x + y]);
                }
                
                secondNodes[index] = new SecondaryNodeTest(localValues.ToArray(), index + 1, part, max, injectParts);
                index++;
                localValues = new List<string>();
            }
        }

        
        
    }
}
