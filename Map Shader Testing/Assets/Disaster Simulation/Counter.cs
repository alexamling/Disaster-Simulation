using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public int value;

    private Text number;

    // Start is called before the first frame update
    void Start()
    {
        number = GetComponent<Text>();
    }

    public void Increase()
    {
        value++;
        UpdateText();
    }

    public void Decrease()
    {
        value--;
        UpdateText();
    }

    void UpdateText()
    {
        number.text = "" + value;
    }
}
