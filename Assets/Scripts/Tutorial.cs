using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> valueDeck;
    public List<GameObject> playerSpecialDeck;
    public List<GameObject> AISpecialDeck;
    public List<GameObject> playerSpecials;
    public List<GameObject> AISpecials;
    public List<GameObject> playerValues;
    public List<GameObject> AIValues;
    public List<GameObject> tempNegsPlayer;
    public List<GameObject> tempNegsAI;
    public List<GameObject> AIAttacks;
    public List<GameObject> AIHelps;
    public List<GameObject> playableCards;
    public List<GameObject> unplayableCards;
    public List<GameObject> played;
    List<GameObject> combinedListAI;
    List<GameObject> combinedListPlayer;

    public int AIValue;
    public int playerValue;
    public int targetValue;
    public int offset;
    public int cardsSwapped;
    public int AICardsSwapped;
    public int cardsDiscarded;
    public int playerCardsDiscarded;
    public int cardsGiven;
    public int AICardsGiven;
    public int playerSum;
    public int opponentSum;
    public int AIDice;
    public int playerDice;
    public int turnCounter;

    public Button passButton;

    public GameObject diceOne;
    public GameObject diceTwo;
    public GameObject diceThree;
    public GameObject diceFour;
    public GameObject dicePrefab;

    public TextMeshProUGUI playerSumText;
    public TextMeshProUGUI opponentSumText;
    public TextMeshProUGUI targetValueText;

    bool quit = false;
    public bool hasDiscarded;
    public bool playerHasDiscarded;
    public bool gameEnd;
    public bool GameOver;
    public bool cardSelected;
    public bool hasPlayable;
    public bool attack;
    public bool nonePlayable;

    public Transform board;
    public Transform playerHand;
    public Transform AIHand;

    
    // Start is called before the first frame update
    void Start()
    {
        cardsSwapped = 0;
        turnCounter = 0;
        cardSelected = false;
        GameOver = false;
        hasDiscarded = false;
        playerHasDiscarded = false;




        //player clicks to draw 4 cards
        //andreev draws 4
        //dice flash, click and roll
        //the sum is 17, draw 6 from special deck
        //explanation of goal and cards
        //since player has 12 and andreev has 5, player starts





        RollDice();
        ShuffleSpecials();
        DealSpecials();
        ShuffleDeck();
        DealCards();
        OrganizeCards();

        StartCoroutine(PlayTurn());
        CheckPlayable();
        SortCards();

        
    }

    // Update is called once per frame
    void Update()
    {
        playerSumText.text = playerSum.ToString();
        opponentSumText.text = opponentSum.ToString();

        if(GameOver)
        {
            GameOverScreen();
        }

        if(turnCounter % 2 == 0)
        {
            passButton.gameObject.SetActive(true);
        }
        else
        {
            passButton.gameObject.SetActive(false);
        }
    }

    void Awake()
    {

    }

    void RollDice()
    {
        //roll dice
        diceOne = (GameObject) Instantiate(dicePrefab);
        diceOne.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceOne.transform.SetParent(board, false);
        diceOne.transform.localPosition = new Vector3(-180, 75, 0);

        diceTwo = (GameObject) Instantiate(dicePrefab);
        diceTwo.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceTwo.transform.SetParent(board, false);
        diceTwo.transform.localPosition = new Vector3 (-205, 75, 0);

        diceThree = (GameObject) Instantiate(dicePrefab);
        diceThree.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceThree.transform.SetParent(board, false);
        diceThree.transform.localPosition = new Vector3(-180, -100, 0);

        diceFour = (GameObject) Instantiate(dicePrefab);
        diceFour.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
        diceFour.transform.SetParent(board, false);
        diceFour.transform.localPosition = new Vector3(-205, -100, 0);


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


        targetValueText.text = "Target Value: " + targetValue.ToString();

        AIDice = diceOne.GetComponent<Dice>().finalSide + diceTwo.GetComponent<Dice>().finalSide;
        playerDice = diceThree.GetComponent<Dice>().finalSide + diceFour.GetComponent<Dice>().finalSide;

        if(AIDice <= playerDice)
        {
            turnCounter = 0;
        }
        else
        {
            turnCounter = 1;
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

    public void DrawSpecial(List<GameObject> playerSpecialDeck, Transform playerHand)
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

    public void AIDrawSpecial()
    {
        if(AISpecialDeck.Count > 0)
        {
            GameObject drawnSpecial = (GameObject) Instantiate(AISpecialDeck[0]);
            drawnSpecial.transform.SetParent(AIHand, false);
            drawnSpecial.transform.localPosition = new Vector3(0, 100, 0);
            AISpecials.Add(drawnSpecial);
            playerSpecialDeck.RemoveAt(0);
            if(drawnSpecial.GetComponent<Draggable>().attack == true)
            {
                AIAttacks.Add(drawnSpecial);
            }
            else
            {
                AIHelps.Add(drawnSpecial);
            }
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

    public void CalculateValue()
    {
        //add all values in both lists
        combinedListAI = new List<GameObject>();
        combinedListAI.AddRange(AIValues);
        combinedListAI.AddRange(tempNegsAI);
        opponentSum = 0;
        playerSum = 0;
        foreach(GameObject i in combinedListAI)
        {
            opponentSum += i.GetComponent<ValueCard>().value;
        }
        combinedListPlayer = new List<GameObject>();
        combinedListPlayer.AddRange(playerValues);
        combinedListPlayer.AddRange(tempNegsPlayer);
        foreach(GameObject k in combinedListPlayer)
        {
            playerSum += k.GetComponent<ValueCard>().value;
        }
    }

    public void OrganizeCards()
    {
        OrganizeValueList();
        CalculateValue();

        int playerPos = 0;
        int AIPos = 0;
        int temp = 0;
        int temp2 = 0;

        int tempCount = 0;
        int tempCount2 = 0;

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
    }

    void GameOverScreen()
    {
        if(Mathf.Abs(targetValue - playerSum) > Mathf.Abs(targetValue - opponentSum))
        {
            Debug.Log("you lose");
        }
        else if(Mathf.Abs(targetValue - playerSum) < Mathf.Abs(targetValue - opponentSum))
        {
            Debug.Log("you win");
        }
        else
        {
            Debug.Log("its a draw");
        }
    }

    IEnumerator PlayTurn()
    {
        yield return new WaitForSeconds(5);

        while(!GameOver)
        {
            if(turnCounter % 2 == 0)
            {
                cardSelected = false;
                yield return StartCoroutine(WaitForPlayerInput());
            }
            else
            {
                yield return StartCoroutine(AITurnCode());
            }

            yield return null;
        }
    }

    IEnumerator AITurnCode()
    {
        Debug.Log("AI turn");
    
        if (Mathf.Abs(opponentSum - targetValue) <= playerSum - targetValue) // AI closer
        {
            Debug.Log("AI closer");
        
            if (AIAttacks.Count > 0)
            {
                PlayAttack();
                yield return null;
            
                if (nonePlayable && AIHelps.Count > 0)
                {
                    PlayHelp();
                    yield return null;
                
                    if (nonePlayable)
                    {
                        Pass();
                        yield return null;
                    }
                }
                else if (nonePlayable)
                {
                    Pass();
                    yield return null;
                }
            }
            else if (AIHelps.Count > 0)
            {
                PlayHelp();
                yield return null;
            
                if (nonePlayable)
                {
                    Pass();
                    yield return null;
                }
            }
            else
            {
                Pass();
                yield return null;
            }
        }
        else // AI farther, player closer
        {
            Debug.Log("player closer");
        
            if (AIHelps.Count > 0)
            {
                PlayHelp();
                yield return null;
            
                if (nonePlayable && AIAttacks.Count > 0)
                {
                    PlayAttack();
                    yield return null;
                
                    if (nonePlayable)
                    {
                        Pass();
                        yield return null;
                    }
                }
                else if (nonePlayable)
                {
                    Pass();
                    yield return null;
                }
            }
            else if (AIAttacks.Count > 0)
            {
                PlayAttack();
                yield return null;
            
                if (nonePlayable)
                {
                    Pass();
                    yield return null;
                }
            }
            else
            {
                Pass();
                yield return null;
            }
        }
    
        yield return null;
    }

    void PlayAttack()
    {
        foreach(GameObject i in AIAttacks)
        {
            if(i != null)
            {
                nonePlayable = true;
                int newValue = i.GetComponent<Draggable>().addValue + playerSum;
                if(Mathf.Abs(targetValue - newValue) >= Mathf.Abs(targetValue - playerSum)) //if attack puts player farther
                {
                    i.GetComponent<Draggable>().ExecuteCard();
                    turnCounter++;
                    Debug.Log("play attack");
                    nonePlayable = false;
                    break;
                }
            }
        }
    }

    void PlayHelp()
    {
        foreach(GameObject i in AIHelps)
        {
            if(i != null)
            {
                nonePlayable = true;
                int newValue = i.GetComponent<Draggable>().addValue + opponentSum;
                if(Mathf.Abs(targetValue - newValue) <= Mathf.Abs(targetValue - opponentSum)) //if help helps
                {
                    i.GetComponent<Draggable>().ExecuteCard();
                    turnCounter++;
                    Debug.Log("play help");
                    nonePlayable = false;
                    break;
                }
            }
        }
    }

    void Pass()
    {
        turnCounter++;
        Debug.Log("pass");
    }

    IEnumerator WaitForPlayerInput()
    {
        cardSelected = false;
        while(!cardSelected)
        {
            yield return null;
        }
        turnCounter++;
    }

    void SortCards()
    {
        for(int i = playableCards.Count - 1; i > -1; i--)
        {
            if(playableCards[i].GetComponent<Draggable>().attack == true)
            {
                AIAttacks.Add(playableCards[i]);
                //playableCards.RemoveAt(i);
            }
            else
            {
                AIHelps.Add(playableCards[i]);
                //playableCards.RemoveAt(i);
            }
        }
    }             
    
    void CheckPlayable()
    {
        for(int x = AISpecials.Count - 1; x > -1; x--)
        {
            if(AISpecials[x].GetComponent<Draggable>().duplicate)
            {
                List<GameObject> myPrefabList = new List<GameObject>();
                //find dupes
                for(int j = 0; j < AIValues.Count; j++)
                {
                    myPrefabList.Add(AIValues[j]);
                }
                for(int k = 0; k < tempNegsAI.Count; k++)
                {
                    myPrefabList.Add(tempNegsAI[k]);
                }

                var duplicateGroups = myPrefabList.GroupBy(x => x.GetComponent<ValueCard>().value)
                    .Where(g => g.Count() > 1)
                    .Select(g => new { Value = g.Key, Objects = g.ToList() });

                // Moving one duplicate to the duplicatesList
        
                if(duplicateGroups.Any())
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().two)
            {
                GameObject two = playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 2);
                if(two != null)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().seven)
            {
                GameObject seven = playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 7);
                if(seven != null)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().eight)
            {
                GameObject eight = playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 8);
                if(eight != null)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().nine)
            {
                GameObject nine = playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 9);
                if(nine != null)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().youNegative)
            {
                if(tempNegsAI.Count > 0)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().theyDiscarded)
            {
                if(playerHasDiscarded == true)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().five)
            {
                GameObject five = playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 5);
                if(five != null)
                {
                    if(!playableCards.Contains(AISpecials[x]))
                    playableCards.Add(AISpecials[x]);
                }
            }

            if(AISpecials[x].GetComponent<Draggable>().conditionMet)
            {
                if(!playableCards.Contains(AISpecials[x]))
                playableCards.Add(AISpecials[x]);
            }
        }
    }
}


            