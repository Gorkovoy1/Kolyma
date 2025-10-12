using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


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

    public bool opponentUsedAction = false;

    public PassAnimationController passAnimationController;
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
            TurnManager.instance.opponentPassed = false;
            TurnManager.instance.opponentPlayedCard = false;

            //if busted, flip or swap a card - if ideal flip, if not ideal, swap //ACTION
            if (NumberManager.instance.oppVal > NumberManager.instance.targetVal && !opponentUsedAction)
            {
                //sort all opp numbers
                List<GameObject> list = NumberManager.instance.OPPallNumbers;
                list.Sort((a, b) =>
                    a.GetComponent<NumberStats>().value.CompareTo(b.GetComponent<NumberStats>().value)
                );

                if (difficulty == Difficulty.Ideal)
                {
                    //flip middle number
                    int i = list.Count / 2;
                    GameObject g = list[i];
                    
                    
                    StartCoroutine(CardSelectionController.instance.FlipNumber(g));
                    opponentUsedAction = true;
                }
                else
                {
                    //flip largest number
                    int i = list.Count - 1;
                    GameObject g = list[i];
                    StartCoroutine(CardSelectionController.instance.FlipNumber(g));
                    opponentUsedAction = true;
                }
                NumberManager.instance.oppAction = true;
                TurnManager.instance.opponentPlayedCard = true;

                StartCoroutine(DelayTurn());
                //ChooseBestCard();
            }
            else if ((NumberManager.instance.oppVal <= NumberManager.instance.targetVal && NumberManager.instance.playerVal > NumberManager.instance.targetVal))
            {
                //pass
                StartCoroutine(aiController.DelaySkipTurn());
            }
            else if(Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.oppVal) < Mathf.Abs(NumberManager.instance.targetVal - NumberManager.instance.playerVal))
            {
                //pass
                StartCoroutine(aiController.DelaySkipTurn());
            }
            else
            {
                ChooseBestCard();
            }
            
            
        }
    }

    public void ChooseBestCard()
    {
        //depending on points or difficulty, go through all cards in hand, find the playable ones, then evaluate which is best card to play
        //put all playable cards in hand into a list

        //depending on assigned values in inspector, assign points to cards in hand, sort in order of points, pick random from top 3 to play
        playableCards.Clear();
        NumberManager.instance.recalculate = true;

        foreach (Transform c in handController.opponentHand.transform)
        {
            if(c.GetComponent<AICardPlace>().isPlayable)
            {
                playableCards.Add(c.gameObject);
            }
            else
            {
                Debug.Log("no add");
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

        if (playableCards.Count > 0)
        {
            // pick between 0 and either 3 or the count, whichever is smaller
            int maxIndex = Mathf.Min(3, playableCards.Count);

            selectedCard = playableCards[Random.Range(0, maxIndex)];

            aiController.selectedCardToPlay = selectedCard;
            aiController.PlayCard();
        }
        else
        {
            Debug.LogWarning("No playable cards available!");
            TurnManager.instance.opponentPlayedCard = false;
            TurnManager.instance.opponentPassed = true;
            passAnimationController.oppPass = true;
        }

        StartCoroutine(DelayTurn());

    }

    public IEnumerator DelayTurn()
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
