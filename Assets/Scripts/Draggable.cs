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

    public bool duplicateValue;

    public bool discardSpecial;
    public bool discardValue;
    public bool drawTwoSpecial;
    public bool drawOneSpecial;
    public bool finisherSwap;

    public bool five;
    public bool eight;
    public bool seven;
    public bool nine;
    public bool two;
    public bool youNegative;
    public bool theyDiscarded;

    public bool duplicate;

    public bool conditionMet;


    public GameObject givePrefab;


    public int finisherSwapCount = 0;

    GameManager gm;

    public bool playableCard;
    public bool attack;
    public bool help;

    void Start()
    {
        discard = GameObject.Find("Discard").transform;
    }

    void Update()
    {
        

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnBeginDrag");

        
            //if(conditionMet == true)
            //{
            parentReturnTo = this.transform.parent;
            

            

            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //}
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        
            //if(conditionMet==true)
            this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        //if(conditionMet==true)
        //{
            cardActivate = true;

            this.transform.SetParent(parentReturnTo);

            GetComponent<CanvasGroup>().blocksRaycasts = true;
            /*
            if(!playableCard)
            {
                this.transform.SetParent(parentReturnTo);
            }
            
                
                    GameObject discardPile = GameObject.Find("Discard");
                    this.transform.position = discardPile.transform.position; 
                    //gm.playerSpecials.Remove(this.gameObject);

                    if(this.transform.position == discardPile.transform.position)
                    {
                        Debug.Log("position equal");
                        cardActivate = true;
                    }
            */    
            
        //}
    }

    
}
