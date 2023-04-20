using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    
    public GameObject cardPrefab;
    public GameObject dicePrefab;
    public GameObject hand;
    /*
    public GameObject player;
    public GameObject opponent;
    */

    public GameObject diceOne;
    public GameObject diceTwo;
    public GameObject diceThree;
    public GameObject diceFour;
    

    public Transform handParent;
    public Transform opponentHandParent;
    public Transform playerParent;
    public Transform opponentParent;
    public Transform board;
    public Transform boardArt;

    public TextMeshProUGUI playerSumText;
    public TextMeshProUGUI opponentSumText;
    public TextMeshProUGUI targetValueText;
    

    public List<GameObject> valuePrefabs;
    public List<GameObject> specialPrefabs;
    public List<GameObject> opponentPrefabs;
    
    public List<GameObject> valueList;
    public List<GameObject> specialList;
    public List<GameObject> opponentList;

    public int cardCount;
    public int valueCount;
    public int opponentCardCount;

    public int playerSum;
    public int opponentSum;
    public int targetValue;

    public int tempValue;


    public bool specialDraw;
    bool quit = false;


    void Start()
    {
        
    }


    void Awake()
    {
        
        specialDraw = false;
        cardCount = 0;
        valueCount = 0;
        playerSum = 0;
        opponentSum = 0;
        Shuffle();
        DealCards();
        DealValues();

        
        //roll dice
        diceOne = (GameObject) Instantiate(dicePrefab);
        diceOne.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceOne.transform.SetParent(board, false);
        diceOne.transform.position = new Vector3(755, 350, 0);

        diceTwo = (GameObject) Instantiate(dicePrefab);
        diceTwo.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceTwo.transform.SetParent(board, false);
        diceTwo.transform.position = new Vector3 (730, 350, 0);

        diceThree = (GameObject) Instantiate(dicePrefab);
        diceThree.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceThree.transform.SetParent(board, false);
        diceThree.transform.position = new Vector3(755, 200, 0);

        diceFour = (GameObject) Instantiate(dicePrefab);
        diceFour.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceFour.transform.SetParent(board, false);
        diceFour.transform.position = new Vector3(730, 200, 0);

        StartCoroutine(targetCalculate());
        
    }

    IEnumerator targetCalculate()
    {
        float counter = 0;
        float waitTime = 3;
        int diceValue;

        while(counter<waitTime)
        {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            Debug.Log("We have waited for: " + counter + " seconds");
            //Wait for a frame so that Unity doesn't freeze
            //Check if we want to quit this function
            if (quit)
            {
                //Quit function
                yield break;
            }
            yield return null;
        }
        
        diceValue = diceOne.GetComponent<Dice>().finalSide;
        targetValue = targetValue + diceValue;
        

        diceValue = diceTwo.GetComponent<Dice>().finalSide;
        targetValue = targetValue + diceValue;


        diceValue = diceThree.GetComponent<Dice>().finalSide;
        targetValue = targetValue + diceValue;


        diceValue = diceFour.GetComponent<Dice>().finalSide;
        targetValue = targetValue + diceValue;



        targetValueText.text = "Target\nValue:\n" + targetValue.ToString();
    }

    void Update()
    {
        int diceValue;

        playerSumText.text = playerSum.ToString();
        opponentSumText.text = opponentSum.ToString();

        if(specialDraw)
        {
            DrawTwo();
            specialDraw = false;
        }
        else
        {

        }

    }



    void Shuffle()
    {
        for(int i = 0; i < 40; i++)
        {
            GameObject temp = valuePrefabs[i];
            int rand = Random.Range(0,40);
            valuePrefabs[i] = valuePrefabs[rand];
            valuePrefabs[rand] = temp;
        }

        foreach(GameObject i in valuePrefabs)
        {
            valueList.Add(i);
        }

        for(int k=0; k < 18; k++)
        {
            GameObject tempTwo = specialPrefabs[k];
            int randTwo = Random.Range(0,18);
            specialPrefabs[k] = specialPrefabs[randTwo];
            specialPrefabs[randTwo] = tempTwo;
        }

        foreach(GameObject k in specialPrefabs)
        {
            specialList.Add(k);
        }

        for(int j=0; j < 18; j++) //for reference, in future this will be length of array
        {
            GameObject tempThree = opponentPrefabs[j];
            int randThree = Random.Range(0,18);
            opponentPrefabs[j] = opponentPrefabs[randThree];
            opponentPrefabs[randThree] = tempThree;
        }

        foreach(GameObject j in opponentPrefabs)
        {
            opponentList.Add(j);
        }

    }
    


    public void DealCards()
    {
        cardCount = 0;
        
       

        for(int k = 0; k < 6; k++)
        {
            

            GameObject card = (GameObject) Instantiate(specialList[k+1]);
            
            card.transform.SetParent(handParent, false);

            cardCount++;
            Debug.Log(" "+ cardCount);

        }

        //opponent hand
        for(int j = 0; j < 6; j++)
        {
            GameObject card2 = (GameObject) Instantiate(opponentList[j+1]);
            
            card2.transform.SetParent(opponentHandParent, false);

            opponentCardCount++;
            Debug.Log(" "+ opponentCardCount);

        }
    }

    public void DealValues()
    {
        valueCount = 0;
        int tempValue = 0;
        int lastValue = 0;
        int tempValue2 = 0;
        int lastValue2 = 0;
        int offset1 = 0;
        int offset2 = 0;

        for (int n = 0; n < 8; n+=2)
        {
            //player
            GameObject valueCard = (GameObject) Instantiate(valueList[n]);
            //get card value from prefab, set it as value, translate card by unit * value, add value to total sum
            //get component, find integer, idk how
            //if negative, do something
            //get value, check for positive or negative, do positives first then negative
            tempValue = valueCard.GetComponent<Tile>().cardValue;
            lastValue = tempValue;
            Debug.Log(tempValue);
                if(tempValue > 0)
                {
                    playerSum = playerSum + tempValue;
                    valueCard.transform.localPosition = new Vector3(-60, -40, 0);
                    valueCard.transform.SetParent(board, false);
                    valueCount++;
                }
                else
                {
                    playerSum = playerSum + tempValue;
                    valueCard.transform.localPosition = new Vector3(-60, -115, 0);
                    valueCard.transform.SetParent(board, false);
                    valueCount++;
                }



            //opponent
            GameObject valueCard2 = (GameObject) Instantiate(valueList[n+1]);
            tempValue2 = valueCard2.GetComponent<Tile>().cardValue;
            lastValue2 = tempValue2;
                if(tempValue2 > 0)
                {
                    opponentSum = opponentSum + tempValue2;
                    valueCard2.transform.localPosition = new Vector3(-60, 120, 0);
                    valueCard2.transform.SetParent(board, false);
                    valueCount++;
                }
                else
                {
                    opponentSum = opponentSum + tempValue2;
                    valueCard2.transform.localPosition = new Vector3(-60, 45, 0);
                    valueCard2.transform.SetParent(board, false);
                    valueCount++;
                }


        Debug.Log("Opponent value is: " + opponentSum);
        Debug.Log("Your value is: " + playerSum);

        }
    }

    public void DrawTwo()
    {
        Debug.Log("drawtwo");
        GameObject cardOne = (GameObject) Instantiate(specialList[cardCount+1]);
        cardOne.transform.SetParent(handParent, false);
        cardCount++;
        GameObject cardTwo = (GameObject) Instantiate(specialList[cardCount+1]);
        cardTwo.transform.SetParent(handParent, false);
        cardCount++;
    }
}