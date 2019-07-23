using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryNodeTest2 : MonoBehaviour
{
    public int identifier;
    public string[] choices;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    displayString = "";
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    displayString = "";
    //}

    public void SetUp(string[] chces, int id)
    {
        choices = chces;
        identifier = id;
    }
}
