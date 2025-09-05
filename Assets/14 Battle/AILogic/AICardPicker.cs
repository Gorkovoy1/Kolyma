using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class AICardPicker : MonoBehaviour
{
    //give 2 or 1 
    //2 is highest priority
    //1 is lower priority
    //0 is no
    public int defensive;
    public int offensive;


    public int discard;
    public int flip;
    public int swap;
    public int draw;

    public int randomPoint;

    public HandController handController;

    public List<GameObject> playableCards;

    public Difficulty difficulty;

    public bool randomChanceOn; //if on then discard, flip, swap, draw are not predetermined qualities

    public GameObject selectedCard;

    public AIController aiController;

    public bool executingTurn = false;
    // Start is called before the first frame update
    void Start()
    {
        if(randomChanceOn)
        {
            discard = Random.Range(1, 3);
            flip = Random.Range(1, 3);
            swap = Random.Range(1, 3);
            draw = Random.Range(1, 3);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!TurnManager.instance.isPlayerTurn && !executingTurn)
        {
            executingTurn = true;
            ChooseBestCard();
        }
    }

    public void ChooseBestCard()
    {
        //depending on points or difficulty, go through all cards in hand, find the playable ones, then evaluate which is best card to play
        //put all playable cards in hand into a list

        //depending on assigned values in inspector, assign points to cards in hand, sort in order of points, pick random from top 3 to play
        playableCards.Clear();

        foreach (GameObject g in handController.opponentSpecialHand)
        {
            if(g.GetComponent<AICardPlace>().isPlayable)
            {
                playableCards.Add(g);
            }

        }

        foreach(GameObject g in playableCards)
        {
            if(g.GetComponent<AICardPlace>().personalityType == PersonalityType.Defensive)
            {
                g.GetComponent<AICardPlace>().points = defensive;
            }
            else if (g.GetComponent<AICardPlace>().personalityType == PersonalityType.Offensive)
            {
                g.GetComponent<AICardPlace>().points = offensive;
            }
            else if (g.GetComponent<AICardPlace>().personalityType == PersonalityType.Discard)
            {
                g.GetComponent<AICardPlace>().points = discard;
            }
            else if (g.GetComponent<AICardPlace>().personalityType == PersonalityType.Flip)
            {
                g.GetComponent<AICardPlace>().points = flip;
            }
            else if (g.GetComponent<AICardPlace>().personalityType == PersonalityType.Swap)
            {
                g.GetComponent<AICardPlace>().points = swap;
            }
            else if (g.GetComponent<AICardPlace>().personalityType == PersonalityType.Draw)
            {
                g.GetComponent<AICardPlace>().points = draw;
            }
        }

        playableCards = playableCards.OrderByDescending(g => g.GetComponent<AICardPlace>().points).ToList();

        selectedCard = playableCards[Random.Range(0, 3)];

        aiController.selectedCardToPlay = selectedCard;
        aiController.PlayCard();

        StartCoroutine(DelayTurn());

    }

    IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(2f);
        TurnManager.instance.isPlayerTurn = true;
        executingTurn = false;
    }
}

public enum PersonalityType
{
    Defensive, //play defensive cards always
    Offensive, //play offensive cards always
    Discard,
    Flip,
    Swap,
    Draw
}

//mixed? when player vs opp abs values, if they are closer than u save urself, once you are close enough start attacking them
//but if they only play defensive, then what if you are already close to the value? screw urself over?
