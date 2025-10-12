using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialHubButtons : MonoBehaviour
{
    public TextMeshProUGUI message;
    public GameObject yesButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(message.text == "Travel to Barracks?")
        {
            yesButton.gameObject.SetActive(true); 
        }
        else
        {
            yesButton.gameObject.SetActive(false);
        }
    }
}
