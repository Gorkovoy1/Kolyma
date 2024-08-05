using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGameLog : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public static CardGameLog Instance;

    int logNumber;

    private void Awake()
    {
        Instance = this;
    }

    public void AddToLog(string newText)
    {
        logNumber++;
        Text.text = logNumber + ") " + newText + "\n" + Text.text;
        Debug.Log(logNumber + ") " + newText);
    }

    public void ClearLog()
    {
        logNumber = 0;
        Text.text = "";
    }
}
