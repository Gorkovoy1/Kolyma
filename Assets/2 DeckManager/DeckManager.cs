using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckManager : MonoBehaviour
{


    public int cardCount;
    public TMP_Text cardCountText;
    public SpecialCardSO[] specialCardSO;
    public GameObject[] cardListGO;
    public CardItem[] cardList;

    // Start is called before the first frame update
    void Start()
    {
        
        cardCountText.text = cardCount.ToString() + "/20";
        LoadSlots();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddCards()
    {
        //check if under max
        cardCount++;
        cardCountText.text = cardCount.ToString() + "/20";
    }

    public void LoadSlots()
    {
        for (int i = 0; i < specialCardSO.Length; i++)
        {
            cardList[i].descriptionText.text = specialCardSO[i].description;
            Image imageComponent = cardList[i].cardImage.GetComponent<Image>();
            imageComponent.sprite = specialCardSO[i].cardArt;
        }
    }
}
