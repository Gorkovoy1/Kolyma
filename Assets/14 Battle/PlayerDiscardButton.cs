using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerDiscardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject discardedVisualPrefab;
    public Transform cardPanelParent;
    public Image lastPlayed;
    public string cardDesc;
    public bool lastPlayedBool = false;
    public GameObject placeholderVisual;
    public GameObject cardPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPlayed != null && !lastPlayedBool)
        {
            lastPlayedBool = true;
            Destroy(placeholderVisual);
        }

    }

    public void AddCardToList()
    {
        GameObject newCard = Instantiate(discardedVisualPrefab, cardPanelParent);
        newCard.GetComponent<Image>().sprite = lastPlayed.transform.Find("Image").GetComponent<Image>().sprite;
        newCard.GetComponentInChildren<TextMeshProUGUI>().text = cardDesc;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardPanel.SetActive(false);
    }
}
