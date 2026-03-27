using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BetSlot : MonoBehaviour, IDropHandler
{
    public GameObject itemToBet;
    public Transform inventoryParent; 

    public void OnDrop(PointerEventData eventData)
    {
        ConsumableController newItem = eventData.pointerDrag.GetComponent<ConsumableController>();

        if (newItem == null) return;

        // If slot already has item send it back
        if (transform.childCount > 0)
        {
            Transform oldItem = transform.GetChild(0);
            oldItem.SetParent(inventoryParent);
            oldItem.transform.position = oldItem.GetComponent<ConsumableController>().originalPosition;
        }

        // Place new item
        newItem.transform.SetParent(transform);
        newItem.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (transform.childCount > 0)
        {
            itemToBet = transform.GetChild(0).gameObject;
        }
        else
        {
            itemToBet = null;
        }
    }
}
