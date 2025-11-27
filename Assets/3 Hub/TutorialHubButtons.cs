using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialHubButtons : MonoBehaviour
{
    public TextMeshProUGUI message;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject elsewhereButton;

    public string messageText;
    // Start is called before the first frame update
    void Start()
    {
        elsewhereButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(message.text == messageText) //change this according to scene
        {
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            elsewhereButton.gameObject.SetActive(false);
        }
        else
        {
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
            elsewhereButton.gameObject.SetActive(true);
        }
    }
}
