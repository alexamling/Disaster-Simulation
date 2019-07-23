using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node4 : MonoBehaviour
{
    public Node4 nextNode;
    public List<string[]> choiceResult;
    string displayChoice;
    string displayResult;

    int identifier;
    public string main;
    public bool completed;
    public bool active;
    public int localPart;
    public int localMax;
    
    public Node4(string[] injects, int part)
    {
        nextNode = null;
        localMax = injects.Length - 2;
        active = false;
        completed = false;
        localPart = part;

        if(part == 0)
            main = injects[injects.Length - 1];

        choiceResult = new List<string[]>();
        string[] local = injects[part].Split('_');

        for(int x = 0; x < local.Length; x++)
        {
            choiceResult.Add(local[x].Split('^'));
        }

        //Debug.Log(main);
        //for (int x = 0; x < choiceResult.Count; x++)
        //{
        //    for (int y = 0; y < choiceResult[x].Length; y++)
        //    {
        //        Debug.Log(choiceResult[x][y]);
        //    }
        //}
        //Debug.Log("-------");

        part++;
        if (part < injects.Length-1)
        {
            nextNode = new Node4(injects, part);
        }
       
    }

    public void Print()
    {
       
    }
}
