using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjective : MonoBehaviour
{
    public float status;
    public Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.AddComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
