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

    public float hoverOffset;
    public bool hovering;
    public bool dragging;
    public bool beingPlayed;

    public float testX;
    public float testY;

    public Transform playerHand;


    // Start is called before the first frame update
    void Start()
    {

        playerHand = this.transform.parent;


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
        dragging = true;
        
        
            //if(conditionMet == true)
            //{
            parentReturnTo = this.transform.parent;
            
            this.transform.SetParent(parentReturnTo.transform.parent);
            

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //}
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        dragging = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        playerHand.GetComponent<HandFanController>().dragging = false;

        if (!beingPlayed)
        {
            this.transform.SetParent(parentReturnTo);
            //this.transform.position = new Vector3(0,0,0);

        }
        else
        {
            
            AnimateBeingPlayed();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        
            //if(conditionMet==true)
            this.transform.position = eventData.position;
            this.correspondingImage.transform.SetSiblingIndex(5);
            //set as last index in array of specila cards (special number of cards)
            playerHand.GetComponent<HandFanController>().dragging = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!beingPlayed)
        {
            if (!dragging)
            {
                hovering = true;
                
            }
            if (correspondingImage != null)
            {
                if (!playerHand.GetComponent<HandFanController>().dragging)
                {
                    correspondingImage.transform.SetSiblingIndex(5);
                    correspondingImage.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                    correspondingImage.transform.position += new Vector3(0f, hoverOffset, 0f);
                }
                
            }
            
        }
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        if(!beingPlayed)
        {
            hovering = false;

            if (correspondingImage != null)
            {
                if (!playerHand.GetComponent<HandFanController>().dragging)
                {
                    correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    correspondingImage.transform.position -= new Vector3(0f, hoverOffset, 0f);
                }

            }



        }
    }

    public void AnimateBeingPlayed()
    {
        this.transform.position = new Vector3(testX, testY, 0);
        correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
    }
}
