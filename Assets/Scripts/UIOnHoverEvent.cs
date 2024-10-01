using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject artwork;

    private DisplayCard card;
    Vector3 cachedPosition;
    Vector3 cachedScale;
    

    void Start()
    {
        if(artwork)
        {
            cachedScale = artwork.transform.localScale;
            cachedPosition = artwork.transform.localPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card = gameObject.GetComponent<DisplayCard>();
        
        if(card != null && card.baseCard is SpecialDeckCard && card.owner == CardGameManager.Instance.player) 
        {
            ShowInfo();
        }

        if(GetComponent<DeckUI>())
        {
            ShowInfo();
        }
        //Debug.Log("enter");
    }


    //fix on click position messing up and not registering as on pointer exit

    public void OnPointerExit(PointerEventData eventData)
    {

         if(card != null && card.baseCard is SpecialDeckCard && card.owner == CardGameManager.Instance.player) {
            HideInfo();
        }

        if (GetComponent<DeckUI>())
        {
            HideInfo();
        }
    }

    public void ShowInfo()
    {
        if(artwork)
        {
            cachedPosition = artwork.transform.localPosition;
            artwork.transform.localPosition = new Vector3(artwork.transform.localPosition.x, artwork.transform.localPosition.y + 100, artwork.transform.localPosition.z);
        }

        if (gameObject.TryGetComponent(out DisplayCard displayCard))
        {
            displayCard.SetHover(true);
        }
        else if (gameObject.TryGetComponent(out DeckUI deckUI))
        {
            deckUI.ShowInfo();
        }
    }

    public void HideInfo()
    {
        if (artwork)
        {
            artwork.transform.localScale = cachedScale;
            artwork.transform.localPosition = cachedPosition;
        }

        if (gameObject.TryGetComponent(out DisplayCard displayCard))
        {
            displayCard.SetHover(false);
        }
        else if(gameObject.TryGetComponent(out DeckUI deckUI))
        {
            deckUI.HideInfo();
        }
    }
}
