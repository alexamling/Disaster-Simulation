using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class advisorTracker : MonoBehaviour
{
    public PlayerControls playerController;
    public Text[] tipMessages = new Text[3];

    private PlayerObjective curSelected;

    public float lowThreshhold = 100; //values below are low severity
    public float highThreshhold = 200; //values above are high severity, between thresholds is medium

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController.selectedObjective != null)
        {
            if (playerController.selectedObjective != curSelected)
            {
                UpdateNotes(4);
            }

            curSelected = playerController.selectedObjective;
        }
    }

    public void UpdateNotes(int noteToUpdate)
    {
        switch (noteToUpdate)
        {
            case 0: //planning hints to severity
                if (playerController.selectedObjective.score <= lowThreshhold)
                {
                    tipMessages[0].text = "The current incident is likely low in severity. We don't expect major reprucussions even if it is not swiftly dealt with.";
                }

                else if (playerController.selectedObjective.score > lowThreshhold && playerController.selectedObjective.score <= highThreshhold)
                {
                    tipMessages[0].text = "The current incident is likely of moderate severity. Major reprucussions may occur.";
                }

                else if (playerController.selectedObjective.score > highThreshhold)
                {
                    tipMessages[0].text = "The current situation is severe. Proceed with caution.";
                }

                break;

            case 1: //logistics hints to effective units
                int logRange = Random.Range(0, 99);
                float[] bestMod = new float[] { 0, 0 }; //index, value
                float[] backupMod = new float[] { 0, 0 }; //index, value
                float[] backupModifiers = new float[playerController.selectedObjective.delayedResponseModifiers.Length];

                //Parse the modifiers and find the largest one, store it's index location and value in bestMod
                for (int i = 0; i < playerController.selectedObjective.delayedResponseModifiers.Length; i++)
                {
                    if (playerController.selectedObjective.delayedResponseModifiers[i] > bestMod[1])
                    {
                        bestMod[1] = playerController.selectedObjective.delayedResponseModifiers[i];
                        bestMod[0] = i;
                    }
                }

                //Parse again and create a copy of the modifier list in backupModifiers, replace bestMod's value with 0
                for (int i = 0; i < playerController.selectedObjective.delayedResponseModifiers.Length; i++)
                {
                    backupModifiers[i] = playerController.selectedObjective.delayedResponseModifiers[i];
                    if (i == bestMod[0])
                    {
                        backupModifiers[i] = 0;
                    }
                }

                //Parse the new array of modifiers (with the previous higest value now set to 0) to find the (2nd) highest value
                for (int i = 0; i < backupModifiers.Length; i++)
                {
                    if (backupModifiers[i] > backupMod[1])
                    {
                        backupMod[1] = backupModifiers[i];
                        backupMod[0] = i;
                    }
                }

                switch (logRange)
                {
                    case int n when (n >= 0 && n < 10):
                        switch (backupMod[0])
                        {
                            case 0:
                                tipMessages[1].text = "The unit type most effective for this incident is likely EMS";
                                break;
                            case 1:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Fire Dept.";
                                break;
                            case 2:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Military";
                                break;
                            case 3:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Police";
                                break;
                            case 4:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Volunteer Groups";
                                break;
                        }

                        break;

                    case int n when (n >= 10 && n < 100):
                        switch (bestMod[0])
                        {
                            case 0:
                                tipMessages[1].text = "The unit type most effective for this incident is likely EMS";
                                break;
                            case 1:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Fire Dept.";
                                break;
                            case 2:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Military";
                                break;
                            case 3:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Police";
                                break;
                            case 4:
                                tipMessages[1].text = "The unit type most effective for this incident is likely Volunteer Groups";
                                break;
                        }

                        break;
                }

                break;

            case 2: //operations hints to needs response or not
                int range = Random.Range(0, 9);

                if (playerController.selectedObjective.needsResponse)
                {
                    switch (range)
                    {
                        case int n when (n >= 0 && n < 1): //10% chance to be wrong
                            {
                                tipMessages[2].text = "Local units appear to have things under control, it may be better to ignore this incident and move on.";
                                break;
                            }

                        case int n when (n >= 1 && n < 6): //40% chance to be right, vague
                            {
                                tipMessages[2].text = "Local units are stetched thin. They may have things covered, but additional resources should be sent if available.";
                                break;
                            }

                        case int n when (n >= 6 && n < 10): //40% chance to be right, certain
                            {
                                tipMessages[2].text = "Local units are overwhelmed and unable to resolve the situation. Additinal aid is needed.";
                                break;
                            }
                    }
                }

                else if (!playerController.selectedObjective.needsResponse)
                {
                    switch (range)
                    {
                        case int n when (n >= 0 && n < 1): //10% chance to be wrong
                            {
                                tipMessages[2].text = "The incident seems to be too much for local units to handle, you are advised to send support";
                                break;
                            }

                        case int n when (n >= 1 && n < 6): //40% chance to be right, vague
                            {
                                tipMessages[2].text = "Local units appear to have things covered; it's likely best to ignore this and move on.";
                                break;
                            }

                        case int n when (n >= 6 && n < 10): //40% chance to be right, certain
                            {
                                tipMessages[2].text = "Local units have got things covered; it's best to ignore this and move on.";
                                break;
                            }
                    }
                }

                break;

            case 4:
                for (int i = 0; i < tipMessages.Length; i++)
                {
                    tipMessages[i].text = "Contact the associated advisor for more information.";
                }
                break;

            default:
                break;
        }
    }
}
