using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public Transform parentReturnTo = null;
    public Transform board;
    public Transform discard;
    public bool cardActivate = false;
    public bool swap;
    public bool flip;
    public bool give;
    public bool self;
    public bool opponent;

    public GameObject plusTwoPrefab;
    public GameObject minusTwoPrefab;

    void Update()
    {
        if(cardActivate == true)
        {
            cardActivate = false;
            
            //figure out what the effect is (by reading a string?)
            //or a boolean (have all possible cards as bools and check diff for each)
            //if true then do it
            //carry out the effect
            if(give == true)
            {
                Debug.Log("give");
                if(opponent == true)
                {
                    GameObject given = (GameObject) Instantiate (plusTwoPrefab);

                }

            }
            else if(swap == true)
            {
                Debug.Log("swap");

            }
            else if(flip == true)
            {
                Debug.Log("flip");

            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log ("OnBeginDrag");

        parentReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");

        this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        this.transform.SetParent(parentReturnTo);

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if(this.transform.parent == parentReturnTo)
        {
            Debug.Log("Parent is Opponent Hand");
            GameObject discardPile = GameObject.Find("Discard");
            this.transform.position = discardPile.transform.position; 

            if(this.transform.position == discardPile.transform.position)
            {
                Debug.Log("position equal");
                cardActivate = true;
            }
        }

    }

    
}
