using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private GameObject discard, panel;
    private Transform parentReturnTo;
    private CardGameManager manager;
    private DisplayCard display;

    private bool dragActive = false;

    void Start() {
        display = gameObject.GetComponent<DisplayCard>();
        discard = GameObject.Find("Canvas/Discard");
        panel = GameObject.Find("Canvas/Panel");
        manager = GameObject.Find("Game Manager").GetComponent<CardGameManager>();    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        parentReturnTo = gameObject.transform.parent;
        dragActive = true;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        gameObject.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragActive = false;
        gameObject.transform.SetParent(parentReturnTo);

        if(gameObject.transform.parent == panel.transform) {
            manager.PlayCard(display.baseCard, display.owner);
        }

    }

    void OnTriggerEnter2D(Collider2D col) {
        if(dragActive) {
            parentReturnTo = col.gameObject.transform;
        }
    }
    
}