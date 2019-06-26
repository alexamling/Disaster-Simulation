using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    public List<Button> buttons;

    float radius = 100;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new List<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display(List<Button> btns)
    {
        // sent old buttons off screen
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = new Vector3(-10000, 0, 0);
        }

        float angleDivision = Mathf.PI / (btns.Count - 1);

        // display new buttons
        for (int i = 0; i < btns.Count; i++)
        {
            float angle = -angleDivision * i + Mathf.PI;
            btns[i].transform.SetParent(this.transform);
            btns[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        buttons = btns;
    }

    public void SetPosition(Vector3 newPos)
    {
        this.GetComponent<RectTransform>().position = newPos;
    }
}
