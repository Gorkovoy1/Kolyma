using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;
    public CardGameManager gm;
    private DisplayCard card;
    Vector3 cachedPosition;
    Vector3 cachedScale;

    

    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<CardGameManager>();
        cachedScale = transform.localScale;
        cachedPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card = gameObject.GetComponent<DisplayCard>();
        
        if(card != null && card.baseCard is SpecialDeckCard && card.owner == gm.player) 
        {
            tempCanvas = gameObject.AddComponent<Canvas>();
            tempCanvas.overrideSorting = true;
            tempCanvas.sortingOrder = 1;
            tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            cachedScale = transform.localScale;
            cachedPosition = transform.localPosition;

            transform.localScale = new Vector3(0.2f, 0.2f, 0.5f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 60, transform.localPosition.z);
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        //Debug.Log("enter");
    }


    //fix on click position messing up and not registering as on pointer exit

    public void OnPointerExit(PointerEventData eventData)
    {

         if(card != null && card.baseCard is SpecialDeckCard && card.owner == gm.player) {
            Destroy(tempRaycaster);
            Destroy(tempCanvas);
            

            transform.localScale = cachedScale;
            transform.localPosition = cachedPosition;
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        
        //Debug.Log("exit");
    }

    
    
}
