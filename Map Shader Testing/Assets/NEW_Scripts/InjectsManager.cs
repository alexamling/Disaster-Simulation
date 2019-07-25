using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InjectsManager : MonoBehaviour
{
    // List of all the injects able to be used
    List<InjectNode> injects;

    // In scene objects to be modified
    public GameObject display;
    public Text mainText;
    public List<GameObject> buttons;

    // Node that manages the current Inject
    InjectNode currentNode;

    // Booleans used to work through inject logic
    bool started;
    bool selected;

    // Local value that holds the chosen interval value
    int chosenValue;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up local values defaults
        injects = GetComponent<ImportScript>().injects;
        started = false;
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Small chance to activate inject based on random number and if an inject is already started
        //if (Random.Range(0f, 1f) > .99f && started == false)
            //StartInject();
    }

    /// STARTINJECT
    /// Description:
    /// Prepares the inject for user input.  Sets up buttons and text
    /// to use for traversing the injects.
    public void StartInject(float delay, float delayVariance)
    {
        // Changes started to true in order to prevent a new inject from overwriting the current one
        started = true;

        // Turns display on so user can see first phase of inject
        display.SetActive(true);

        // Sets current node to a random inject from the list
        currentNode = injects[Random.Range(0, injects.Count - 1)];

        // Set thte main text of the UI to reflect that of the starting text of the inject
        mainText.text = currentNode.main;

        // Sets up choices for user based on the numnber of choices in the array for the Inject node
        for(int x = 0; x < currentNode.numChoices; x++)
        {
            buttons[x].GetComponentInChildren<Text>().text = currentNode.choices[x];
            buttons[x].SetActive(true);
        }

        // Start the inject
        StartCoroutine(ProcessInject(delay, delayVariance));
    }

    /// PROCESSINJECT
    /// COROUTINE
    /// Description:
    /// Goes through each of the sections of the inject and makes sure there is user input
    IEnumerator ProcessInject(float delay, float delayVariance)
    {
        // Wait until an option has been chosen.  It changes selected to true and allows the routine to continue
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        ProcessChanges(chosenValue);
        selected = false;
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        ProcessChanges(chosenValue);
        selected = false;
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));
        ProcessChanges(chosenValue);
        selected = false;
        yield return new WaitUntil(() => selected == true);
        yield return new WaitForSeconds(delay + Random.Range(-delayVariance, delayVariance));

        // Resets values for buttons after inject ends
        for (int x = 0; x < buttons.Count; x++)
            buttons[x].GetComponent<ButtonValues>().Reset();

        selected = false;
        started = false;
        yield return null;
    }

    /// PROCESSCHANGES
    /// Description:
    /// Takes the user input and uses it to change the next steps for the inject
    void ProcessChanges(int value)
    {
        // Activates display for user to see the next options
        display.SetActive(true);

        // Sets the canvas text to the previously chosen result
        mainText.text = currentNode.results[value];

        // Checks if the inject is in it's final phase or not
        if (currentNode.localPart < currentNode.localMax)
        {
            // Changes current node to the next node in line
            currentNode = currentNode.nextNode;

            // Local array to hold new set of intervals
            string[] local = currentNode.intervals[value].Split('^');

            // Local value for handling changing button data
            int index = 0;

            // Go through, change the text and value of each of the buttons and activate it for user to interact
            for (int x = int.Parse(local[0]); x < int.Parse(local[1]); x++)
            {
                buttons[index].GetComponentInChildren<Text>().text = currentNode.choices[x];
                buttons[index].GetComponent<ButtonValues>().value = x;
                buttons[index].SetActive(true);
                index++;
            }
        }
        else  // If the final section is finished, display final results and continue button
        {
            display.SetActive(true);
            buttons[0].SetActive(true);
            buttons[0].GetComponentInChildren<Text>().text = "Continue";
        }
        
    }

    /// GETINPUT
    /// Description:
    /// When button is clicked, check its value, set it to chosenValue and
    /// turn display off in preparation for the next section of the inject
    public void GetInput(ButtonValues value)
    {
        // Changes selected to true in order to continue
        selected = true;

        // Sets chosenValue used to set up intervals for choices
        chosenValue = value.value;

        // Sets UI elements to be inactive
        for (int x = 0; x < buttons.Count; x++)
            buttons[x].SetActive(false);

        display.SetActive(false);
    }
}
