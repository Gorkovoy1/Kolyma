using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    public string option1;
    public string option2;
    public string option3;
    public Button button1;
    public Button button2;
    public Button button3;
    public Dialogue dialogue;

    // Start is called before the first frame update
    void Start()
    {
        option1 = "";
        option2 = "";
        option3 = "";
    }


    // Update is called once per frame
    void Update()
    { 
      button1.GetComponentInChildren<Text>().text = option1;
      button2.GetComponentInChildren<Text>().text = option2;
      button3.GetComponentInChildren<Text>().text = option3;
    }
}
