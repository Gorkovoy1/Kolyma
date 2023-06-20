using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

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


    public bool draw2;
    public bool give4;
    public bool giveself2;
    public bool swaphighest;
    public bool giveopp2;
    public bool forcediscardspecial;
    public bool swapfinisher;
                   
    GameObject newObj;
    int integerValueToMatch;

    public int addValue;


    void Start()
    {
        discard = GameObject.Find("Discard").transform;
        GameObject tempBoard = GameObject.Find("Canvas/Panel");
        board = tempBoard.transform; 
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();


        if(duplicate)
        {
                    List<GameObject> myPrefabList = new List<GameObject>();

                    // Finding duplicates using LINQ
                    for(int j = 0; j < gm.AIValues.Count; j++)
                    {
                        myPrefabList.Add(gm.AIValues[j]);
                    }
                    for(int k = 0; k < gm.tempNegsAI.Count; k++)
                    {
                        myPrefabList.Add(gm.tempNegsAI[k]);
                    }


                    var duplicateGroups = myPrefabList.GroupBy(x => x.GetComponent<ValueCard>().value)
                        .Where(g => g.Count() > 1)
                        .Select(g => new { Value = g.Key, Objects = g.ToList() });

                    // Moving one duplicate to the duplicatesList
        
                    foreach (var group in duplicateGroups)
                    {
                        // Select the first duplicate and remove it from the myPrefabList
                        GameObject duplicatePrefab2 = group.Objects[0];    
                        addValue = 0 - duplicatePrefab2.GetComponent<ValueCard>().value;
                    }
        }
        if(nine)
        {
                List<GameObject> combinedList = new List<GameObject>();
                combinedList.AddRange(gm.playerValues);
                combinedList.AddRange(gm.tempNegsPlayer);

                int lowestValue = int.MaxValue;
                GameObject lowestObject = null;

                foreach (GameObject obj in combinedList)
                {
                    // Get the integer component of the current game object
                    int value = obj.GetComponent<ValueCard>().value;

                    // Check if this integer value is the lowest one found so far
                    if (value < lowestValue)
                    {
                        lowestValue = value;
                        lowestObject = obj;
                    }
                }

                addValue = 0 - lowestValue;
        }
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

    public void DestroyAICard()
    {
        GameObject copy = this.gameObject;
        gm.played.Add(copy);
        gm.AISpecials.Remove(copy);
        gm.playableCards.Remove(copy);
        gm.AIAttacks.Remove(copy);
        gm.AIHelps.Remove(copy);
        //Destroy(this.gameObject);
    }

    public void ExecuteCard()
    {
            Debug.Log("executing card");
            if(duplicate)
            {
                Debug.Log("duplicate");
                    List<GameObject> myPrefabList = new List<GameObject>();

                    // Finding duplicates using LINQ
                    for(int j = 0; j < gm.AIValues.Count; j++)
                    {
                        myPrefabList.Add(gm.AIValues[j]);
                    }
                    for(int k = 0; k < gm.tempNegsAI.Count; k++)
                    {
                        myPrefabList.Add(gm.tempNegsAI[k]);
                    }


                    var duplicateGroups = myPrefabList.GroupBy(x => x.GetComponent<ValueCard>().value)
                        .Where(g => g.Count() > 1)
                        .Select(g => new { Value = g.Key, Objects = g.ToList() });

                    // Moving one duplicate to the duplicatesList
        
                    foreach (var group in duplicateGroups)
                    {
                        // Select the first duplicate and remove it from the myPrefabList
                        GameObject duplicatePrefab2 = group.Objects[0];
                        myPrefabList.Remove(duplicatePrefab2);
                        GameObject duplicatePrefab = (GameObject) Instantiate (duplicatePrefab2, board);

                        if(duplicatePrefab.GetComponent<ValueCard>().value > 0)
                        {
                            gm.AIValues.Remove(group.Objects[0]);
                            gm.playerValues.Add(duplicatePrefab);
                            duplicatePrefab.transform.SetParent(board, false);
                            //gm.AIValues.Insert(gm.AIValues.Count, duplicatePrefab);
                            gm.OrganizeCards();
                            gm.AICardsGiven += 1;
                        }
                        if(duplicatePrefab.GetComponent<ValueCard>().value < 0)
                        {
                            gm.tempNegsAI.Remove(group.Objects[0]);
                            gm.tempNegsPlayer.Add(group.Objects[0]);
                            group.Objects[0].transform.SetParent(board, false);
                            gm.OrganizeCards();
                            gm.AICardsGiven += 1;
                        }

            
                        DestroyAICard();
                    }
            }
            

            if(two)
            {
                Debug.Log("two");
                //draw special card for AI hand
                if(gm.AISpecialDeck.Count > 0)
                {
                    GameObject drawnSpecial = (GameObject) Instantiate(gm.AISpecialDeck[0]);
                    drawnSpecial.transform.SetParent(gm.AIHand, false);
                    drawnSpecial.transform.localPosition = new Vector3(0, -200, 0);
                    gm.AISpecials.Add(drawnSpecial);
                    gm.AISpecialDeck.RemoveAt(0);
                    DestroyAICard();
                }
                else
                {
                    Debug.Log("Deck is empty!");
                }
            }

            if(seven)
            {
                Debug.Log("seven");
                //give player +3
                GameObject three = this.GetComponent<Give>().three;
                GameObject givePrefab = (GameObject) Instantiate (three, board);
                givePrefab.transform.SetParent(board, false);
                gm.playerValues.Add(givePrefab);
                gm.OrganizeCards();
                gm.AICardsGiven += 1;

                DestroyAICard();
            }

            if(eight)
            {
                Debug.Log("eight");
                //player discards special card
                if(gm.playerSpecials != null && gm.playerSpecials.Count > 0)
                {
                    int randomIndex = Random.Range(0, gm.playerSpecials.Count);
                    GameObject tempPrefab = gm.playerSpecials[randomIndex];
                    gm.playerSpecials.RemoveAt(randomIndex);
                    Destroy(tempPrefab);
                    gm.playerHasDiscarded = true;
                    gm.playerCardsDiscarded += 1;
                }
                else
                {
                    Debug.Log("The opponent's hand is empty!");
                }
                DestroyAICard();
            }

            if(nine)
            {
            Debug.Log("nine");
                //player flips lowest card
                List<GameObject> combinedList = new List<GameObject>();
                combinedList.AddRange(gm.playerValues);
                combinedList.AddRange(gm.tempNegsPlayer);

                int lowestValue = int.MaxValue;
                GameObject lowestObject = null;

                foreach (GameObject obj in combinedList)
                {
                    // Get the integer component of the current game object
                    int value = obj.GetComponent<ValueCard>().value;

                    // Check if this integer value is the lowest one found so far
                    if (value < lowestValue)
                    {
                        lowestValue = value;
                        lowestObject = obj;
                    }
                }

                integerValueToMatch = 0 - lowestValue;

                // Find all objects in the scene with the specified component
                ValueCard[] components = FindObjectsOfType<ValueCard>();

                if (components != null && components.Length > 0)
                {
                    // Get the first object with the specified integer value for the component
                    GameObject obj = GetObjectWithValueCard(components, integerValueToMatch);

                    if (obj != null)
                    {
                        // Instantiate the found object
                        newObj = Instantiate(obj);

                        // Use the new object however you need to in your script
                        Debug.Log("New object instantiated: " + newObj.name);
                    }
                    else
                    {
                        Debug.Log("No object found with integer value of " + integerValueToMatch + " in ValueCard component");
                    }
                }

                if(lowestObject.GetComponent<ValueCard>().value > 0)
                {
                    gm.playerValues.Remove(lowestObject);
                    Destroy(lowestObject);
                    GameObject flipped = newObj;
                    gm.tempNegsPlayer.Add(flipped);
                    gm.OrganizeCards();
                }
                else
                {
                    gm.tempNegsPlayer.Remove(lowestObject);
                    Destroy(lowestObject);
                    GameObject flipped = newObj;
                    gm.playerValues.Add(flipped);
                    gm.OrganizeCards();
                }
                DestroyAICard();
            }

            if(youNegative)
            {
            Debug.Log("youneg");
                //player discards special card
                if(gm.playerSpecials != null && gm.playerSpecials.Count > 0)
                {
                    int randomIndex = Random.Range(0, gm.playerSpecials.Count);
                    GameObject tempPrefab = gm.playerSpecials[randomIndex];
                    gm.playerSpecials.RemoveAt(randomIndex);
                    Destroy(tempPrefab);
                    gm.playerHasDiscarded = true;
                    gm.playerCardsDiscarded += 1;
                }
                else
                {
                    Debug.Log("The opponent's hand is empty!");
                }
                DestroyAICard();
            }

            if(theyDiscarded)
            {
            Debug.Log("theydisc");
                //give player +2
                if(gm.playerHasDiscarded)
                {
                    GameObject two = this.GetComponent<Give>().two;
                    GameObject givePrefab = (GameObject) Instantiate (two, board);
                    givePrefab.transform.SetParent(board, false);
                    //gm.AIValues.Insert(0, givePrefab);
                    gm.playerValues.Add(givePrefab);
                    gm.OrganizeCards();
                    gm.AICardsGiven += 1;
                    DestroyAICard();
                }
            }

            if(five)
            {
            Debug.Log("five");
                //swap out the five
                // Loop through the list of game objects
                foreach (var gameObject in gm.playerValues)
                {
                    // Check if the game object has the component
                    if (gameObject.GetComponent<ValueCard>().value == 5)
                    {
                        // Remove the game object from the list
                        gm.playerValues.Remove(gameObject);
                        Destroy(gameObject);
                        GameObject nextCard = (GameObject) Instantiate (gm.valueDeck[0]);
                        gm.playerValues.Add(nextCard);
                        gm.valueDeck.RemoveAt(0);
                        gm.OrganizeCards();
                        gm.AICardsSwapped+= 1;
                        DestroyAICard();
                        break;
                    }
                }

            }

            if(conditionMet)
            {
                if(draw2)
                {
                Debug.Log("draw2");
                    //draw 2 for AI hand
                    gm.DrawSpecial(gm.AISpecialDeck, gm.AIHand);
                    gm.DrawSpecial(gm.AISpecialDeck, gm.AIHand);
                    DestroyAICard();
                }
                if(give4)
                {
                Debug.Log("give4");
                    //give4 method
                    this.GetComponent<Give>().GiveAll4();
                    DestroyAICard();
                }
                if(giveself2)
                {
                Debug.Log("giveslef2");
                    //giveAI-2
                    GameObject negTwo = this.GetComponent<Give>().negTwo;
                    GameObject givePrefab = (GameObject) Instantiate (negTwo, board);
                    givePrefab.transform.SetParent(board, false);
                    gm.AIValues.Add(givePrefab);
                    gm.OrganizeCards();
                    DestroyAICard();
                }
                if(swaphighest)
                {
                Debug.Log("swaphi");
                    //swap highest for player
                    int highestValue = int.MinValue;
                    GameObject highestObject = null;
        
                    List<GameObject> combinedList = new List<GameObject>();
                    combinedList.AddRange(gm.playerValues);
                    combinedList.AddRange(gm.tempNegsPlayer);

                    foreach (GameObject obj in combinedList)
                    {
                        // Get the integer component of the current game object
                        int value = obj.GetComponent<ValueCard>().value;

                        // Check if this integer value is the highest one found so far
                        if (value > highestValue)
                        {
                            highestValue = value;
                            highestObject = obj;
                        }
                    }
                    gm.playerValues.Remove(highestObject);
                    combinedList.Remove(highestObject);
                    Destroy(highestObject);
                    GameObject nextCard = (GameObject) Instantiate (gm.valueDeck[0]);
                    gm.playerValues.Add(nextCard);
                    gm.valueDeck.RemoveAt(0);
                    gm.OrganizeCards();
                    gm.cardsSwapped += 1;

                    DestroyAICard();
                }
                if(giveopp2)
                {
                Debug.Log("give2");
                    //give player +2
                    GameObject two = this.GetComponent<Give>().two;
                    GameObject givePrefab = (GameObject) Instantiate (two, board);
                    givePrefab.transform.SetParent(board, false);
                    //gm.AIValues.Insert(0, givePrefab);
                    gm.playerValues.Add(givePrefab);
                    gm.OrganizeCards();
                    gm.AICardsGiven += 1;
                    DestroyAICard();

                }
                if(swapfinisher)
                {
                Debug.Log("finish");
                    this.GetComponent<Finisher>().SwapFinisher();
                    DestroyAICard();
                    gm.GameOver = true;
                }
                if(forcediscardspecial)
                {
                Debug.Log("discardspec");
                    //player discard special
                    if(gm.playerSpecials != null && gm.playerSpecials.Count > 0)
                    {
                        int randomIndex = Random.Range(0, gm.playerSpecials.Count);
                        GameObject tempPrefab = gm.playerSpecials[randomIndex];
                        gm.playerSpecials.RemoveAt(randomIndex);
                        Destroy(tempPrefab);
                        gm.playerHasDiscarded = true;
                        gm.playerCardsDiscarded += 1;
                    }
                    else
                    {
                        Debug.Log("The opponent's hand is empty!");
                    }
                    DestroyAICard();
                }
                
            }
            else
            {
                Debug.Log("else");
            }
        

    }

    private GameObject GetObjectWithValueCard(ValueCard[] components, int valueToMatch)
    {
        foreach (ValueCard component in components)
        {
            if (component.value == valueToMatch)
            {
                return component.gameObject;
            }
        }

        return null;
    }

    
}


//change: for each card discarded - combined by two players? given?