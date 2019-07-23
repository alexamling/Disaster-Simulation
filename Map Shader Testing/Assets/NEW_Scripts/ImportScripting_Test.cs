using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImportScripting_Test : MonoBehaviour
{
    // Dictionary that holds the name of a file and an 
    // array of strings with it's lines
    Dictionary<string, string[]> directionValues;

    public List<InjectFlow> injects;

    // Start is called before the first frame update
    void Awake()
    {
        injects = new List<InjectFlow>();
        // Instantiates dictionary
        directionValues = new Dictionary<string, string[]>();

        // Stores path to search for files and gets directory info from it
        string path = Application.dataPath + "/InGameText";
        DirectoryInfo textDirectory = new DirectoryInfo(path);

        // Checks if the directory exists or not
        if(textDirectory != null)
        {
            // Grabs all the files in the directory
            FileInfo[] data = textDirectory.GetFiles("*.*");

            // Goes through each of the files and skips the meta files 
            for (int x = 0; x < data.Length; x+=2)
            {
                // Takes file name and removes file extension for readability
                string name = data[x].Name.Replace(".txt", "");

                // Transform list into array and add it to the dictionary
                // with the name string as they key and array as the value
                directionValues.Add(name, File.ReadAllLines(path + "/" + data[x].Name));
            }
        }
        else // If directory can not be found, put error message in debug
        {
            Debug.Log("Error - No InGameText folder detected");
        }

        // Parses and adds types of injects to a list
        foreach(KeyValuePair<string, string[]> entry in directionValues)
        {
            injects.Add(new InjectFlow(directionValues[entry.Key], 0));
        }
    }
}
