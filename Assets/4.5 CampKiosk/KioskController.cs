using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KioskController : MonoBehaviour
{
    public GameObject shopPanel;
    public List<GameObject> shopPool;

    public int cardsToStock;
    public int resourcesToStock;

    public GameObject shopButtonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        shopPool = new List<GameObject>();

        StockShop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StockShop()
    {
        foreach (Transform child in shopPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(GameObject g in CardInventoryController.instance.cardInventory)
        {
            if(!g.GetComponent<DeckCardButton>().owned)
            {
                shopPool.Add(g);
            }
        }

        for(int i = 0; i < cardsToStock; i++)
        {
            int randomIndex = Random.Range(0, shopPool.Count);
            GameObject cardToAdd = shopPool[randomIndex];

            //instantiate a button, feed card info
            GameObject newCard = Instantiate(shopButtonPrefab, shopPanel.transform);

            //update button image, name, desc
            newCard.GetComponent<Image>().sprite = cardToAdd.GetComponent<Image>().sprite;
            newCard.transform.Find("CardName").GetComponent<TextMeshProUGUI>().text = cardToAdd.GetComponent<DeckCardButton>().cardName;
            newCard.transform.Find("CardDesc").GetComponent<TextMeshProUGUI>().text = cardToAdd.GetComponent<DeckCardButton>().cardDesc;
            newCard.GetComponent<ShopButtonController>().deckButton = shopPool[randomIndex];


            shopPool.RemoveAt(randomIndex);
        }
    }
}
