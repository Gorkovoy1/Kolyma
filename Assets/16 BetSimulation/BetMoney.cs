using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BetMoney : MonoBehaviour
{
    public TextMeshProUGUI betMoney;
    public int originalBalance;

    public Button bet0;
    public Button bet5;
    public Button bet10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(WaitForMoneyController());
    }

    IEnumerator WaitForMoneyController()
    {
        yield return new WaitUntil(() => MoneyController.instance != null);

        originalBalance = MoneyController.instance.balance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Bet5()
    {
        bet0.interactable = true;
        bet5.interactable = false;
        bet10.interactable = true;
        betMoney.text = "5";
        MoneyController.instance.balance = originalBalance - 5;
    }

    public void Bet10()
    {
        bet0.interactable = true;
        bet5.interactable = true;
        bet10.interactable = false;
        betMoney.text = "10";
        MoneyController.instance.balance = originalBalance - 10;
    }

    public void Bet0()
    {
        bet0.interactable = false;
        bet5.interactable = true;
        bet10.interactable = true;
        betMoney.text = "0";
        MoneyController.instance.balance = originalBalance;
    }
}
