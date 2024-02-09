using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int gold;
    public TMP_Text goldText;
    public ShopItemSO[] shopItemsSO;
    public GameObject[] shopPanelsGO;
    public ShopItem[] shopPanels;
    public Button[] buyButtons;

    // Start is called before the first frame update
    void Start()
    {
        goldText.text = "Gold: " + gold.ToString(); 
        LoadPanels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddCoins()
    {
        gold++;
        goldText.text = "Gold: " + gold.ToString();
        CheckPurchaseable();
    }

    public void CheckPurchaseable()
    {

    }

    public void LoadPanels()
    {
        for (int i = 0; i < shopItemsSO.Length; i++)
        {
            shopPanels[i].titleText.text = shopItemsSO[i].cardName;
            shopPanels[i].descriptionText.text = shopItemsSO[i].description;
            shopPanels[i].costText.text = shopItemsSO[i].cost.ToString() + " Gold";
            Image imageComponent = shopPanels[i].cardImage.GetComponent<Image>();
            imageComponent.sprite = shopItemsSO[i].cardArt;
        }
    }
}
