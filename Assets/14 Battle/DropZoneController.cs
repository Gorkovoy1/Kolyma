using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZoneController : MonoBehaviour, IDropHandler
{
    public GameObject playerDiscardZone;
    public GameObject droppedCard;

    public void OnDrop(PointerEventData eventData)
    {
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
                    droppedCard.GetComponent<CardPlace>().inDropZone = true;
                }
                
            }

        }
    }
}