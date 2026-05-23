using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MoneyController : MonoBehaviour
{
    public static MoneyController instance;
    public int balance;
    public TextMeshProUGUI[] moneyTexts;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadMoney();          //put this back when persistent object
    }

    // Update is called once per frame
    void Update()
    {
        foreach(TextMeshProUGUI moneyText in moneyTexts)
            moneyText.text = "Balance: " + balance.ToString();

    }

    public void AddMoney(int amount)
    {
        balance += amount;
        SaveMoney();
    }

    public bool SpendMoney(int amount)
    {
        if(balance >= amount)
        {
            balance -= amount;
            SaveMoney();
            return true;
        }
        return false;
    }

    void SaveMoney()
    {
        PlayerPrefs.SetInt("balance", balance);
        PlayerPrefs.Save();
    }

    void LoadMoney()
    {
        balance = PlayerPrefs.GetInt("balance", 0);
    }
}
