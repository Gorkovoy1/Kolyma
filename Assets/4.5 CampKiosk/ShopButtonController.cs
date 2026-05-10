using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonController : MonoBehaviour
{
    public GameObject deckButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseCard()
    {
        //check if can purchase, and subtract money


        //add to inventory by setting owned
        if(deckButton != null)
        {
            deckButton.GetComponent<DeckCardButton>().owned = true;
        }

        //deactivate button
        this.GetComponent<Button>().interactable = false;
    }
}
