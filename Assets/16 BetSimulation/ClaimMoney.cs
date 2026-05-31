using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClaimMoney : MonoBehaviour
{
    private int amount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyButton()
    {
        Destroy(this.gameObject);
    }

    public void SetMoney(int money)
    {
        amount = money;
        this.GetComponentInChildren<TextMeshProUGUI>().text = "" + money;
    }

    public void AddPotMoney()
    {
        MoneyController.instance.AddMoney(amount);
    }
}
