using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGameLog : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public static CardGameLog Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddToLog(string newText)
    {
        Text.text = "-" + newText + "\n" + Text.text;
    }

    public void ClearLog()
    {
        Text.text = "";
    }
}
