using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

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
    

    // Start is called before the first frame update
    void Start()
    {
        title.text = cardName;
        description.text = cardDesc;
        sidePanel = GameObject.Find("SidePanel");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCardToDeck()
    {
        //check if deck has less than 15
        if (CardInventoryController.instance.playerDeck.Count < 15)
        {
            CardInventoryController.instance.playerDeck.Add(specialCardPrefab);
            GameObject sideButton = Instantiate(buttonPrefab, sidePanel.transform.GetChild(0));
            sideButton.GetComponentInChildren<TextMeshProUGUI>().text = cardName;
            sideButton.GetComponent<DeckCardButtonSmall>().buttonReference = this.gameObject;
            this.GetComponent<Button>().interactable = false;
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
