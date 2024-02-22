using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 cachedScale;
    Vector3 cachedPosition;
    public CardGameManager gm;
    private DisplayCard card;

    

    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<CardGameManager>();
        cachedScale = transform.localScale;
        cachedPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card = gameObject.GetComponent<DisplayCard>();
        
        if(card != null && card.baseCard is SpecialDeckCard && card.owner == gm.player) {
            cachedScale = transform.localScale;
            cachedPosition = transform.localPosition;

            transform.localScale = new Vector3(0.2f, 0.2f, 0.5f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 60, transform.localPosition.z);
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        //transform.position += Vector3.up;
        //transform.localPosition = new Vector3(transform.localPosition.x, 200, transform.localPosition.z);
        //i think the issue is that its parented

        //Debug.Log("enter");
    }


    //fix on click position messing up and not registering as on pointer exit

    public void OnPointerExit(PointerEventData eventData)
    {

        if(card != null && card.baseCard is SpecialDeckCard && card.owner == gm.player) {

            transform.localScale = cachedScale;
            transform.localPosition = cachedPosition;
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        
        //Debug.Log("exit");
    }

    
    
}
