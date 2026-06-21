using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopEnd : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lostText;
    // Start is called before the first frame update
    void Start()
    {
        lostText.text = "You Survived:\n" + UIPanelManager.instance.weekNumber + " Weeks";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
