using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject discard, playZone;
    private Transform parentReturnTo;
    private DisplayCard display;

    private bool dragActive = false;

    void Start() {
        display = gameObject.GetComponent<DisplayCard>();
        discard = CardGameUIManager.Instance.DiscardZone;
        playZone = CardGameUIManager.Instance.CardPlayZone;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(display.baseCard is SpecialDeckCard && (CardGameManager.Instance.GameState == CardGameManager.State.PLAYERTURN)) {
            parentReturnTo = gameObject.transform.parent;
            dragActive = true;
        }
        else {
            dragActive = false;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(dragActive) {
            gameObject.transform.position = eventData.position;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(dragActive && display.baseCard is SpecialDeckCard && (CardGameManager.Instance.GameState == CardGameManager.State.PLAYERTURN || CardSelectionHandler.Instance.SelectingCharacter != null))
        {
            dragActive = false;
            gameObject.transform.SetParent(parentReturnTo);

            if(gameObject.transform.parent == playZone.transform) {
                CardGameManager.Instance.PlayCard(display);
            }
            else if(gameObject.transform.parent == discard.transform) {
                CardGameManager.Instance.DiscardCard(display);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D col) {
        if(dragActive) {
            parentReturnTo = col.gameObject.transform;
        }
    }
    
}