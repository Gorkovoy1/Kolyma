using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
 
    

public class AICardPlace : MonoBehaviour //AICardPlace

{
    public bool isDragging;
    public Transform parentReturnTo = null;
    public GameObject imagePrefab;
    public GameObject correspondingImage;
    public Transform imagesParent;

    public float hoverOffset;
    public bool hovering;
    public bool dragging;
    public bool beingPlayed;

    public GameObject playerDiscardZone;
    public GameObject opponentDiscardZone;

    public Transform playerHand;
    public Transform opponentHand;

    private bool numberCard;

    public int childCount;

    public SpecialCardType specialCardType;

    private float duration = 1f;

    public bool isPlayable; //for the player


    public Material defaultMat;
    public Material playableMat;

    public List<GameObject> discardedCards;
    public bool discardUpdated;

    public Sprite cardBack;
    public Sprite grey;

    //public bool isFlipping;
    public bool isFlipped;

    public Difficulty difficulty;
    public PersonalityType personalityType;

    public int points;

    public int selfValue = 0; //check for self bust

    

    // Start is called before the first frame update
    void Start()
    {
        discardUpdated = false;
        isFlipped = false;

        opponentHand = this.transform.parent;
        playerHand = opponentHand.parent.Find("PlayerHand");
        parentReturnTo = opponentHand;
        //set images parent
        //set discard zone obj
        playerDiscardZone = GameObject.FindWithTag("PlayerDiscard");
        opponentDiscardZone = GameObject.FindWithTag("OpponentDiscard");

        if (imagePrefab != null)
        {
            //this means its a special card
            imagesParent = this.GetComponentInParent<HandController>().imagesParent;
            correspondingImage = Instantiate(imagePrefab, imagesParent);
            correspondingImage.GetComponent<AISpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();
            defaultMat = correspondingImage.GetComponent<Image>().material;

            //start flipped
            if (this.transform.parent == opponentHand)
            {
                grey = correspondingImage.GetComponent<Image>().sprite;
                correspondingImage.GetComponent<Image>().sprite = cardBack;
                correspondingImage.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
        }
        CheckPlayable();
    }

    // Update is called once per frame
    void Update()
    {

        if (imagePrefab != null)
        {
            //this means its a special card
            if (!TurnManager.instance.isPlayerTurn && this.transform.parent == opponentHand) //check parent is hand
            {
                NumberManager.instance.recalculate = true;
                CheckPlayable();
                if (isPlayable)
                {
                    //outline card in green
                    //change material to outline shader
                    //change back when playing card

                    //note this is happening for opponent cards for some reason
                    correspondingImage.GetComponent<Image>().material = playableMat;
                }
                else
                {
                    isPlayable = false;
                    correspondingImage.GetComponent<Image>().material = defaultMat;
                }
            }


        }

        //track discarded cards, update whenever becomes player turn
        if (!TurnManager.instance.isPlayerTurn /*&& !discardUpdated*/)          //uncomment when have turns
        {
            discardedCards = new List<GameObject>();

            foreach (Transform child in opponentDiscardZone.transform)
            {
                discardedCards.Add(child.gameObject);
            }

            discardUpdated = true;
        }

    }

    //AnimateBeingPlayed(g);

    public void AnimateBeingPlayed()
    {
        isPlayable = false;
        correspondingImage.GetComponent<Image>().material = defaultMat;
        StartCoroutine(BeingPlayed());

    }

    IEnumerator BurnShader(GameObject g)
    {
        RawImage rawImage = g.GetComponent<CardPlace>().correspondingImage.GetComponent<RawImage>();

        Material mat = new Material(rawImage.materialForRendering);

        rawImage.material = mat;

        float startFade = mat.GetFloat("_Fade");
        float endFade = 0f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float lerpVal = Mathf.Lerp(startFade, endFade, t / duration);
            mat.SetFloat("_Fade", lerpVal);
            yield return null;
        }

        mat.SetFloat("_Fade", endFade);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator BeingPlayed()
    {
        this.transform.SetParent(null, true);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(130f, 250f, 0f);
        correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FlipOverCard(this.gameObject));
        yield return new WaitForSeconds(1f);

        correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        this.transform.SetParent(opponentDiscardZone.transform);
        this.transform.position = opponentDiscardZone.transform.position;

        StartCoroutine(PlayCorrespondingAction());
    }

    public void CheckPlayable()
    {
        switch (specialCardType)
        {
            case SpecialCardType.CaughtRedHanded:
                if (NumberManager.instance.yellows.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.EmptyPockets:
                if (NumberManager.instance.blues.Count > 0 && this.GetComponentInParent<HandController>().playerSpecialHand.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Burden:
                if (NumberManager.instance.reds.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.RifleButt:
                if (NumberManager.instance.reds.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.SmokeBreak:
                if (discardedCards.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Weakness:
                if (discardedCards.Count > 0)
                {
                    foreach (GameObject g in NumberManager.instance.positives)
                    {
                        if (g.GetComponent<NumberStats>().value == 2)
                        {
                            isPlayable = true;
                            break;
                        }
                    }
                }
                break;

            case SpecialCardType.ThickWoolenCoat:
                isPlayable = true;
                break;

            case SpecialCardType.Setup:
                if (NumberManager.instance.negatives.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Bribe:
                if (NumberManager.instance.OPPduplicates.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Fist:
                if (NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.CondensedMilk:
                isPlayable = true;
                break;

            case SpecialCardType.InCahoots:
                isPlayable = true;
                break;

            case SpecialCardType.Search:
                isPlayable = true;
                break;

            case SpecialCardType.Poison:
                if (this.GetComponentInParent<HandController>().playerSpecialHand.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.BackstabDiscard:
                if (PlayerStats.instance.discarded)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Scam:
                isPlayable = true;
                break;

            case SpecialCardType.GiveItUp:
                if (NumberManager.instance.OPPnegatives.Count > 0 && this.GetComponentInParent<HandController>().playerSpecialHand.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Rotation:
                if (NumberManager.instance.OPPnegatives.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.DirtyTrickIV:
                if (NumberManager.instance.yellows.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.BaitAndSwitch:
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value == 2 || g.GetComponent<NumberStats>().value == -2)
                    {
                        isPlayable = true;
                        break;
                    }
                }
                break;

            case SpecialCardType.SelfHarm:
                isPlayable = true;
                break;

            case SpecialCardType.BackstabSwap:
                if (PlayerStats.instance.swapped)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.ThereThere:
                if (NumberManager.instance.OPPduplicates.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.NotMyProblem:
                isPlayable = true;
                break;

            case SpecialCardType.Frostbite:
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value == 2 || g.GetComponent<NumberStats>().value == -2)
                    {
                        isPlayable = true;
                        break;
                    }
                }
                foreach (GameObject g in NumberManager.instance.OPPallNumbers)
                {
                    if (g.GetComponent<NumberStats>().value == 2 || g.GetComponent<NumberStats>().value == -2)
                    {
                        isPlayable = true;
                        break;
                    }
                }
                break;

            case SpecialCardType.DirtyTrickI:
                if (NumberManager.instance.OPPreds.Count > 0 || NumberManager.instance.reds.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.DirtyTrickII:
                if (NumberManager.instance.OPPyellows.Count > 0 || NumberManager.instance.yellows.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.DirtyTrickIII:
                if (NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.LousyDeal:
                isPlayable = true;
                break;

            case SpecialCardType.FindersKeepers:
                if (NumberManager.instance.OPPreds.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Gossip:
                isPlayable = true;
                break;

            case SpecialCardType.FairShare:
                if (NumberManager.instance.negatives.Count > 0)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.SleeplessNight:
                if (PlayerStats.instance.swapped)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Payback:
                if (PlayerStats.instance.gave)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Knife:
                isPlayable = true;
                break;

            case SpecialCardType.ExtraWork:
                if (PlayerStats.instance.flipped)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Scratch:
                if (PlayerStats.instance.gave)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Leftovers:
                if (PlayerStats.instance.discarded)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.Glare:
                isPlayable = true;
                break;

            case SpecialCardType.Snitch:
                if (PlayerStats.instance.discarded)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.GoodDeal:
                isPlayable = true;
                break;

            case SpecialCardType.EasyDay:
                isPlayable = true;
                break;

            case SpecialCardType.GoodFeeling:
                isPlayable = true;
                break;

            case SpecialCardType.Forgery:
                isPlayable = true;
                break;

            case SpecialCardType.Pushover:
                isPlayable = true;
                break;

            case SpecialCardType.Scavenge:
                isPlayable = true;
                break;

            case SpecialCardType.Overwhelmed:
                if (NumberManager.instance.OPPallNumbers.Count > 5)
                {
                    isPlayable = true;
                }
                break;

            case SpecialCardType.VitaminShotII:
                isPlayable = true;
                break;

            case SpecialCardType.VitaminShotV:
                isPlayable = true;
                break;
        }
    }

    IEnumerator PlayCorrespondingAction()
    {
        //
        NumberManager.instance.recalculate = true;
        yield return new WaitForSeconds(0.5f);


        if (specialCardType == SpecialCardType.CaughtRedHanded)
        {
            //find all yellow cards
            //determine which is best to swap out (smallest or largest number)
            //if player is over or close to value, swap out smallest
            //if player is under or far from value, swap out largest

            //thats ideal 

            //mid tier is always largest
            //always smallest

            //low tier is random

            if(difficulty == Difficulty.AlwaysLargest)
            {
                GameObject chosenCard = null;
                int val = 0;

                foreach (GameObject g in NumberManager.instance.yellows)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }

                }
                StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                GameObject chosenCard = null;
                int val = 9;

                foreach (GameObject g in NumberManager.instance.yellows)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }

                }
                StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.yellows.Count);
                GameObject chosenCard = NumberManager.instance.yellows[randomInt];
                StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));

            }
            else if (difficulty == Difficulty.Ideal)
            {
                if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 3)
                {
                    //smallest
                    GameObject chosenCard = null;
                    int val = 9;

                    foreach (GameObject g in NumberManager.instance.yellows)
                    {
                        if (g.GetComponent<NumberStats>().value < val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;
                        }

                    }
                    StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));
                }
                else if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal - 4)
                {
                    //largest
                    GameObject chosenCard = null;
                    int val = 0;

                    foreach (GameObject g in NumberManager.instance.yellows)
                    {
                        if (g.GetComponent<NumberStats>().value > val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;
                        }

                    }
                    StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));
                }
                       

            }


}
        else if (specialCardType == SpecialCardType.EmptyPockets)
        {
            GameObject randomSpecial = playerHand.GetChild(Random.Range(0, playerHand.childCount)).gameObject;
            //DiscardSpecial(randomSpecial, "opponent");
            StartCoroutine(DiscardAnimation(randomSpecial, "player"));


        }
        else if (specialCardType == SpecialCardType.Burden)
        {
            if (NumberManager.instance.reds.Count > 0)
            {
                if(difficulty == Difficulty.Ideal)
                {
                    //give player +4 if over
                    //give player -4 if under
                    if (NumberManager.instance.playerVal > NumberManager.instance.targetVal - 4)
                    {
                        SpecialCardManager.instance.Give(4, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(-4, "player");
                    }
                }
                else if(difficulty == Difficulty.AlwaysLargest)
                {
                    SpecialCardManager.instance.Give(4, "player");
                }
                else if(difficulty == Difficulty.AlwaysSmallest)
                {
                    SpecialCardManager.instance.Give(-4, "player");
                }
                else if(difficulty == Difficulty.Random)
                {
                    int randomInt = Random.Range(0, 2);
                    if(randomInt == 0)
                    {
                        SpecialCardManager.instance.Give(-4, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(4, "player");
                    }
                }
                        


            }

        NumberManager.instance.recalculate = true;

        }
        else if (specialCardType == SpecialCardType.RifleButt)
        {
            GameObject chosenCard = null;
            int val = 0;
            if (difficulty == Difficulty.Ideal)
            {
                //biggest difference, flip biggest abs value
                foreach(GameObject g in NumberManager.instance.allNumbers)
                {
                    if(Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value)) > val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value));
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
                        
            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                //flip smallest
                val = 9;
                foreach(GameObject g in NumberManager.instance.allNumbers)
                {
                    if(g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                //flip largest
                val = 0;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
    }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                StartCoroutine(CardSelectionController.instance.FlipNumber(NumberManager.instance.allNumbers[randomInt]));
            }

            PlayerStats.instance.flipped = true;
            NumberManager.instance.recalculate = true;
        }
        else if (specialCardType == SpecialCardType.SmokeBreak)
        {
            //
            //children of player discard zone
            //get 2 random cards and add back to hand

            DrawSpecialFromDiscard();
            DrawSpecialFromDiscard();

        }
        else if (specialCardType == SpecialCardType.Weakness)
        {
            DrawSpecialFromDiscard();

        }
        else if (specialCardType == SpecialCardType.ThickWoolenCoat)
        {
            if (difficulty == Difficulty.Ideal)
            {
                if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
                else if(NumberManager.instance.playerVal < NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }

            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                SpecialCardManager.instance.Give(2, "player");
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                SpecialCardManager.instance.Give(-2, "player");
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, 2);
                if(randomInt == 0)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }
                else
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
            }

            NumberManager.instance.recalculate = true;
            


        }
        else if (specialCardType == SpecialCardType.Setup)
        {
            GameObject chosenCard = null;
            int val = 0;

            if (difficulty == Difficulty.Ideal)
            {
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value)) > val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value));
                    }
                }
                StartCoroutine(CardSelectionController.instance.DiscardNumber(chosenCard));
                        
            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                //remove smallest
                val = 9;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }
                }
                StartCoroutine(CardSelectionController.instance.DiscardNumber(chosenCard));
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                //remove largest
                val = 0;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }
                }
                StartCoroutine(CardSelectionController.instance.DiscardNumber(chosenCard));
            }
            else if (difficulty == Difficulty.Random)
            {
                //remove random
                int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                StartCoroutine(CardSelectionController.instance.DiscardNumber(NumberManager.instance.allNumbers[randomInt]));
            }
            


        }
        else if (specialCardType == SpecialCardType.Bribe)
        {
            if(difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.OPPduplicates.Count);
                NumberManager.instance.OPPduplicates[randomInt].transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                NumberManager.instance.recalculate = true;
            }
            else if(difficulty == Difficulty.AlwaysLargest)
            {
                int val = 0;
                GameObject chosenCard = null;
                foreach (GameObject g in NumberManager.instance.OPPduplicates)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                        
                    }
                    chosenCard.transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                    NumberManager.instance.recalculate = true;
                }
            }
            else if(difficulty == Difficulty.AlwaysSmallest)
            {
                int val = 9;
                GameObject chosenCard = null;
                foreach (GameObject g in NumberManager.instance.OPPduplicates)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;

                    }
                    chosenCard.transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                    NumberManager.instance.recalculate = true;
                }
            }
            else if (difficulty == Difficulty.Ideal)
            {
                int val = 20;
                GameObject chosenCard = NumberManager.instance.OPPduplicates[0];
                //give away card that will put you closest to the target
                foreach (GameObject g in NumberManager.instance.OPPduplicates)
                {
                            
                    if(Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value)) < val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value));
                    }
                    chosenCard.transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                    NumberManager.instance.recalculate = true;
                }
                /*
                //if you are over, give away large card
                if(NumberManager.instance.oppVal > NumberManager.instance.targetVal)
                {
                    int val = 0;
                    GameObject chosenCard = null;
                    foreach (GameObject g in NumberManager.instance.OPPduplicates)
                    {
                        if (g.GetComponent<NumberStats>().value > val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;

                        }
                        chosenCard.transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                        NumberManager.instance.recalculate = true;
                    }
                }
                //if you are under, give away small card
                else
                {
                    int val = 9;
                    GameObject chosenCard = null;
                    foreach (GameObject g in NumberManager.instance.OPPduplicates)
                    {
                        if (g.GetComponent<NumberStats>().value < val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;

                        }
                        chosenCard.transform.SetParent(NumberManager.instance.playerPositiveArea.transform);
                        NumberManager.instance.recalculate = true;
                    }
                }
                */
            }




        }
        else if (specialCardType == SpecialCardType.Fist)
        {
            if (NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
            {
                foreach (GameObject g in NumberManager.instance.OPPblues)
                {
                    StartCoroutine(BurnShader(g));
                    yield return new WaitForSeconds(1f);

                    Destroy(g.GetComponent<CardPlace>().correspondingImage);
                    Destroy(g);
                    yield return new WaitForSeconds(0.7f);
                    CardPlacementController.instance.DealOneCard("opponent");
                    yield return new WaitForSeconds(0.7f);
                    OpponentStats.instance.swapped = true;
                }

                foreach (GameObject g in NumberManager.instance.blues)
                {
                    StartCoroutine(BurnShader(g));
                    yield return new WaitForSeconds(1f);


                    Destroy(g.GetComponent<CardPlace>().correspondingImage);
                    Destroy(g);
                    yield return new WaitForSeconds(0.7f);
                    CardPlacementController.instance.DealOneCard("player");
                    yield return new WaitForSeconds(0.7f);
                }
            }

        }
        else if (specialCardType == SpecialCardType.CondensedMilk)
        {
            //determine best card to swap out
            GameObject chosenCard = null;
            int val = 0;

            if (difficulty == Difficulty.Random)
            {
                int randomIntOne = Random.Range(0, 2);
                if(randomIntOne == 0)
                {
                    int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                    chosenCard = NumberManager.instance.allNumbers[randomInt];
                }
                else
                {
                    int randomInt = Random.Range(0, NumberManager.instance.OPPallNumbers.Count);
                    chosenCard = NumberManager.instance.OPPallNumbers[randomInt];
                }
                        
                        
            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                val = 9;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;

                    }
                }
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                val = 0;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;

                    }
                }
            }
            else if (difficulty == Difficulty.Ideal)
            {
                //if you are over, swap large card
                if(NumberManager.instance.oppVal > NumberManager.instance.targetVal)
                {
                    val = 0;
                    foreach (GameObject g in NumberManager.instance.OPPallNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value > val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;

                        }
                    }
                }
                        
                //if you are under, swap small card
                else if(NumberManager.instance.oppVal < NumberManager.instance.targetVal - 3)
                {
                    val = 9;
                    foreach (GameObject g in NumberManager.instance.OPPallNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value < val)
                        {
                            val = g.GetComponent<NumberStats>().value;
                            chosenCard = g;

                        }
                    }
                }
                else
                {
                    if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 3)
                    {
                        val = 9;
                        foreach (GameObject g in NumberManager.instance.allNumbers)
                        {
                            if (g.GetComponent<NumberStats>().value < val)
                            {
                                val = g.GetComponent<NumberStats>().value;
                                chosenCard = g;

                            }
                        }
                    }
                    else 
                    {
                        val = 0;
                        foreach (GameObject g in NumberManager.instance.allNumbers)
                        {
                            if (g.GetComponent<NumberStats>().value > val)
                            {
                                val = g.GetComponent<NumberStats>().value;
                                chosenCard = g;

                            }
                        }
                    }
                }
            }

            StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "player"));
            PlayerStats.instance.swapped = true;

}
        else if (specialCardType == SpecialCardType.InCahoots)
        {
            SpecialCardManager.instance.Give(4, "player");
            SpecialCardManager.instance.Give(4, "opponent");
        }
        else if (specialCardType == SpecialCardType.Search)
        {
            GameObject chosenCard = null;
            int val = 0;

            if (difficulty == Difficulty.AlwaysLargest)
            {
                //remove smallest
                val = 9;

                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if(g.GetComponent<NumberStats>().value < val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }

                }


            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                //remove largest
                val = 0;

                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }

                }


            }
            else if (difficulty == Difficulty.Random)
            {
                //remove random
                int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                chosenCard = NumberManager.instance.allNumbers[randomInt];

                        

            }
            else if(difficulty == Difficulty.Ideal)
            {
                /*
                //if player is under or at value, remove biggest card
                if(NumberManager.instance.playerVal <= NumberManager.instance.targetVal)
                {
                    val = 0;

                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value > val)
                        {
                            chosenCard = g;
                            val = g.GetComponent<NumberStats>().value;
                        }

                    }
                }
                //if player is over value, remove smallest card
                else if(NumberManager.instance.playerVal > NumberManager.instance.targetVal)
                {
                    val = 9;

                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value < val)
                        {
                            chosenCard = g;
                            val = g.GetComponent<NumberStats>().value;
                        }

                    }
                }
                */

                //for each number, if remove (targetval - value), see how far from target, target - this value, absolute value, and if its larger set it as the new card
                val = 0;        
                foreach(GameObject g in NumberManager.instance.allNumbers)
                {
                    if(Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value)) > val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value));
                    }
                }
            }


            StartCoroutine(CardSelectionController.instance.DiscardNumber(chosenCard));

        }
        else if (specialCardType == SpecialCardType.Poison)
        {
            //get random special card
            GameObject randomSpecial = playerHand.GetChild(Random.Range(0, playerHand.childCount)).gameObject;
            //DiscardSpecial(randomSpecial, "opponent");
            StartCoroutine(DiscardAnimation(randomSpecial, "player"));

        }
        else if (specialCardType == SpecialCardType.BackstabDiscard)
        {
            if (difficulty == Difficulty.Ideal)
            {
                if (NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
                else if (NumberManager.instance.playerVal < NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }

            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                SpecialCardManager.instance.Give(2, "player");
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                SpecialCardManager.instance.Give(-2, "player");
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, 2);
                if (randomInt == 0)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }
                else
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
            }

            NumberManager.instance.recalculate = true;




        }
        else if (specialCardType == SpecialCardType.Scam)
        {
            int val = 0;
            GameObject chosenCard = null;
            if(difficulty == Difficulty.AlwaysLargest)
            {
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if(g.GetComponent<NumberStats>().value > val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }
                }
            }
            else if(difficulty == Difficulty.AlwaysSmallest)
            {
                val = 9;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        chosenCard = g;
                        val = g.GetComponent<NumberStats>().value;
                    }
                }
            }
            else if(difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                chosenCard = NumberManager.instance.allNumbers[randomInt];
            }
            else if(difficulty == Difficulty.Ideal)
            {
                val = 0;
                foreach(GameObject g in NumberManager.instance.allNumbers)
                {
                    if(Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal + g.GetComponent<NumberStats>().value)) > val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal + g.GetComponent<NumberStats>().value));
                    }
                }
            }
                    
            if(chosenCard.GetComponent<NumberStats>().value > 0)
            {
                Instantiate(chosenCard, CardPlacementController.instance.playerPositiveArea);
            }
            else if(chosenCard.GetComponent<NumberStats>().value < 0)
            {
                Instantiate(chosenCard, CardPlacementController.instance.playerNegativeArea);
            }
                    

        }
        else if (specialCardType == SpecialCardType.GiveItUp)
        {

            GameObject randomSpecial = playerHand.GetChild(Random.Range(0, playerHand.childCount)).gameObject;
            StartCoroutine(DiscardAnimation(randomSpecial, "player"));

        }
        else if (specialCardType == SpecialCardType.Rotation)
        {
            int val = 0;
            GameObject chosenCard = null;
            if(difficulty == Difficulty.AlwaysLargest || difficulty == Difficulty.AlwaysSmallest || difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.OPPnegatives.Count);
                chosenCard = NumberManager.instance.OPPnegatives[randomInt];
            }
            else if(difficulty == Difficulty.Ideal)
            {
                val = 9;
                chosenCard = NumberManager.instance.OPPnegatives[0];
                foreach (GameObject g in NumberManager.instance.OPPnegatives)
                {
                    if (Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value + CardPlacementController.instance.numberDeck[0].GetComponent<NumberStats>().value)) < val)
                    {
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value + CardPlacementController.instance.numberDeck[0].GetComponent<NumberStats>().value));
                        chosenCard = g;
                    }
                }
            }

            StartCoroutine(CardSelectionController.instance.SwapOut(chosenCard, "opponent"));
            
        }
        else if (specialCardType == SpecialCardType.DirtyTrickIV)
        {


        }
        else if (specialCardType == SpecialCardType.BaitAndSwitch)
        {
            //if under, change 2 to -2
            //if over, change -2 to 2

            //if close to busting change -2 to 2

            bool notFound = true;
            if(difficulty == Difficulty.AlwaysLargest)
            {
                //change -2 to 2
                foreach(GameObject g in NumberManager.instance.allNumbers)
                {
                    if(g.GetComponent<NumberStats>().value == -2)
                    {
                        StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                        notFound = false;
                        break;
                    }
                }
                if(notFound)
                {
                    foreach(GameObject g in NumberManager.instance.allNumbers)
                    {
                        if(g.GetComponent<NumberStats>().value == 2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                }
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                //change -2 to 2
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value == 2)
                    {
                        StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                        notFound = false;
                        break;
                    }
                }
                if (notFound)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == -2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                }
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, 2);
                if(randomInt == 0)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == -2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                    if(notFound)
                    {
                        foreach(GameObject g in NumberManager.instance.allNumbers)
                        {
                            if(g.GetComponent<NumberStats>().value == 2)
                            {
                                StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                                notFound = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == 2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                    if(notFound)
                    {
                        foreach(GameObject g in NumberManager.instance.allNumbers)
                        {
                            if(g.GetComponent<NumberStats>().value == -2)
                            {
                                StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                                notFound = false;
                                break;
                            }
                        }
                    }
                }
                
            }
            else if(difficulty == Difficulty.Ideal) //change to magnitude (abs val)
            {
                if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 3)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == -2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                    if(notFound)
                    {
                        foreach(GameObject g in NumberManager.instance.allNumbers)
                        {
                            if(g.GetComponent<NumberStats>().value == 2)
                            {
                                StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                                notFound = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == 2)
                        {
                            StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -2, "player"));
                            notFound = false;
                            break;
                        }
                    }
                    if(notFound)
                    {
                        foreach(GameObject g in NumberManager.instance.allNumbers)
                        {
                            if(g.GetComponent<NumberStats>().value == -2)
                            {
                                StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 2, "player"));
                                notFound = false;
                                break;
                            }
                        }
                    }
                }
            }



        }
        else if (specialCardType == SpecialCardType.SelfHarm)
        {
            GameObject chosenCard = null;
            int val = 0;
            if(difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.OPPallNumbers.Count);
                chosenCard = NumberManager.instance.OPPallNumbers[randomInt];
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
                
            }
            else
            {
                val = 9;
                chosenCard = NumberManager.instance.OPPallNumbers[0];
                foreach (GameObject g in NumberManager.instance.OPPallNumbers)
                {
                    if (Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value)) < val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value));
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
            }
        }
        else if (specialCardType == SpecialCardType.BackstabSwap)
        {
            if (difficulty == Difficulty.Ideal)
            {
                if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
                else if(NumberManager.instance.playerVal < NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }

            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                SpecialCardManager.instance.Give(2, "player");
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                SpecialCardManager.instance.Give(-2, "player");
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, 2);
                if(randomInt == 0)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }
                else
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
            }

            NumberManager.instance.recalculate = true;

        }
        else if (specialCardType == SpecialCardType.ThereThere)
        {
                    
            if (difficulty == Difficulty.Ideal)
            {
                if(NumberManager.instance.playerVal >= NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
                else if(NumberManager.instance.playerVal < NumberManager.instance.targetVal - 1)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }

            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                SpecialCardManager.instance.Give(2, "player");
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                SpecialCardManager.instance.Give(-2, "player");
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, 2);
                if(randomInt == 0)
                {
                    SpecialCardManager.instance.Give(-2, "player");
                }
                else
                {
                    SpecialCardManager.instance.Give(2, "player");
                }
            }

            NumberManager.instance.recalculate = true;

        }
        else if (specialCardType == SpecialCardType.NotMyProblem)
        {
            GameObject chosenCard = null;
            int val = 0;
            if(difficulty == Difficulty.AlwaysLargest)
            {
                foreach(GameObject g in NumberManager.instance.OPPallNumbers)
                {
                    if(g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
            }
            else if(difficulty == Difficulty.AlwaysSmallest)
            {
                val = 9;
                foreach (GameObject g in NumberManager.instance.OPPallNumbers)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
            }
            else if(difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.OPPallNumbers.Count);
                chosenCard = NumberManager.instance.OPPallNumbers[randomInt];
            }
            else if(difficulty == Difficulty.Ideal)
            {
                //find number to get rid of that will put opp closest to value
                val = 10;
                chosenCard = NumberManager.instance.OPPallNumbers[0];
                foreach(GameObject g in NumberManager.instance.OPPallNumbers)
                {
                    if (Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value)) < val)
                    {
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.oppVal - g.GetComponent<NumberStats>().value));
                        chosenCard = g;
                    }
                }
            }

            CardSelectionController.instance.GiftNumber(chosenCard);
        }
        else if (specialCardType == SpecialCardType.Frostbite)
        {
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                if (g.GetComponent<NumberStats>().value == 2)
                {
                    //remove 2 and add 4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 4, "player"));
                }
                if (g.GetComponent<NumberStats>().value == -2)
                {
                    //remove -2 and add -4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -4, "player"));
                }
            }
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                if (g.GetComponent<NumberStats>().value == 2)
                {
                    //remove 2 and add 4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 4, "opponent"));
                }
                if (g.GetComponent<NumberStats>().value == -2)
                {
                    //remove -2 and add -4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -4, "opponent"));
                }
            }

        }
        else if (specialCardType == SpecialCardType.DirtyTrickI)
        {


        }
        else if (specialCardType == SpecialCardType.DirtyTrickII)
        {

        }
        else if (specialCardType == SpecialCardType.DirtyTrickIII)
        {

        }
        else if (specialCardType == SpecialCardType.LousyDeal)
        {


        }
        else if (specialCardType == SpecialCardType.FindersKeepers)
        {


        }
        else if (specialCardType == SpecialCardType.Gossip)
        {


        }
        else if (specialCardType == SpecialCardType.FairShare)
        {


        }
        else if (specialCardType == SpecialCardType.SleeplessNight)
        {


        }
        else if (specialCardType == SpecialCardType.Payback)
        {


        }
        else if (specialCardType == SpecialCardType.Knife)
        {
            GameObject chosenCard = null;
            int val = 0;
            if (difficulty == Difficulty.Ideal)
            {
                //biggest difference, flip biggest abs value
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value)) > val)
                    {
                        chosenCard = g;
                        val = Mathf.Abs(NumberManager.instance.targetVal - (NumberManager.instance.playerVal - g.GetComponent<NumberStats>().value - g.GetComponent<NumberStats>().value));
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));

            }
            else if (difficulty == Difficulty.AlwaysLargest)
            {
                //flip smallest
                val = 9;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value < val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
            }
            else if (difficulty == Difficulty.AlwaysSmallest)
            {
                //flip largest
                val = 0;
                foreach (GameObject g in NumberManager.instance.allNumbers)
                {
                    if (g.GetComponent<NumberStats>().value > val)
                    {
                        val = g.GetComponent<NumberStats>().value;
                        chosenCard = g;
                    }
                }
                StartCoroutine(CardSelectionController.instance.FlipNumber(chosenCard));
            }
            else if (difficulty == Difficulty.Random)
            {
                int randomInt = Random.Range(0, NumberManager.instance.allNumbers.Count);
                StartCoroutine(CardSelectionController.instance.FlipNumber(NumberManager.instance.allNumbers[randomInt]));
            }

            PlayerStats.instance.flipped = true;
            NumberManager.instance.recalculate = true;

}
        else if (specialCardType == SpecialCardType.ExtraWork)
        {
            CardPlacementController.instance.DealOneCard("player");

        }
        else if (specialCardType == SpecialCardType.Scratch)
        {


        }
        else if (specialCardType == SpecialCardType.Leftovers)
        {
            DrawSpecialFromDiscard();

        }
        else if (specialCardType == SpecialCardType.Glare)
        {


        }
        else if (specialCardType == SpecialCardType.Snitch)
        {
            /*
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("change", "opponent", 5);
            */
        }
        else if (specialCardType == SpecialCardType.GoodDeal)
        {


        }
        else if (specialCardType == SpecialCardType.EasyDay)
        {


        }
        else if (specialCardType == SpecialCardType.GoodFeeling)
        {
            if (CardPlacementController.instance.numberDeck[0].GetComponent<NumberStats>().yellow)
            {
                CardPlacementController.instance.DealOneCard("opponent");
                yield return new WaitForSeconds(0.7f);
                CardPlacementController.instance.DealOneCard("player");
            }
            else
            {
                CardPlacementController.instance.DealOneCard("opponent");
            }


        }
        else if (specialCardType == SpecialCardType.Forgery)
        {
            /*
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("duplicate", "player");
            */
        }
        else if (specialCardType == SpecialCardType.Pushover)
        {
            /*
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("discard", "player");
            */

            //calculate which number to remove to be closest to target
        }
        else if (specialCardType == SpecialCardType.Scavenge)
        {
            if (discardedCards.Count > 0)
            {
                //add best special card to hand
            }

        }
        else if (specialCardType == SpecialCardType.Overwhelmed)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                //pick best number to remove to be closest to target
            }



        }
        else if (specialCardType == SpecialCardType.VitaminShotII)
        {
            SpecialCardManager.instance.Give(2, "opponent");

        }
        else if (specialCardType == SpecialCardType.VitaminShotV)
        {
            SpecialCardManager.instance.Give(5, "opponent");
        }

    }

    public void DiscardSpecial(GameObject g, string target)
    {
        if (target == "player")
        {
            PlayerStats.instance.discarded = true;
            g.transform.SetParent(playerDiscardZone.transform);
            g.transform.position = playerDiscardZone.transform.position;
        }
        else
        {
            OpponentStats.instance.discarded = true;
            g.transform.SetParent(opponentDiscardZone.transform);
            g.transform.position = opponentDiscardZone.transform.position;
        }

    }

    IEnumerator DiscardAnimation(GameObject g, string target)
    {

        //opponent special cards upside down with card back
        //discard animation -goes down, upside down, and then turns around and goes away

                
        //show description


        if (target == "player")
        {
            g.GetComponent<CardPlace>().isPlayable = false;
            g.GetComponent<CardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector3(400f, 0f, 0);
            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
            yield return new WaitForSeconds(1f);

            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            g.transform.SetParent(playerDiscardZone.transform);
            g.transform.position = playerDiscardZone.transform.position;

            PlayerStats.instance.discarded = true;
        }
        else
        {
            //is card back
            //squeeze to 0 then stretch back
            //when 0, hide cardback and show card image
            g.GetComponent<AICardPlace>().isPlayable = false;
            g.GetComponent<RectTransform>().anchoredPosition = new Vector3(200f, -320f, 0);
            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FlipOverCard(g));
            yield return new WaitForSeconds(1f);

            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            g.transform.SetParent(opponentDiscardZone.transform);
            g.transform.position = opponentDiscardZone.transform.position;

            OpponentStats.instance.discarded = true;
        }

    }

    IEnumerator FlipOverCard(GameObject g)
    {
        float timer = 0f;
        float halfTime = 0.1f;

        float originalScaleX = g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.x;

        for (timer = 0; timer < halfTime; timer += Time.deltaTime)
        {
            float scaleX = Mathf.Lerp(originalScaleX, 0f, timer / halfTime);
            g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(scaleX, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.z);
            yield return null;
        }

        g.GetComponent<AICardPlace>().correspondingImage.GetComponent<Image>().sprite = grey;
        g.GetComponent<AICardPlace>().correspondingImage.transform.Find("Image").GetComponent<Image>().enabled = true;
        g.GetComponent<AICardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);

        for (timer = 0; timer < halfTime; timer += Time.deltaTime)
        {
            float scaleX = Mathf.Lerp(0f, originalScaleX, timer / halfTime);
            g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(scaleX, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.z);
            yield return null;
        }

        g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(originalScaleX, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<AICardPlace>().correspondingImage.transform.localScale.z);
    }

    public void DrawSpecialFromDiscard()
    {
        if (discardedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, discardedCards.Count);
            GameObject chosenCard = discardedCards[randomIndex];
            discardedCards.RemoveAt(randomIndex);
            chosenCard.GetComponent<AICardPlace>().beingPlayed = false;
            //chosenCard.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            chosenCard.transform.SetParent(opponentHand);
        }
        else
        {
            Debug.Log("not enough cards!");
        }
    }

    public void ActivateChoice(int x)
    {
        //show two buttons, choice one and choice two
        //on click instantiate right card for right target depending on pos or neg
        ChoiceController.instance.ShowChoice(x);

    }



    public enum SpecialCardType
    {
        CaughtRedHanded,
        EmptyPockets,
        Burden,
        RifleButt,
        SmokeBreak,
        Weakness,
        ThickWoolenCoat,
        Setup,
        Bribe,
        Fist,
        CondensedMilk,
        InCahoots,
        Search,
        Poison,
        BackstabDiscard,
        Scam,
        GiveItUp,
        Rotation,
        DirtyTrickIV,
        BaitAndSwitch,
        SelfHarm,
        BackstabSwap,
        ThereThere,
        NotMyProblem,
        Frostbite,
        DirtyTrickI,
        DirtyTrickII,
        DirtyTrickIII,
        LousyDeal,
        FindersKeepers,
        Gossip,
        FairShare,
        SleeplessNight,
        Payback,
        Knife,
        ExtraWork,
        Scratch,
        Leftovers,
        Glare,
        Snitch,
        GoodDeal,
        EasyDay,
        GoodFeeling,
        Forgery,
        Pushover,
        Scavenge,
        Overwhelmed,
        VitaminShotII,
        VitaminShotV
    }
}

public enum Difficulty
{
    Random,
    AlwaysSmallest,
    AlwaysLargest,
    Ideal
}



    

