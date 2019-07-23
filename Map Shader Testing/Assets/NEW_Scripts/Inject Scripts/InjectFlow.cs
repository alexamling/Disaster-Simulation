using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *NSF REU SERIOUS GEOGAME RESEARCH 
 *Elliot Privateer
 *2nd Year
 *Node class that holds data for displaying injects and decisions
*/
public class InjectFlow : MonoBehaviour
{
    // Next connected node for next section of inject
    // List of possible results and choices for user
    public InjectFlow nextNode;
    public List<string[]> choiceResult;

    // Values used to determine displayed text and check levels of inject
    public string main;
    public int localPart;
    public int localMax;

    public InjectFlow(string[] injects, int part)
    {
        // Instantiate values
        choiceResult = new List<string[]>();
        localMax = injects.Length - 2;
        localPart = part;
        string[] local = injects[part].Split('_');

        // Determines if it is the first level of the inject and sets default display text
        if (part == 0)
            main = injects[injects.Length - 1];

        // Splits string that holds both choice and result and places array into list
        for (int x = 0; x < local.Length; x++)
            choiceResult.Add(local[x].Split('^'));

        // Increment part to change level focus and create next attached node
        part++;
        if (part < injects.Length - 1)
            nextNode = new InjectFlow(injects, part);
    }
}
