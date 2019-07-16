using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;

public class objectiveReader : MonoBehaviour
{
    public TextAsset testObjectivesFire;
    public TextAsset testObjectivesFlood;

    public List<PlayerObjective> fireList;
    public List<PlayerObjective> floodList;

    // Start is called before the first frame update
    void Start()
    {
        readFile(testObjectivesFire, fireList);
        readFile(testObjectivesFlood, floodList);

        /*Debug.Log("FIRE EVENTS");
        foreach (PlayerObjective obj in fireList)
        {
            Debug.Log(obj.score);
            Debug.Log(obj.location);
            Debug.Log(obj.immediateResponseModifiers.Length);
            Debug.Log(obj.delayedResponseModifiers.Length);
            Debug.Log(obj.notificationTitle);
            Debug.Log(obj.fullMessage);
        }
        Debug.Log("FLOOD EVENTS");
        foreach (PlayerObjective obj in floodList)
        {
            Debug.Log(obj.score);
            Debug.Log(obj.location);
            Debug.Log(obj.immediateResponseModifiers.Length);
            Debug.Log(obj.delayedResponseModifiers.Length);
            Debug.Log(obj.notificationTitle);
            Debug.Log(obj.fullMessage);
        }*/
    }

    public void readFile(TextAsset textFile, List<PlayerObjective> list)
    {
        PlayerObjective objective = new PlayerObjective();
        string[] mainObjectives = textFile.text.Split(new string[] {" /n " }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string mainObjective in mainObjectives)
        {
            string[] subObjectives = mainObjective.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            //objective.score = float.Parse(subObjectives[0]);
            for (int i = 0; i < subObjectives.Length; i++)
            {
                objective.score = float.Parse(subObjectives[0], CultureInfo.InvariantCulture.NumberFormat);
                objective.location = new Vector2(float.Parse(subObjectives[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[2], CultureInfo.InvariantCulture.NumberFormat));
                objective.immediateResponseModifiers = new float[] { float.Parse(subObjectives[3], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[4], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[5], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[6], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[7], CultureInfo.InvariantCulture.NumberFormat) };
                objective.delayedResponseModifiers = new float[] { float.Parse(subObjectives[8], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[9], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[10], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[11], CultureInfo.InvariantCulture.NumberFormat), float.Parse(subObjectives[12], CultureInfo.InvariantCulture.NumberFormat) };
                objective.notificationTitle = subObjectives[13];
                objective.fullMessage = subObjectives[14];
            }
            list.Add(objective);
        }
    }
}
