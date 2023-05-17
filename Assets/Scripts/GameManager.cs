using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public List<GameObject> valueDeck;

    public List<GameObject> playerSpecialDeck;
    public List<GameObject> AISpecialDeck;
    public List<GameObject> playerSpecials;
    public List<GameObject> AISpecials;

    public List<GameObject> playerValues;
    public List<GameObject> AIValues;

    public List<GameObject> tempNegsPlayer;
    public List<GameObject> tempNegsAI;

    
    public bool hasDiscarded;


    public Transform board;
    public Transform playerHand;
    public Transform AIHand;


    public int offset;

    public int cardsSwapped;
    public int cardsDiscarded;
    public int cardsGiven;

    public bool gameEnd;

    void Start()
    {
        board = GameObject.Find("Canvas/Panel").transform;
        cardsSwapped = 0;
        hasDiscarded = false;
        ShuffleSpecials();
        DealSpecials();
        ShuffleDeck();
        DealCards();
        OrganizeCards();
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void ShuffleDeck()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = valueDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = valueDeck[i];
            valueDeck[i] = valueDeck[j];
            valueDeck[j] = temp;
        }
    }

    void DealCards()
    {
       

        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 1; j++)
            {
                

                GameObject cardOne = (GameObject) Instantiate(valueDeck[j]);
                cardOne.tag = "PlayerValue";
                playerValues.Add(cardOne);
                
                
             

                GameObject cardTwo = (GameObject) Instantiate(valueDeck[j+1]);
                cardTwo.tag = "AIValue";
                AIValues.Add(cardTwo);
                

                valueDeck.RemoveAt(1);
                valueDeck.RemoveAt(0);
            }
        
        }
    }

    void ShuffleSpecials()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = playerSpecialDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp1 = playerSpecialDeck[i];
            playerSpecialDeck[i] = playerSpecialDeck[j];
            playerSpecialDeck[j] = temp1;
        }

        // Fisher-Yates shuffle algorithm
        for (int i = AISpecialDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp2 = AISpecialDeck[i];
            AISpecialDeck[i] = AISpecialDeck[j];
            AISpecialDeck[j] = temp2;
        }
    }

    void DealSpecials()
    {
        for(int k = 0; k < 6; k++)
        {
            GameObject playerSpecial = (GameObject) Instantiate(playerSpecialDeck[0]);
            playerSpecial.transform.SetParent(playerHand, false);
            //playerSpecial.transform.localPosition = new Vector3(0, -200, 0);
            playerSpecials.Add(playerSpecial);
            playerSpecialDeck.RemoveAt(0);
        }

        for(int j = 0; j < 6; j++)
        {
            GameObject AISpecial = (GameObject) Instantiate(AISpecialDeck[0]);
            AISpecial.transform.SetParent(AIHand, false);
            //AISpecial.transform.localPosition = new Vector3(0, 200, 0);
            AISpecials.Add(AISpecial);
            AISpecialDeck.RemoveAt(0);
        }
    }

    public void DrawSpecial()
    {
        if(playerSpecialDeck.Count > 0)
        {
            GameObject drawnSpecial = (GameObject) Instantiate(playerSpecialDeck[0]);
            drawnSpecial.transform.SetParent(playerHand, false);
            drawnSpecial.transform.localPosition = new Vector3(0, -200, 0);
            playerSpecials.Add(drawnSpecial);
            playerSpecialDeck.RemoveAt(0);
        }
        else
        {
            Debug.Log("Deck is empty!");
        }
        
    }

    void OrganizeValueList()
    {
        

        for(int i = playerValues.Count-1; i >= 0; i--)
        {
            if(playerValues[i].GetComponent<ValueCard>().value < 0)
            {
                tempNegsPlayer.Add(playerValues[i]);
                playerValues.RemoveAt(i);
            }
        }
        



        for(int k = AIValues.Count-1; k >= 0; k--)
        {
            if(AIValues[k].GetComponent<ValueCard>().value < 0)
            {
                tempNegsAI.Add(AIValues[k]);
                AIValues.RemoveAt(k);
            }
        }
        
    }

    public void OrganizeCards()
    {
        OrganizeValueList();

        int playerPos = 0;
        int AIPos = 0;
        int temp = 0;
        int temp2 = 0;

        int tempCount = 0;
        int tempCount2 = 0;

        //try forcing layer sort like in special card hand (opposite for pos and neg)
        //try keeping positives and negatives in separate lists
        //have position indicated after doing positive

        if(playerValues.Count > 0)
        {
            foreach(GameObject i in playerValues)
            {
                i.transform.SetParent(board, false);
                i.transform.localPosition = new Vector3(playerPos, -60, 0);
                playerPos = playerPos + i.GetComponent<ValueCard>().value * offset;
                tempCount = tempCount + i.GetComponent<ValueCard>().value;
            }
        }
        if(tempNegsPlayer.Count > 0)
        {
            temp = tempCount*offset - 10*offset;
            for(int k = 0; k < tempNegsPlayer.Count; k++)
            {
                tempNegsPlayer[k].transform.SetParent(board, false);
                tempNegsPlayer[k].transform.localPosition = new Vector3(temp, -170, 0);
                temp = temp + tempNegsPlayer[k].GetComponent<ValueCard>().value * offset;
            }
        }


        if(AIValues.Count > 0)
        {
            foreach(GameObject i in AIValues)
            {
                i.transform.SetParent(board, false);
                i.transform.localPosition = new Vector3(AIPos, 150, 0);
                AIPos = AIPos + i.GetComponent<ValueCard>().value * offset;
                tempCount2 = tempCount2 + i.GetComponent<ValueCard>().value;
            }
        }
        if(tempNegsAI.Count > 0)
        {
            temp2 = tempCount2*offset - 10*offset;
            for(int k = 0; k < tempNegsAI.Count; k++)
            {
                tempNegsAI[k].transform.SetParent(board,false);
                tempNegsAI[k].transform.localPosition = new Vector3(temp2, 40, 0);
                temp2 = temp2 + tempNegsAI[k].GetComponent<ValueCard>().value * offset;
            }
        }


        
        //fix offset issue - line up not the cards but the squares
        //fix layer issue when new card enters list
    }
    
}