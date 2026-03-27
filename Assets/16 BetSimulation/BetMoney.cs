using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BetMoney : MonoBehaviour
{
    public TextMeshProUGUI betMoney;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bet5()
    {
        betMoney.text = "5";
    }

    public void Bet10()
    {
        betMoney.text = "10";
    }

    public void Bet0()
    {
        betMoney.text = "0";
    }
}
