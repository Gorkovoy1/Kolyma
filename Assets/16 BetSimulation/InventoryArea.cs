using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryArea : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        ConsumableController item = eventData.pointerDrag.GetComponent<ConsumableController>();

        if (item != null)
        {
            item.transform.SetParent(transform);
            item.transform.position = item.originalPosition;
        }
    }
}