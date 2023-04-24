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

    public List<int> playerValues;
    public List<int> opponentValues;
    public List<GameObject> currentPlayerValues;
    public List<GameObject> currentOpponentValues;

    public int cardCount;
    public int valueCount;
    public int opponentCardCount;

    public int playerSum;
    public int opponentSum;
    public int targetValue;

    public int tempValue;


    public bool specialDraw;
    bool quit = false;


    public int tempLastValue;
    public int oppTempLastValue;
    public int negMultiple;

    public int multiple;
    public int oppMultiple;
     



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


        //maybe have an updating script where it takes the list of prefab objects and constantly reorders them into the right position
        //StartCoroutine(DelayCode());

        
        //player value organizing
        int currentPos = -60;
        List<GameObject> negatives = new List<GameObject>();
        List<GameObject> positives = new List<GameObject>();
        List<int> positives2 = new List<int>();
        List<int> negatives2 = new List<int>();

        for(int j = 0; j < currentPlayerValues.Count; j++)
        {
            if(playerValues[j] > 0)
            {
                positives.Add(currentPlayerValues[j]);
                positives2.Add(playerValues[j]);
            }
            else
            {
                negatives.Add(currentPlayerValues[j]);
                negatives2.Add(playerValues[j]);
            }
        }

        for(int i = 1; i < positives.Count; i++)
        {
            positives[0].transform.localPosition = new Vector3(-60, 111, 0);
            int multiple = positives2[i-1];
            positives[i].transform.localPosition = new Vector3(currentPos + multiple * 20, 111, 0);
            currentPos = currentPos + multiple * 20;
            tempLastValue = currentPos;
        }

        if (negatives.Count >= 1)
        {
            if(positives.Count == 1)
            {
                
                //negatives[0].transform.localPosition = new Vector3(tempLastValue - multiple * 20, 36, 0);
                //tempLastValue = tempLastValue - multiple*20;
                negatives[0].transform.localPosition = new Vector3(-60, 36, 0);
                tempLastValue = -60;
            }
            else
            {
                negatives[0].transform.localPosition = new Vector3(tempLastValue, 36, 0);
            }
            
        }
        for(int k = 1; k < negatives.Count; k++)
        {
            int multiple2 = negatives2[k-1];
            negatives[k].transform.localPosition = new Vector3(tempLastValue + multiple2 * 20, 36, 0);
        }
        //fix if 3 negative values positioning is wrong


        //opponent values organizing
        int oppCurrentPos = -60;
        List<GameObject> oppNegatives = new List<GameObject>();
        List<GameObject> oppPositives = new List<GameObject>();
        List<int> oppPositives2 = new List<int>();
        List<int> oppNegatives2 = new List<int>();

        for(int j = 0; j < currentOpponentValues.Count; j++)
        {
            if(opponentValues[j] > 0)
            {
                oppPositives.Add(currentOpponentValues[j]);
                oppPositives2.Add(opponentValues[j]);
            }
            else
            {
                oppNegatives.Add(currentOpponentValues[j]);
                oppNegatives2.Add(opponentValues[j]);
            }
        }

        for(int i = 1; i < oppPositives.Count; i++)
        {
            oppPositives[0].transform.localPosition = new Vector3(-60, 266, 0);
            int oppMultiple = oppPositives2[i-1];
            oppPositives[i].transform.localPosition = new Vector3(oppCurrentPos + oppMultiple * 20, 266, 0);
            oppCurrentPos = oppCurrentPos + oppMultiple * 20;
            oppTempLastValue = oppCurrentPos;
        }

        if (oppNegatives.Count >= 1)
        {
            if(oppPositives.Count == 1)
            {
                //oppTempLastValue = oppTempLastValue + oppMultiple*20;
                //oppNegatives[0].transform.localPosition = new Vector3(oppTempLastValue, 191, 0);
                oppNegatives[0].transform.localPosition = new Vector3(-60, 191, 0);
                oppTempLastValue = -60;
                
            }
            else
            {
                oppNegatives[0].transform.localPosition = new Vector3(oppTempLastValue, 191, 0);
            }
        }
        for(int k = 1; k < oppNegatives.Count; k++)
        {
            int oppMultiple2 = oppNegatives2[k-1];
            oppNegatives[k].transform.localPosition = new Vector3(oppTempLastValue + oppMultiple2 * 20, 191, 0);
        }

    }
    

    IEnumerator DelayCode()
    {
        float counter = 0;
        float waitTime = 3;

        while(counter<waitTime)
        {
            //Increment Timer until counter >= waitTime
            counter += Time.deltaTime;
            //Debug.Log("We have waited for: " + counter + " seconds");
            //Wait for a frame so that Unity doesn't freeze
            //Check if we want to quit this function
            if (quit)
            {
                //Quit function
                yield break;
            }
            yield return null;
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
            playerValues.Add(tempValue);
            currentPlayerValues.Add(valueCard);
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
            opponentValues.Add(tempValue2);
            currentOpponentValues.Add(valueCard2);
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

    public void DrawOne()
    {
        GameObject cardDraw = (GameObject) Instantiate(specialList[cardCount+1]);
        cardDraw.transform.SetParent(handParent, false);
        cardCount++;
    }
}