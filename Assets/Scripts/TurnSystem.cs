using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnSystem : MonoBehaviour
{
    public int turnCounter;
    public bool GameOver;
    public bool cardSelected;
    public bool hasPlayable;
    public GameManager gm;

    public List<GameObject> AIAttacks;
    public List<GameObject> AIHelps;
    public List<GameObject> playableCards;
    public List<GameObject> unplayableCards;
    public List<GameObject> specialCopy;

    public bool attack;

    public int AIValue;
    public int playerValue;
    public int targetValue;

    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");
        gm = manager.GetComponent<GameManager>();
        turnCounter = 0; //even is player turn odd is AI
        //if the dice value is higher, +1 if not, do nothing
        cardSelected = false;
        GameOver = false;
        StartCoroutine(PlayTurn());
        //check playable each turn
    }

    void Update()
    {
        
    }

    private IEnumerator PlayTurn()
    {
        while (!GameOver)
        {
            if (turnCounter % 2 == 0)
            {
                // Wait for player input
                yield return WaitForPlayerInput();
                
                
            }
            else
            {
                Debug.Log("AI Taking Turn");
                CheckPlayable();
                if(Mathf.Abs(gm.AIValue - gm.targetValue) > Mathf.Abs(gm.playerValue - gm.targetValue))
                {
                    //if have attack card
                    //play attack card
                }

                if(Mathf.Abs(gm.AIValue - gm.targetValue) > Mathf.Abs(gm.playerValue - gm.targetValue))
                {
                    //if have help card
                    //play help card
                }

                // AI logic for selecting a card

                //if have playable cards
                //if closer to target, play attack card
                //find and pick random attack card 
                //if no card, compare difference with help card 
                //play it or dont play anything and pass
                //if dont have anything playable, skip


                //if player closer, play help card
                //find and pick random help card that will help closer
                //if dont have play attack card with largest effect
                //else do nothing and skip
                turnCounter++;
            }

            

            yield return null;
        }
    }


    private IEnumerator WaitForPlayerInput()
    {
        cardSelected = false;
        // Wait until the player has made a selection
        while (!cardSelected)
        {
            yield return null;
        }
        turnCounter++;
    }

    void AITurn()
    {
        
        //access hand and sort cards
        foreach(GameObject i in playableCards)
        {
            
            if(i.GetComponent<Draggable>().attack == true)
            {
                List<GameObject> AIAttacksTemp = new List<GameObject>();
                AIAttacksTemp.Add(i);
                AIAttacks = AIAttacksTemp;
            }
            else if(i.GetComponent<Draggable>().help == true)
            {
                List<GameObject> AIHelpsTemp = new List<GameObject>();
                AIHelpsTemp.Add(i);
                AIHelps = AIHelpsTemp;
            }
        }

        //bool AIPlayable - put in script, if number exists in player values then make true
        //depending on what bools, it will know what condition, then can determine if playable or not
    }

    void SortCards()
    {
        for(int i = playableCards.Count - 1; i > -1; i--)
        {
            if(playableCards[i].GetComponent<Draggable>().attack == true)
            {
                AIAttacks.Add(playableCards[i]);
            }
            else
            {
                AIHelps.Add(playableCards[i]);
            }
        }
    }

    void CheckPlayable()
    {
        for(int x = gm.AISpecials.Count - 1; x > -1; x--)
        {
            if(gm.AISpecials[x].GetComponent<Draggable>().duplicate)
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
        
                if(duplicateGroups.Any())
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(gm.AISpecials[x]);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().two)
            {
                GameObject two = gm.playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 2);
                if(two != null)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().seven)
            {
                GameObject seven = gm.playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 7);
                if(seven != null)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().eight)
            {
                GameObject eight = gm.playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 8);
                if(eight != null)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().nine)
            {
                GameObject nine = gm.playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 9);
                if(nine != null)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().youNegative)
            {
                if(gm.tempNegsAI.Count > 0)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().theyDiscarded)
            {
                if(gm.playerHasDiscarded == true)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().five)
            {
                GameObject five = gm.playerValues.Find(obj => obj.GetComponent<ValueCard>().value == 5);
                if(five != null)
                {
                    if(!playableCards.Contains(gm.AISpecials[x]))
                    playableCards.Add(gm.AISpecials[x]);
                    //specialCopy.Remove(x);
                }
            }

            if(gm.AISpecials[x].GetComponent<Draggable>().conditionMet)
            {
                if(!playableCards.Contains(gm.AISpecials[x]))
                playableCards.Add(gm.AISpecials[x]);
                //specialCopy.Remove(x);
            }

        }

        
        
    }

    

}

/*
for AI

all code, no game objects
read what cards are in AI hand
separate into attack cards and defense card lists
detect player hand and cards
find card condition
see which cards are playable and which ones are not
if has condition, change has playable card in TS to true
if true, look at closeness to target


if closer, play random attack card
if no attack card, compare difference with help card - play it or dont play and pass
if nothing playable, skip


if player closer, play help card
pick random help card that will not put value over
if no help card, play largest effect attack card
if nothing playable, skip

*/

//separate to playable and unplayable
//or separate to attack and help and remove unplayable cards


