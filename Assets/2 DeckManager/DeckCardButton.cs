using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using AK.Wwise;

public class DeckCardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject specialCardPrefab;
    public GameObject buttonPrefab;

    public GameObject sidePanel;

    public GameObject descriptionPanel;

    public string cardName;
    public string cardDesc;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public bool owned = false;

    private GameObject sideButton;



    // Start is called before the first frame update
    void Start()
    {
        title.text = cardName;
        description.text = cardDesc;
        sidePanel = GameObject.Find("SidePanel");

        //darken because not added
        this.GetComponent<Image>().color = new Color32(200, 200, 200, 128);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCardToDeck() //toggle card in deck
    {
        //check if deck has less than 10
        if (CardInventoryController.instance.playerDeck.Count < 10 && !CardInventoryController.instance.playerDeck.Contains(specialCardPrefab))
        {
            CardInventoryController.instance.playerDeck.Add(specialCardPrefab);
            sideButton = Instantiate(buttonPrefab, sidePanel.transform.GetChild(0));
            sideButton.GetComponentInChildren<TextMeshProUGUI>().text = cardName;
            sideButton.GetComponent<DeckCardButtonSmall>().buttonReference = this.gameObject;
            //this.GetComponent<Button>().interactable = false;
            //lighten
            this.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            AkSoundEngine.PostEvent("Play_Click_2", this.gameObject);
        }
        else if(CardInventoryController.instance.playerDeck.Contains(specialCardPrefab)) //already in deck so remove it and darken (because its not in yoru deck)
        {
            CardInventoryController.instance.playerDeck.Remove(specialCardPrefab);
            Destroy(sideButton);
            //darken
            this.GetComponent<Image>().color = new Color32(200, 200, 200, 128);

            AkSoundEngine.PostEvent("Play_Click_2", this.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
    }
}
