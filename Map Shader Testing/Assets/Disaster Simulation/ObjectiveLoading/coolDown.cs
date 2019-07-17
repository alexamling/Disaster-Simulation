using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coolDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Image>().fillAmount == 1)
        {
            var color = gameObject.GetComponent<Button>().colors;
            color.normalColor = new Color(0.5896226f, 1.0f, 0.5907668f, 1.0f);
            color.selectedColor = new Color(0.5896226f, 1.0f, 0.5907668f, 1.0f);
            color.pressedColor = new Color(0.5896226f, 1.0f, 0.5907668f, 1.0f);
            gameObject.GetComponent<Button>().colors = color;
        }
        else
        {
            var color = gameObject.GetComponent<Button>().colors;
            color.normalColor = new Color(0.3207f, 0.3207f, 0.3207f, 1.0f);
            color.selectedColor = new Color(0.3207f, 0.3207f, 0.3207f, 1.0f);
            color.pressedColor = new Color(0.3207f, 0.3207f, 0.3207f, 1.0f);
            gameObject.GetComponent<Button>().colors = color;
        }
    }

    public void removeFill()
    {
        if (gameObject.GetComponent<Image>().fillAmount == 1)
            gameObject.GetComponent<Image>().fillAmount = 0;
    }
}
