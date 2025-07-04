using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TutorialScripts
{

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
                    if (droppedCard.GetComponent<CardPlace>().isPlayable == true)
                    {
                        Debug.Log("set being played");
                        droppedCard.GetComponent<CardPlace>().beingPlayed = true;
                        droppedCard.GetComponent<CardPlace>().correspondingImage.GetComponent<Image>().material = droppedCard.GetComponent<CardPlace>().defaultMat;
                        droppedCard.transform.SetParent(playerDiscardZone.transform);


                        //call script to animate card and carry out effect
                        //droppedCard.GetComponent
                    }
                    else
                    {
                        droppedCard.GetComponent<CardPlace>().beingPlayed = false;
                    }
                }

            }
        }
    }
}