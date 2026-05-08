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
            if (oldItem.gameObject.CompareTag("Bread"))
            {
                InventoryManager.instance.AddNewItem(oldItem.gameObject, InventoryManager.instance.breadList, InventoryManager.instance.breadIndex);
            }
            else if (oldItem.gameObject.CompareTag("Drink"))
            {
                InventoryManager.instance.AddNewItem(oldItem.gameObject, InventoryManager.instance.drinkList, InventoryManager.instance.drinkIndex);
            }
            else if (oldItem.gameObject.CompareTag("Clothes"))
            {
                InventoryManager.instance.AddNewItem(oldItem.gameObject, InventoryManager.instance.clothesList, InventoryManager.instance.clothesIndex);
            }
            else if (oldItem.gameObject.CompareTag("Other"))
            {
                InventoryManager.instance.AddNewItem(oldItem.gameObject, InventoryManager.instance.otherList, InventoryManager.instance.otherIndex);
            }
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
