using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CardPlace : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler

{
    public bool isDragging;
    public Transform parentReturnTo = null;
    public GameObject imagePrefab;
    public GameObject correspondingImage;
    public Transform imagesParent;
    // Start is called before the first frame update
    void Start()
    {
        if(imagePrefab != null)
        {
            //this means its a special card
            correspondingImage = Instantiate(imagePrefab, imagesParent);
            correspondingImage.GetComponent<SpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log ("OnBeginDrag");

        
            //if(conditionMet == true)
            //{
            parentReturnTo = this.transform.parent;
            
            this.transform.SetParent(parentReturnTo.transform.parent);
            

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //}
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        

            this.transform.SetParent(parentReturnTo);
            //this.transform.position = new Vector3(0,0,0);

            GetComponent<CanvasGroup>().blocksRaycasts = true;
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        
            //if(conditionMet==true)
            this.transform.position = eventData.position;
            this.correspondingImage.transform.SetSiblingIndex(5);
            //set as last index in array of specila cards (special number of cards)

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hovered over card!");
        // Add hover effect (e.g., scale, change color)
    }

    // On hover out
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Hover exited!");
        // Revert hover effect (e.g., reset scale, color)
    }


}
