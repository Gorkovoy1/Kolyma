using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private SortingGroup currentSortingGroup;
    private SortingGroup targetSortingGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
        currentSortingGroup = GetComponentInParent<SortingGroup>();  // Get current sorting group of the parent panel
    }

    // Called when dragging starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Begin dragging {gameObject.name}");

        // Disable interaction with the card while dragging
        canvasGroup.blocksRaycasts = false;

        // Make the card semi-transparent during the drag
        canvasGroup.alpha = 0.8f;

        // Store the current sorting group to revert back to it later
        if (currentSortingGroup != null)
        {
            Debug.Log($"Current SortingGroup: {currentSortingGroup.sortingOrder}");
        }

        // Set the sorting order to be above other objects during drag
        currentSortingGroup.sortingOrder = 1000; // Example: Push the card above other UI elements
    }

    // Called while dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen point to local point in the parent canvas
        if (parentCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        // Update the position of the card during drag
        rectTransform.localPosition = localPoint;
    }

    // Called when dragging ends
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"End dragging {gameObject.name}");

        // Reset the sorting order of the card back to the parent sorting group
        if (currentSortingGroup != null)
        {
            Debug.Log($"Resetting SortingGroup: {currentSortingGroup.sortingOrder}");
            currentSortingGroup.sortingOrder = 0;  // Reset to original or adjust as needed
        }

        // Enable interaction with the card after dragging
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f; // Restore the full opacity
    }

    // Called when the card is dropped in a different panel
    public void OnDrop(PointerEventData eventData)
    {
        // Debugging drop event
        Debug.Log($"Dropped {gameObject.name} onto {eventData.pointerDrag.name}");

        // Check if the card is dropped onto a new panel with a different sorting group
        var targetPanel = eventData.pointerEnter?.GetComponentInParent<SortingGroup>();
        if (targetPanel != null)
        {
            // Update the card's sorting group to the new panel's sorting group
            targetSortingGroup = targetPanel;
            Debug.Log($"Target SortingGroup: {targetSortingGroup.sortingOrder}");
        }

        // Optionally, re-parent the card if it should change panels
        if (targetSortingGroup != null)
        {
            transform.SetParent(targetSortingGroup.transform);
            Debug.Log($"Re-parented {gameObject.name} to new panel with sorting order: {targetSortingGroup.sortingOrder}");
        }
    }
}