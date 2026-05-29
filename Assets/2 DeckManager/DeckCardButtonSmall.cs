using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AK.Wwise;

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
        AkSoundEngine.PostEvent("Play_Click_2", this.gameObject);
        buttonReference.GetComponent<Image>().color = new Color32(200, 200, 200, 128); //darken cuz not in deck
        CardInventoryController.instance.playerDeck.Remove(buttonReference.GetComponent<DeckCardButton>().specialCardPrefab);
        Destroy(this.gameObject);
    }
}
