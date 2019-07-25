using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mutualAidManager : MonoBehaviour
{
    public enum AidState { Standby, Arrived, Sending, Returning, Inactive };
    public AidState aidState = AidState.Standby;

    private int[] unitRangesA = new int[10]; //units to send ranges for county A
    private int[] unitRangesB = new int[10]; //units to send ranges for county B
    private int[] unitRangesC = new int[10]; //units to send ranges for county C
    public int[] selectedRange; //Currently selected range, based on dropdown input
    public int[] unitsToAdd = new int[] { 0, 0, 0, 0, 0 }; //Aid units to add to overall unit count

    public List<int[]> blackList = new List<int[]>();

    public Text description;
    public Text rangesText;
    public Dropdown dropdown;
    public Button requestButton;

    public int minUnitRange = 0; //minimum possible available units to recieve
    public int maxUnitRange = 3; //maximum possible available units to recieve
    public int unitRangeVariance = 1; //amount the individual range minimums may be grater than the global minimum

    public Image aidCar;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public float step = 0;
    public float travelTimeRoundTrip = 30; //travel time of the car in seconds

    public ManageUnits unitManager;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRanges(unitRangesA);
        GenerateRanges(unitRangesB);
        GenerateRanges(unitRangesC);

        selectedRange = unitRangesA; //Display initial range (default of dropdown)
        DisplayUnits();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (aidState != AidState.Inactive)
        {
            if (aidState == AidState.Standby)
            {
                for (int i = 0; i < blackList.Count; i++)
                {
                    if (selectedRange == blackList[i])
                    {
                        requestButton.interactable = false;
                    }
                }
            }

            else if (aidState == AidState.Returning)
            {
                Vector3 newPosition = new Vector3(Vector3.Lerp(endPoint, startPoint, step).x, aidCar.transform.localPosition.y, aidCar.transform.localPosition.z);
                aidCar.transform.localPosition = newPosition;
                step += 1 / ((1 / Time.fixedDeltaTime) * (travelTimeRoundTrip / 2));

                if (aidCar.transform.localPosition.x == startPoint.x)
                {
                    dropdown.interactable = true;
                    requestButton.interactable = true;
                    step = 0;
                    aidCar.transform.Rotate(new Vector3(0, 1, 0), 180); //flip sprite for standby
                    DisplayUnits();
                    aidState = AidState.Standby;
                }
            }

            else if (aidState == AidState.Sending)
            {
                Vector3 newPosition = new Vector3(Vector3.Lerp(startPoint, endPoint, step).x, aidCar.transform.localPosition.y, aidCar.transform.localPosition.z);
                aidCar.transform.localPosition = newPosition;
                step += 1 / ((1 / Time.fixedDeltaTime) * (travelTimeRoundTrip / 2));

                if (aidCar.transform.localPosition.x == endPoint.x)
                {
                    aidState = AidState.Arrived;
                }
            }

            else if (aidState == AidState.Arrived)
            {
                for (int i = 0; i < unitsToAdd.Length; i++)
                {
                    unitManager.availibleUnits[i] += unitsToAdd[i];
                    unitManager.resourceValues[i].text = "" + unitManager.availibleUnits[i];
                }

                aidCar.transform.Rotate(new Vector3(0, 1, 0), 180); //flip sprite for retrun trip
                step = 0;
                aidState = AidState.Returning;
            }
        }
    }

    /// <summary>
    /// Generate the initial ranges that the counties can have
    /// </summary>
    /// <param name="range"></param>
    void GenerateRanges(int[] range)
    {
        for (int i = 0; i < range.Length; i++)
        {
            if (i % 2 == 0) //if even index (low range)
            {
                range[i] = minUnitRange + Random.Range(unitRangeVariance * -1, unitRangeVariance + 1); //rnage is minimum with variance
                if (range[i] < minUnitRange) //if given range would be less than the min, set to the min
                {
                    range[i] = minUnitRange;
                }
            }

            else if (i % 2 != 0) //if odd index (high range)
            {
                range[i] = Random.Range(range[i - 1], maxUnitRange + 1);
            }
        }
    }

    /// <summary>
    /// Generate what units will be sent given the range of the current county
    /// </summary>
    void PopulateUnits()
    {
        for (int i = 0; i < unitsToAdd.Length; i++)
        {
            unitsToAdd[i] = Random.Range(selectedRange[i * 2], selectedRange[i * 2 + 1]);
        }
    }

    /// <summary>
    /// Display the current ranges for the currently selected county
    /// </summary>
    public void DisplayUnits()
    {
        int[] range = selectedRange;
        rangesText.text = range[0] + " - " + range[1] + "       " + range[2] + " - " + range[3] + "    " + range[4] + " - " + range[5] + "      " + range[6] + " - " + range[7] + "     " + range[8] + " - " + range[9];
    }

    /// <summary>
    /// Connected to request button, populates units and sends them
    /// </summary>
    public void Request()
    {
        blackList.Add(selectedRange); //add the currently slected range to the blacklist (not to be used again)
        PopulateUnits();
        for (int i = 0; i < selectedRange.Length; i++)
        {
            selectedRange[i] = 0; //set the used range's new values all to 0
        }
        aidState = AidState.Sending; //send the car
        dropdown.interactable = false;
        requestButton.interactable = false;
    }

    /// <summary>
    /// Change the current selected range as an onChange event of the dropdown
    /// </summary>
    public void UpdateSlectedRange()
    {
        if (dropdown.value == 0)
        {
            selectedRange = unitRangesA;
        }
        else if (dropdown.value == 1)
        {
            selectedRange = unitRangesB;
        }
        else if (dropdown.value == 2)
        {
            selectedRange = unitRangesC;
        }

        requestButton.interactable = true;
    }
}
