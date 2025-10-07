using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class OpponentDiscardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image lastPlayed;
    public Image discardImage;
    public string cardDesc;
    public GameObject discardedCard;
    public TextMeshProUGUI cardDescText;

    // Start is called before the first frame update
    void Start()
    {
        discardedCard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(lastPlayed != null)
        {
            discardImage.sprite = lastPlayed.transform.Find("Image").GetComponent<Image>().sprite;
            cardDescText.text = cardDesc;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        discardedCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        discardedCard.SetActive(false);
    }
}
