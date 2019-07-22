using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class objectiveReader : MonoBehaviour
{
    public TextAsset ObjectivesFire;
    public TextAsset ObjectivesFlood;
    public TextAsset ObjectivesAccident;
    public TextAsset ObjectivesEvac;

    public List<PlayerObjective> fireList;
    public List<PlayerObjective> floodList;
    public List<PlayerObjective> accidentList;
    public List<PlayerObjective> evacList;

    public GameObject objectivePrefab;

    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        readFile(ObjectivesFire, fireList);
        readFile(ObjectivesFlood, floodList);
        readFile(ObjectivesAccident, accidentList);
        readFile(ObjectivesEvac, evacList);
    }

    public void readFile(TextAsset textFile, List<PlayerObjective> list)
    {
        string[] newStrings = textFile.text.Split(new String[] { "Score: ", "Location: ", "ImmediateResponseModifiers: ", "DelayedResponseModifiers: ", "NotificationTitle: ", "FullMessage: ", "TimeLimit: ", "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
        string newString = String.Join("", newStrings);

       
        //string[] mainObjectives = textFile.text.Split(new string[] {"|" }, StringSplitOptions.RemoveEmptyEntries);
        string[] mainObjectives = newString.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string mainObjective in mainObjectives)
        {
            PlayerObjective objective = Instantiate(objectivePrefab).GetComponent<PlayerObjective>();
            objective.transform.SetParent(parent.transform);
            string[] subObjectives = mainObjective.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //objective.score = float.Parse(subObjectives[0]);
            for (int i = 0; i < subObjectives.Length; i++)
            {
                objective.score = float.Parse(subObjectives[0], CultureInfo.InvariantCulture.NumberFormat);
                objective.location = new Vector2(float.Parse(subObjectives[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[2], CultureInfo.InvariantCulture.NumberFormat));
                objective.immediateResponseModifiers = new float[] { float.Parse(subObjectives[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[6], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[7], CultureInfo.InvariantCulture.NumberFormat) };
                objective.delayedResponseModifiers = new float[] { float.Parse(subObjectives[8], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[9], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[10], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[11], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[12], CultureInfo.InvariantCulture.NumberFormat) };
                objective.notificationTitle = subObjectives[13];
                objective.fullMessage = subObjectives[14];
                objective.timeLimit = float.Parse(subObjectives[15], CultureInfo.InvariantCulture.NumberFormat);
                objective.units = new int[] { 0, 0, 0, 0, 0 };
            }
            list.Add(objective);
        }
    }
}
