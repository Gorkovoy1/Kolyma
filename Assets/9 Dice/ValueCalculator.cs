using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueCalculator : MonoBehaviour
{
    public GameObject dice1;
    public GameObject dice2;
    public GameObject dice3;
    public GameObject dice4;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<TextMeshProUGUI>().text = "" + (dice1.GetComponent<DiceController>().topVal + dice2.GetComponent<DiceController>().topVal + dice3.GetComponent<DiceController>().topVal + dice4.GetComponent<DiceController>().topVal);
    }
}
