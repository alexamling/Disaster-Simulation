using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TertiaryNodeTest : MonoBehaviour
{
    public int identifier;
    public string displayString;

    public TertiaryNodeTest(List<string[]> injectParts)
    {
        displayString = injectParts[2][0];
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
