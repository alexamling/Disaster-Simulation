using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
/*
 *NSF REU SERIOUS GEOGAME RESEARCH 
 *Elliot Privateer
 *2nd Year
 *Manager class that takes into account all injects and functions for using injects in game
*/
public class InjectManager : MonoBehaviour
{
    // List of possible injects and placeholder for currently active inject
    public List<InjectFlow> injects;
    InjectFlow currentInject;

    // Holds indices for determining where in the choice results
    // the display strings should be pulling from.
    int[] currentIndex;

    // In scene gameobjects manipulated by this script
    public GameObject injectCanvas;
    public Text mainText;
    public GameObject buttonHolder;
    public List<GameObject> buttons;

    // Booleans used to start and end injects
    bool started = false;
    bool selected = false;

    //// Start is called before the first frame update
    void Start()
    {
        // Sets inject blueprints to a local list
        injects = GetComponent<ImportScripting_Test>().injects;

        // Instantiates array
        currentIndex = new int[2];
    }

    /// STARTINJECT
    /// Description:
    /// Begins inject by creating a random one, setting it to a local class value,
    /// and calls the function that handles the visual portion of the inject.
    public void StartInject()
    {
        // Sets current inject to a random inject type
        currentInject = injects[Random.Range(0, injects.Count)];
        SetMainText();

        injectCanvas.SetActive(true);
    }

    /// ONGOINGINJECT
    /// Description:
    /// Called for each portion of the inject.  Handles the display and choice of
    /// options in the inject.
    public void OngoingInject()
    {
        // Gives user options
        CheckNodeOptions();
    }

    /// SETMAINTEXT
    /// Description:
    /// Sets the main display text of the inject based on user choice.
    void SetMainText()
    {
        // Grabs the main text from the current state of the inject and sets it to main display texts
        mainText.text = currentInject.main;
    }

    /// CHECKNODEOPTIONS
    /// Description:
    /// Displays the options based on the start of the inject
    /// and changes based on choices made by user.
    void CheckNodeOptions()
    {
        // Sets distance of plane back to normal distance to see
        gameObject.GetComponent<Canvas>().planeDistance = 100f;

        // Displays the options at the start of the inject
        if (currentInject.localPart == 0)
        {
            if (currentInject.choiceResult.Count == 1)
            {
                buttons[0].SetActive(true);
            }
            else if (currentInject.choiceResult.Count == 2)
            {
                buttons[0].SetActive(true);
                buttons[1].SetActive(true);
            }
            else if (currentInject.choiceResult.Count == 3)
            {
                buttons[0].SetActive(true);
                buttons[1].SetActive(true);
                buttons[2].SetActive(true);
            }

            // Sets content for each of the buttons and makes sure their scripts have the appropriate values attached
            for (int x = 0; x < currentInject.choiceResult.Count; x++)
            {
                buttons[x].GetComponentInChildren<Text>().text = currentInject.choiceResult[x][0];
                buttons[x].GetComponent<ButtonValues>().value = x;
            }
        }
        else  // Checks and replaces display and buttons based on user inputs
        {
            int index = 0;

            // If there's two valid elements in the index array, go from one indice to the next to determine display
            if(currentIndex[1] != -1)
            {
                for (int x = currentIndex[0]; x <= currentIndex[1]; x++)
                {
                    buttons[index].SetActive(true);
                    buttons[index].GetComponentInChildren<Text>().text = currentInject.choiceResult[x][0];
                    buttons[index].GetComponent<ButtonValues>().value = x;
                    index++;
                }
            }
            else // If there's only one valud element in the array, set the display based on that single value
            {
                buttons[0].SetActive(true);
                buttons[index].GetComponentInChildren<Text>().text = currentInject.choiceResult[currentIndex[0]][0];
                buttons[index].GetComponent<ButtonValues>().value = currentIndex[0];
            }
            
        }


    }

    /// GETINPUT
    /// Desciption:
    /// Gets the input from the user input from the buttons,
    /// resets active buttons, sets next portion of inject,
    /// and sets up next portion of the inject.
    public void GetInput(ButtonValues value)
    {
        // Local value used to find the proper index based on user input
        int index = value.value;
        
        // Determines which indices need to be checked for determining choices and results
        currentIndex[0] = -1;
        currentIndex[1] = -1;

        // Sets main display to display the result based on user choice
        mainText.text = currentInject.choiceResult[index][1];

        // Depending on the nunber of indeces parsed out of the inject nodes,
        // set currentIndex values to the different indeces parsed from the strings
        if (currentInject.choiceResult[index].Length == 3)
            currentIndex[0] = int.Parse(currentInject.choiceResult[index][2]);
        else if (currentInject.choiceResult[index].Length == 4)
        {
            currentIndex[0] = int.Parse(currentInject.choiceResult[index][2]);
            currentIndex[1] = int.Parse(currentInject.choiceResult[index][3]);
        }

        // Resets all buttons to deactivate so they can reactivate the proper buttons
        for (int x = 0; x < buttons.Count; x++)
        {
            buttons[x].SetActive(false);
        }

        // Checks to see if the current section of the inject is the final section.
        // If not, continue through to the next section.
        if(currentInject.localPart < currentInject.localMax)
        {
            // Goes to the next node in the line
            currentInject = currentInject.nextNode;

            // Changes selected to true in order to continue the inject coroutine
            selected = true;

            // Sets plane distance behind the camera for the sake of hiding UI without turning it off
            gameObject.GetComponent<Canvas>().planeDistance = -100f;
        }
        else  // If the current section is the final section, change 
            selected = true;
    }

    void Update()
    {

    }

    // Determine if an inject should fire or not
    public IEnumerator RunInject(float delay, float delayVariance)
    {
        // Sets up each of the parts of the inject and waits till the
        // yield requirements are met.  Based on both time and and logic
        // from user input.
        //Debug.Log("Inject Activated");
        started = true;
        StartInject();
        OngoingInject();
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance,delayVariance));
        selected = false;
        //Debug.Log("-1-");
        OngoingInject();
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        selected = false;
        //Debug.Log("-2-");
        OngoingInject();
           
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        selected = false;
        //Debug.Log("Heck");
        buttons[0].SetActive(true);
        buttons[0].GetComponentInChildren<Text>().text = "Continue";

        //Debug.Log("-3-");
        yield return new WaitUntil(() => selected == true);
        injectCanvas.SetActive(false);
        selected = false;

        // Resets 'started' to false in preparation for a new inject
        started = false;
        yield return null;
    }
}
