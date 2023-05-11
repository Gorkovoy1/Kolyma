using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    public string text;
    public Dialogue dialogue;
    public TextMeshProUGUI buttonText;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        buttonText.text = text;
        dialogue.index += 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
