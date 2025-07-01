using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardButtonSmall : MonoBehaviour
{
    public GameObject buttonReference; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveCardFromDeck()
    {
        buttonReference.GetComponent<Button>().interactable = true;
        CardInventoryController.instance.playerDeck.Remove(buttonReference.GetComponent<DeckCardButton>().specialCardPrefab);
        Destroy(this.gameObject);
    }
}
