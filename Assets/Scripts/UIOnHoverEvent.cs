using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIOnHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject artwork;

    private Canvas tempCanvas;
    private GraphicRaycaster tempRaycaster;
    private DisplayCard card;
    Vector3 cachedPosition;
    Vector3 cachedScale;
    private RectTransform rectTrans;
    

    void Start()
    {
        cachedScale = artwork.transform.localScale;
        cachedPosition = artwork.transform.localPosition;
        rectTrans = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card = gameObject.GetComponent<DisplayCard>();
        
        if(card != null && card.baseCard is SpecialDeckCard && card.owner == CardGameManager.Instance.player) 
        {
            /*tempCanvas = gameObject.AddComponent<Canvas>();
            tempCanvas.overrideSorting = true;
            tempCanvas.sortingOrder = 1;
            tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            cachedScale = transform.localScale;*/
            cachedPosition = artwork.transform.localPosition;

            //artwork.transform.localScale = cachedScale * 1.5f;
            artwork.transform.localPosition = new Vector3(artwork.transform.localPosition.x, artwork.transform.localPosition.y + 100, artwork.transform.localPosition.z);
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        //Debug.Log("enter");
    }


    //fix on click position messing up and not registering as on pointer exit

    public void OnPointerExit(PointerEventData eventData)
    {

         if(card != null && card.baseCard is SpecialDeckCard && card.owner == CardGameManager.Instance.player) {
            //Destroy(tempRaycaster);
            //Destroy(tempCanvas);


            artwork.transform.localScale = cachedScale;
            artwork.transform.localPosition = cachedPosition;
            gameObject.GetComponent<DisplayCard>().ToggleHover();
        }
        
        //Debug.Log("exit");
    }

    
    
}
