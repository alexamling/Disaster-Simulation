using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryNodeTest3
{
    public string[] choices;
    public string[] nodesDisplay;
    public int localPart;
    public int localMax;
    public SecondaryNodeTest3[] otherNodes;
    //public string mainDisplay;
    int option1;
    int option2;

    public int identifier;
    public string mainDisplay;
    public string resultDisplay;

    string[] options;
    string[] results;
    string[] identifiers;

    public SecondaryNodeTest3(List<List<string[]>> allItems, int part, int nextOptions, string choices, string main)
    {
        //localPart = part;
        //choices = allItems[localPart][0];
        //localPart++;

        option1 = nextOptions;
        option2 = nextOptions+1;

        //options = new string[3];
        //results = new string[3];
        //identifiers = new string[3];

        string[] local = choices.Split('#');
        //Debug.Log(choices);
        mainDisplay = local[0];
        resultDisplay = local[1];
        identifier = int.Parse(local[2]);
        //Debug.Log("");
        //Debug.Log(mainDisplay);
        //Debug.Log(resultDisplay);
        //Debug.Log(identifier);

        localPart = part;
        for (int x = 0; x < choices.Length; x++)
        {
            //local = choices[x].Split('#');
            //options[x] = local[0];
            //results[x] = local[1];
            //identifiers[x] = local[2];
        }


        //Debug.Log("A: " + option1);
        //Debug.Log("B: " + option2);
        //Debug.Log(localPart);
        //Debug.Log(allItems.Count);

        if (part < allItems.Count)
        {
            
           
            otherNodes = new SecondaryNodeTest3[allItems[part][nextOptions / 2].Length];

            for (int x = 0; x < otherNodes.Length; x++)
            {
                //    Debug.Log(part);
                //    Debug.Log(nextOptions);
                //    Debug.Log(x);
                //    Debug.Log(allItems[part][nextOptions / 2][x]);
                //otherNodes[x] = new SecondaryNodeTest3(allItems, part+1, (nextOptions + x), allItems[part][nextOptions / 2][x], main);
                Debug.Log("ID: " + (identifier - 1));
                otherNodes[x] = new SecondaryNodeTest3(allItems, part + 1, (nextOptions + x), allItems[part][identifier][x], main);
                Debug.Log(main);
                //Debug.Log("Next: " + nextOptions / 2);
                //Debug.Log("Part: " + part);
                //Debug.Log(allItems[part][nextOptions / 2][x]);
                for (int y = 0; y < allItems[part][nextOptions / 2].Length; y++)
                {
                    //Debug.Log(allItems[part][nextOptions / 2][y]);
                }
                Debug.Log(otherNodes[x].mainDisplay);
                Debug.Log(otherNodes[x].resultDisplay);
                Debug.Log(otherNodes[x].identifier);
            }
            //Debug.Log(allItems[part][nextOptions].Length);



            //otherNodes[0] = new SecondaryNodeTest3(allItems, localPart, nextOptions);
            //otherNodes[1] = new SecondaryNodeTest3(allItems, localPart, nextOptions * 2, );

            //for (int x = 0; x < otherNodes[0].choices.Length; x++)
            //{
            //    Debug.Log(otherNodes[0].choices[x]);
            //}
            //for (int x = 0; x < otherNodes[1].choices.Length; x++)
            //{
            //    Debug.Log(otherNodes[1].choices[x]);
            //}
        }
    }

    public void Print()
    {

        //for (int x = 0; x < 3; x++)
        //{
        //if (x == 0)
        //    Debug.Log(nodesDisplay[identifier - 1] + " : " + choices[x]);
        //else
        //    Debug.Log(nodesDisplay[identifier] + " : " + choices[x]);

        //Debug.Log(choices[x]);
        //Debug.Log(mainDisplay);
        //Debug.Log(resultDisplay);
        //Debug.Log(identifier);
        ////}
        Debug.Log("----");

        if (localPart == localMax+2)
        {
            for (int x = 0; x < otherNodes.Length; x++)
            {
                otherNodes[x].Print();
            }
        }
        
    }
}
