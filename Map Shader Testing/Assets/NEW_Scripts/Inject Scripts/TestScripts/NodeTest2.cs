using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest2 : MonoBehaviour
{
    public string displayString;
    public SecondaryNodeTest2[] secondNodes;

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SetUp(List<string[]> injectParts, int part)
    {
        displayString = injectParts[part][0];
        List<string> localValues = new List<string>();
        secondNodes = new SecondaryNodeTest2[injectParts[part].Length - 1];
        part++;
        int index = 0;

        for (int x = 0; x < injectParts[part].Length; x++)
        {
            if (injectParts[part][x].Contains("^"))
            {
                for (int y = 1; y < int.Parse(injectParts[part][x][1] + "") + 1; y++)
                {
                    localValues.Add(injectParts[part][x + y]);
                }

                secondNodes[index] = new SecondaryNodeTest2();
                secondNodes[index].SetUp(localValues.ToArray(), index + 1);
                index++;
                localValues = new List<string>();
            }
        }
    }
}
