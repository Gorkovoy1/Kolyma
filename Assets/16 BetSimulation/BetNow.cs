using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BetNow : MonoBehaviour
{
    public TextMeshProUGUI moneyBet;
    public GameObject betSlot1;
    public GameObject betSlot2;

    private int money = 0;
    private string item1 = "none";
    private string item2 = "none";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadValues()
    {
        if (moneyBet.text != "")
        {
            money = int.Parse(moneyBet.text);
        }
        else
        {
            money = 0;
        }

        if(betSlot1.GetComponent<BetSlot>().itemToBet != null)
        {
            item1 = betSlot1.GetComponent<BetSlot>().itemToBet.name;
        }
        else
        {
            item1 = "none";
        }

        if (betSlot2.GetComponent<BetSlot>().itemToBet != null)
        {
            item2 = betSlot2.GetComponent<BetSlot>().itemToBet.name;
        }
        else
        {
            item2 = "none";
        }
    }

    public void DisplayBet()
    {
        ReadValues();
        //on click, to test display bet items
        Debug.Log("Betting " + money + " + " + item1 + " + " + item2);
    }
}
