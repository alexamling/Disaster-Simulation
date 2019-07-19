using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class advisorTracker : MonoBehaviour
{
    public PlayerControls playerController;
    public GameObject[] tipMessages = new GameObject[3];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateNotes(int noteToUpdate)
    {
        tipMessages[noteToUpdate].GetComponent<Text>().text = playerController.selectedObjective.tipString[noteToUpdate];
    }
}
