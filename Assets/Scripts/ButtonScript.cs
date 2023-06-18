using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonScript : MonoBehaviour
{
    public string choice1;
    public string choice2;
    public string choice3;

    public Dialogue1 dialogue1;

    public Button firstChoice;
    public Button secondChoice;
    public Button thirdChoice;


    void Start()
    {
        choice1 = "";
        choice2 = "";
        choice3 = "";
    }

    void Update()
    {
        // Change the text of the button
        firstChoice.GetComponentInChildren<Text>().text = choice1;
        secondChoice.GetComponentInChildren<Text>().text = choice2;
        thirdChoice.GetComponentInChildren<Text>().text = choice3;
    }


}