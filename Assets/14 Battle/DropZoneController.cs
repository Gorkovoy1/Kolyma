using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneController : MonoBehaviour, IDropHandler
{
    public GameObject playerDiscardZone;
    public GameObject droppedCard;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Card dropped into the zone!");
        if(eventData.pointerDrag != null)
        {
            droppedCard = eventData.pointerDrag;
        }
        
        if (droppedCard != null)
        {
            if (!droppedCard.TryGetComponent<NumberStats>(out var component))
            {
                if(droppedCard.TryGetComponent<CardPlace>(out var cardPlace))
                {
                    if (cardPlace.isPlayable == true)
                    {
                        Debug.Log("set being played");
                        cardPlace.beingPlayed = true;
                        cardPlace.correspondingImage.GetComponent<Image>().material = droppedCard.GetComponent<CardPlace>().defaultMat;
                        droppedCard.transform.SetParent(playerDiscardZone.transform);


                        //call script to animate card and carry out effect
                        //droppedCard.GetComponent
                    }
                    else
                    {
                        cardPlace.beingPlayed = false;
                    }
                }
                
            }

        }
    }
}