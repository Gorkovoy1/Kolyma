using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using AILogic;
 
    namespace TutorialScripts
    {

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

            // Start is called before the first frame update
            void Start()
            {
                discardUpdated = false;
                isFlipped = false;

                playerHand = this.transform.parent;
                opponentHand = playerHand.parent.Find("OpponentHand");
                parentReturnTo = playerHand;
                //set images parent
                //set discard zone obj
                playerDiscardZone = GameObject.FindWithTag("PlayerDiscard");
                opponentDiscardZone = GameObject.FindWithTag("OpponentDiscard");

                if (imagePrefab != null)
                {
                    //this means its a special card
                    imagesParent = this.GetComponentInParent<TutorialScripts.HandController>().imagesParent;
                    correspondingImage = Instantiate(imagePrefab, imagesParent);
                    correspondingImage.GetComponent<TutorialScripts.AISpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();
                    defaultMat = correspondingImage.GetComponent<Image>().material;

                    //start flipped
                    if (this.transform.parent == opponentHand)
                    {
                        grey = correspondingImage.GetComponent<Image>().sprite;
                        correspondingImage.GetComponent<Image>().sprite = cardBack;
                        correspondingImage.transform.Find("Image").GetComponent<Image>().enabled = false;
                    }
                }

            }

            // Update is called once per frame
            void Update()
            {

                if (imagePrefab != null)
                {
                    //this means its a special card
                    if (TurnManager.instance.isPlayerTurn && this.transform.parent == playerHand) //check parent is hand
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
                this.GetComponent<RectTransform>().anchoredPosition = new Vector3(-80f, -350f, 0);
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
                //Debug.Log("checkingplayable");
                if (specialCardType == SpecialCardType.CaughtRedHanded)
                {
                    if (NumberManager.instance.yellows.Count > 0)
                    {
                        isPlayable = true;
                    }
                }
                else if (specialCardType == SpecialCardType.EmptyPockets)
                {
                    if (NumberManager.instance.blues.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Burden)
                {
                    if (NumberManager.instance.reds.Count > 0)
                    {
                        isPlayable = true;
                    }
                }
                else if (specialCardType == SpecialCardType.RifleButt)
                {
                    if (NumberManager.instance.reds.Count > 0)
                    {
                        isPlayable = true;
                    }
                }
                else if (specialCardType == SpecialCardType.SmokeBreak)
                {
                    //draw 2 random specials from discard
                    isPlayable = true;


                }
                else if (specialCardType == SpecialCardType.Weakness)
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
                else if (specialCardType == SpecialCardType.ThickWoolenCoat)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Setup)
                {
                    if (NumberManager.instance.negatives.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Bribe)
                {

                    if (NumberManager.instance.OPPduplicates.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Fist)
                {
                    if (NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.CondensedMilk)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.InCahoots)
                {
                    isPlayable = true;
                }
                else if (specialCardType == SpecialCardType.Search)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Poison)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.BackstabDiscard)
                {
                    //if opponent discarded then true
                    if (PlayerStats.instance.discarded)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Scam)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.GiveItUp)
                {
                    if (NumberManager.instance.OPPnegatives.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Rotation)
                {
                    if (NumberManager.instance.OPPnegatives.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.DirtyTrickIV)
                {
                    if (NumberManager.instance.yellows.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.BaitAndSwitch)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        if (g.GetComponent<NumberStats>().value == 2 || g.GetComponent<NumberStats>().value == -2)
                        {
                            isPlayable = true;
                            break;
                        }
                    }

                }
                else if (specialCardType == SpecialCardType.SelfHarm)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.BackstabSwap)
                {
                    //if opp swapped then playable
                    if (PlayerStats.instance.swapped)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.ThereThere)
                {
                    if (NumberManager.instance.OPPduplicates.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.NotMyProblem)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Frostbite)
                {
                    //if ahve 2 or -2
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

                }
                else if (specialCardType == SpecialCardType.DirtyTrickI)
                {
                    if (NumberManager.instance.OPPreds.Count > 0 || NumberManager.instance.reds.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.DirtyTrickII)
                {
                    if (NumberManager.instance.OPPyellows.Count > 0 || NumberManager.instance.yellows.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.DirtyTrickIII)
                {
                    if (NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.LousyDeal)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.FindersKeepers)
                {
                    if (NumberManager.instance.OPPreds.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Gossip)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.FairShare)
                {
                    if (NumberManager.instance.negatives.Count > 0)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.SleeplessNight)
                {
                    if (PlayerStats.instance.swapped)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Payback)
                {
                    if (PlayerStats.instance.gave)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Knife)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.ExtraWork)
                {
                    //if opp flipped
                    if (PlayerStats.instance.flipped)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Scratch)
                {
                    if (PlayerStats.instance.gave)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Leftovers)
                {
                    //if opp discarded
                    if (PlayerStats.instance.discarded)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.Glare)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Snitch)
                {
                    //if opp discarded
                    if (PlayerStats.instance.discarded)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.GoodDeal)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.EasyDay)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.GoodFeeling)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Forgery)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Pushover)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Scavenge)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.Overwhelmed)
                {
                    if (NumberManager.instance.OPPallNumbers.Count > 5)
                    {
                        isPlayable = true;
                    }

                }
                else if (specialCardType == SpecialCardType.VitaminShotII)
                {
                    isPlayable = true;

                }
                else if (specialCardType == SpecialCardType.VitaminShotV)
                {
                    isPlayable = true;
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


                    if (SceneManager.GetActiveScene().name == "TutorialScene")
                    {
                        foreach (GameObject g in NumberManager.instance.allNumbers)
                        {
                            if (g.GetComponent<NumberStats>().value == 2)
                            {
                                Debug.Log("TutorialScripts.CardSelectionController.instance = " + TutorialScripts.CardSelectionController.instance);
                                Debug.Log("g = " + g);
                                Debug.Log("g.GetComponent<NumberStats>() = " + g.GetComponent<NumberStats>());
                                
                                StartCoroutine(TutorialScripts.CardSelectionController.instance.SwapOut(g, "player"));
                                break;
                            }
                        }
                    }

                }
                else if (specialCardType == SpecialCardType.EmptyPockets)
                {
                    //get random special card
                    GameObject randomSpecial = playerHand.GetChild(Random.Range(0, playerHand.childCount)).gameObject;
                    //DiscardSpecial(randomSpecial, "opponent");
                    StartCoroutine(DiscardAnimation(randomSpecial, "player"));

                }
                else if (specialCardType == SpecialCardType.Burden)
                {
                    if (NumberManager.instance.reds.Count > 0)
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

                }
                else if (specialCardType == SpecialCardType.RifleButt)
                {
                    //determine best card to flip from player, flip it (any number) (largest impact is large number)

                    //flip the largest number

                    PlayerStats.instance.flipped = true;
                }
                else if (specialCardType == SpecialCardType.SmokeBreak)
                {
                    //find children of player discard zone
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

                    if (NumberManager.instance.playerVal > NumberManager.instance.targetVal - 2)
                    {
                        SpecialCardManager.instance.Give(2, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(-2, "player");
                    }


                }
                else if (specialCardType == SpecialCardType.Setup)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        //if under,
                        //find the largest number and discard it 

                        //if bust, discard the negative number
                        //or dont discard
                    }


                }
                else if (specialCardType == SpecialCardType.Bribe)
                {
                    //give player largest or negative number depending on under or over

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

                }
                else if (specialCardType == SpecialCardType.InCahoots)
                {
                    SpecialCardManager.instance.Give(4, "player");
                    SpecialCardManager.instance.Give(4, "opponent");
                }
                else if (specialCardType == SpecialCardType.Search)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        //find best number to remove (largest, or negative)
                    }

                    //CardSelectionController.instance.CallButtons("discard", "opponent");

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
                    if (NumberManager.instance.playerVal > NumberManager.instance.targetVal - 2)
                    {
                        SpecialCardManager.instance.Give(2, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(-2, "player");
                    }

                }
                else if (specialCardType == SpecialCardType.Scam)
                {
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        //find best number to duplicate (largest or negative)
                    }

                    //CardSelectionController.instance.CallButtons("duplicate", "opponent");

                }
                else if (specialCardType == SpecialCardType.GiveItUp)
                {

                    GameObject randomSpecial = playerHand.GetChild(Random.Range(0, playerHand.childCount)).gameObject;
                    StartCoroutine(DiscardAnimation(randomSpecial, "player"));

                }
                else if (specialCardType == SpecialCardType.Rotation)
                {
                    foreach (GameObject g in NumberManager.instance.OPPnegatives)
                    {
                        //find which number best to swap - know the next card in deck?
                    }

                    //CardSelectionController.instance.CallButtons("swap", "player");

                }
                else if (specialCardType == SpecialCardType.DirtyTrickIV)
                {


                }
                else if (specialCardType == SpecialCardType.BaitAndSwitch)
                {
                    //if under, change 2 to -2
                    //if over, change -2 to 2

                    //if close to busting change -2 to 2
                }
                else if (specialCardType == SpecialCardType.SelfHarm)
                {
                    /*
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        g.GetComponent<NumberStats>().selectable = true;
                    }

                    CardSelectionController.instance.CallButtons("flip", "player");

                    PlayerStats.instance.flipped = true;
                    */
                }
                else if (specialCardType == SpecialCardType.BackstabSwap)
                {
                    if (NumberManager.instance.playerVal > NumberManager.instance.targetVal - 2)
                    {
                        SpecialCardManager.instance.Give(2, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(-2, "player");
                    }

                }
                else if (specialCardType == SpecialCardType.ThereThere)
                {
                    if (NumberManager.instance.playerVal > NumberManager.instance.targetVal - 2)
                    {
                        SpecialCardManager.instance.Give(2, "player");
                    }
                    else
                    {
                        SpecialCardManager.instance.Give(-2, "player");
                    }

                }
                else if (specialCardType == SpecialCardType.NotMyProblem)
                {
                    /*
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        g.GetComponent<NumberStats>().selectable = true;
                    }

                    CardSelectionController.instance.CallButtons("give", "opponent");
                    */
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
                    foreach (GameObject g in NumberManager.instance.allNumbers)
                    {
                        //find best number to flip
                    }

                    //CardSelectionController.instance.CallButtons("flip", "player");

                    //PlayerStats.instance.flipped = true;

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

                g.GetComponent<CardPlace>().isPlayable = false;
                //show description


                if (target == "player")
                {
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
                    chosenCard.GetComponent<CardPlace>().beingPlayed = false;
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

    }

