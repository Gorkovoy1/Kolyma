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


    public GameObject givePrefab;

    public GameManager gameManager;

    public int finisherSwapCount = 0;



    void Update()
    {
        if(cardActivate == true)
        {
            //must check if condition is fulfilled
            //separate forces and conditions
            //check condition first then check card info 
            //check condition by searching for presence of prefab?



            //on initial deal, read card info and set value bools
            //take bools from game manager as condition


            cardActivate = false;
            
            //figure out what the effect is (by reading a string?)
            //or a boolean (have all possible cards as bools and check diff for each)
            //if true then do it
            //carry out the effect
            if(give == true)
            {
                //if card takes this condition, if list has the value, then carry out action
                //every time card is given, update current value of player
                Debug.Log("give");
                if(opponent == true)
                {
                    GameObject given = (GameObject) Instantiate (givePrefab);
                    GameObject board = GameObject.Find("Board");
                    given.transform.SetParent(board.transform);
                    given.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    given.transform.localPosition = new Vector3(80, 266, 0);

                }
                if(self == true)
                {
                    GameObject given = (GameObject) Instantiate (givePrefab);
                    GameObject board = GameObject.Find("Board");
                    given.transform.SetParent(board.transform);
                    given.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    given.transform.localPosition = new Vector3(80, 110, 0);

                }

            }
            else if(swap == true)
            {
                Debug.Log("swap");
                finisherSwapCount++;

            }
            else if(flip == true)
            {
                Debug.Log("flip");

            }
            else if (drawTwoSpecial == true) //try using get component to set special draw in game manager to true so it executes from there
            {
                if(self == true)
                {
                    //this.GetComponent<GameManager>().DrawTwo();           //this doesnt work, try moving script to here
                    //gameManager.DrawTwo();
                }
            }
            else if(drawOneSpecial == true)
            {
                if(self == true)
                {
                    gameManager.DrawOne();
                }
            }
            else if (finisherSwap == true)
            {
                for(int i = 0; i < finisherSwapCount; i++)
                {
                    GameObject given = (GameObject) Instantiate (givePrefab);
                    GameObject board = GameObject.Find("Board");
                    given.transform.localScale = new Vector3(0.095f, 0.095f, 0.095f);
                    given.transform.localPosition = new Vector3(319, 356, 0);
                    given.transform.SetParent(board.transform);
                }
            }

        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log ("OnBeginDrag");

        if(this.GetComponent<Tile>().special == true)
        {
            parentReturnTo = this.transform.parent;
            this.transform.SetParent(this.transform.parent.parent);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        if(this.GetComponent<Tile>().special == true)
            this.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        if(this.GetComponent<Tile>().special == true)
        {
            cardActivate = true;

            this.transform.SetParent(parentReturnTo);

            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if(this.GetComponent<Tile>().special == true)
            {
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
    }

    
}
