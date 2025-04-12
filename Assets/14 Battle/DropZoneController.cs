using UnityEngine;
using UnityEngine.EventSystems;

public class DropZoneController : MonoBehaviour, IDropHandler
{
    public GameObject playerDiscardZone;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Card dropped into the zone!");

        GameObject droppedCard = eventData.pointerDrag;
        if (droppedCard != null)
        {
            if (!droppedCard.TryGetComponent<NumberStats>(out var component))
            {
                droppedCard.GetComponent<CardPlace>().beingPlayed = true;
                droppedCard.transform.SetParent(playerDiscardZone.transform);

                //call script to animate card and carry out effect
                //droppedCard.GetComponent
            }

        }
    }
}