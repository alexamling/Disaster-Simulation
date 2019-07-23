using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTwo4 : MonoBehaviour
{
    public string[] choicesResults;
    public NodeTwo4[] otherNodes;
    public bool completed;

    public NodeTwo4(string[] inject, string chRe, int part, int max)
    {
        completed = false;
        if(part < max)
        {
            choicesResults = chRe.Split('^');
            part++;

            string[] next = inject[part].Split('_');
            otherNodes = new NodeTwo4[next.Length];

            for (int x = 0; x < next.Length; x++)
            {
                otherNodes[x] = new NodeTwo4(inject, next[x], part, max);
            }
        }
        
    }

    public void Print()
    {
        if (otherNodes != null)
        {


            for (int x = 0; x < choicesResults.Length; x++)
            {
               // Debug.Log(choicesResults[x]);
            }
        }
        //Debug.Log("*");
    }
}
