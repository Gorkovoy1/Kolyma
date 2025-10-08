using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using AK.Wwise;

public class CardPlace : MonoBehaviour,
    IDragHandler, IBeginDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler

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

    public AK.Wwise.Event trickSound;
    //public GameObject sfxObj;

    

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
            imagesParent = this.GetComponentInParent<HandController>().imagesParent;
            correspondingImage = Instantiate(imagePrefab, imagesParent);
            correspondingImage.GetComponent<SpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();
            defaultMat = correspondingImage.GetComponent<Image>().material;

            //start flipped
            if(this.transform.parent == opponentHand)
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
                if(!TurnManager.instance.checkedPlayable)
                {
                    
                    CheckPlayable();
                }
                
                if(isPlayable)
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
        if (TurnManager.instance.isPlayerTurn /*&& !discardUpdated*/)          //uncomment when have turns
        {
            discardedCards = new List<GameObject>();

            foreach (Transform child in playerDiscardZone.transform)
            {
                discardedCards.Add(child.gameObject);
            }

            discardUpdated = true;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            hovering = false;
            Debug.Log("OnBeginDrag");
            dragging = true;


            //if(conditionMet == true)
            //{
            parentReturnTo = this.transform.parent;

            //
            this.correspondingImage.transform.SetAsLastSibling();

            this.transform.SetParent(parentReturnTo.transform.parent);


            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //}
        }


    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            dragging = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            playerHand.GetComponent<HandFanController>().dragging = false;

            if (!beingPlayed)
            {
                this.transform.SetParent(parentReturnTo);
                //this.transform.position = new Vector3(0,0,0);

            }
            else
            {
                CheckPlayable();
                if(isPlayable)
                {
                    AnimateBeingPlayed();
                }
                else
                {
                    this.transform.SetParent(parentReturnTo);
                }
                
            }
        }
        

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            //Debug.Log("OnDrag");

            //if(conditionMet==true)
            this.transform.position = eventData.position;
            this.correspondingImage.transform.SetAsLastSibling();
            //set as last index in array of specila cards (special number of cards)
            playerHand.GetComponent<HandFanController>().dragging = true;



            correspondingImage.transform.SetAsLastSibling();
            correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //correspondingImage.transform.position += new Vector3(0f, hoverOffset, 0f);
            
        }
            

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            if (!beingPlayed)
            {
                if (!dragging)
                {
                    hovering = true;

                }
                if (correspondingImage != null)
                {
                    if (!playerHand.GetComponent<HandFanController>().dragging)
                    {
                        correspondingImage.transform.SetAsLastSibling();
                        correspondingImage.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        correspondingImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(correspondingImage.GetComponent<RectTransform>().anchoredPosition.x, correspondingImage.GetComponent<RectTransform>().anchoredPosition.y + hoverOffset, 0f);
                        correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);
                    }

                }

            }
        }
        
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name == "PlayerHand")
        {
            if (!beingPlayed)
            {
                hovering = false;

                if (correspondingImage != null)
                {
                    if (!playerHand.GetComponent<HandFanController>().dragging)
                    {
                        correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        correspondingImage.transform.position -= new Vector3(0f, hoverOffset, 0f);
                        correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
                    }

                }



            }
        }
        
    }

    public void AnimateBeingPlayed()
    {
        isPlayable = false;
        TurnManager.instance.playerPlayedCard = true;
        //also set every other card to not playable
        TurnManager.instance.checkedPlayable = true;
        foreach (Transform child in playerHand)
        {
            child.GetComponent<CardPlace>().isPlayable = false;
        }


        if(trickSound != null)
        {
            trickSound.Post(this.gameObject);
        }
        
        correspondingImage.GetComponent<Image>().material = defaultMat;
        StartCoroutine(BeingPlayed());
        
    }

    IEnumerator BurnShader(GameObject g)
    {
        AkSoundEngine.PostEvent("Play_Card_Burn", playerHand.GetComponentInParent<HandController>().sfxObj);

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
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(400f, 0f, 0);
        correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
        yield return new WaitForSeconds(1f);

        correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        this.transform.SetParent(playerDiscardZone.transform);
        this.transform.position = playerDiscardZone.transform.position;

        playerHand.GetComponentInParent<HandController>().playerDiscardButton.GetComponent<PlayerDiscardButton>().lastPlayed = correspondingImage.GetComponent<Image>();
        foreach (var tmp in correspondingImage.GetComponentsInChildren<TextMeshProUGUI>(true)) // true = include inactive
        {
            if (tmp.name == "Text (TMP) (1)") // Replace with your actual object name
            {
                playerHand.GetComponentInParent<HandController>().playerDiscardButton.GetComponent<PlayerDiscardButton>().cardDesc = tmp.text;
                break;
            }
        }
        playerHand.GetComponentInParent<HandController>().playerDiscardButton.GetComponent<PlayerDiscardButton>().AddCardToList();

        StartCoroutine(PlayCorrespondingAction());
    }

    public void CheckPlayable()
    {
        //Debug.Log("checkingplayable");
        if (specialCardType == SpecialCardType.CaughtRedHanded)
        {
            if (NumberManager.instance.OPPyellows.Count > 0)
            {
                isPlayable = true;
            }
        }
        else if (specialCardType == SpecialCardType.EmptyPockets)
        {
            if (NumberManager.instance.OPPblues.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.Burden)
        {
            if (NumberManager.instance.OPPreds.Count > 0)
            {
                isPlayable = true;
            }
        }
        else if (specialCardType == SpecialCardType.RifleButt)
        {
            if (NumberManager.instance.OPPreds.Count > 0)
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
            foreach(GameObject g in NumberManager.instance.OPPpositives)
            {
                if(g.GetComponent<NumberStats>().value == 2)
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
            if (NumberManager.instance.OPPnegatives.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.Bribe) 
        {
            
            if(NumberManager.instance.duplicates.Count > 0)
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
            if(OpponentStats.instance.discarded)
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
            if(NumberManager.instance.negatives.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.Rotation)
        {
            if (NumberManager.instance.negatives.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.DirtyTrickIV)
        {
            if (NumberManager.instance.OPPyellows.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.BaitAndSwitch)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
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
            if(OpponentStats.instance.swapped)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.ThereThere)
        {
            if (NumberManager.instance.duplicates.Count > 0)
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
            foreach(GameObject g in NumberManager.instance.allNumbers)
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
            if(NumberManager.instance.OPPreds.Count > 0 || NumberManager.instance.reds.Count > 0)
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
            if (NumberManager.instance.reds.Count > 0)
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
            if(NumberManager.instance.OPPnegatives.Count > 0)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.SleeplessNight)
        {
            //if opp swapped

        }
        else if (specialCardType == SpecialCardType.Payback)
        {
            //if opp gave u something

        }
        else if (specialCardType == SpecialCardType.Knife)
        {
            isPlayable = true;

        }
        else if (specialCardType == SpecialCardType.ExtraWork)
        {
            //if opp flipped
            if(OpponentStats.instance.flipped)
            {
                isPlayable = true;
            }

        }
        else if (specialCardType == SpecialCardType.Scratch)
        {
            //if opp gave u something

        }
        else if (specialCardType == SpecialCardType.Leftovers)
        {
            //if opp discarded
            if(OpponentStats.instance.discarded)
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
            if(OpponentStats.instance.discarded)
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
            if(NumberManager.instance.allNumbers.Count > 5)
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


        if(specialCardType == SpecialCardType.CaughtRedHanded)
        {
            //set all yellow opponent selectable
            //on click swap out click

            foreach (GameObject g in NumberManager.instance.OPPyellows)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("swap", "opponent");

        }
        else if (specialCardType == SpecialCardType.EmptyPockets)
        {
            //get random special card
            GameObject randomSpecial = opponentHand.GetChild(Random.Range(0, opponentHand.childCount)).gameObject;
            //DiscardSpecial(randomSpecial, "opponent");
            StartCoroutine(DiscardAnimation(randomSpecial, "opponent"));

        }
        else if (specialCardType == SpecialCardType.Burden)
        {
            if (TurnManager.instance.isPlayerTurn)
            {
                if (NumberManager.instance.OPPreds.Count > 0)
                {
                    ActivateChoice(4);
                }
            }

            //else ai logic
            //if under target then negative
            //if over target then positive
        }
        else if (specialCardType == SpecialCardType.RifleButt)
        {
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("flip", "opponent");

            OpponentStats.instance.flipped = true;
        }
        else if (specialCardType == SpecialCardType.SmokeBreak)
        {
            //find children of player discard zone
            //get 2 random cards and add back to hand

            //exclude smoke break 

            DrawSpecialFromDiscard();
            DrawSpecialFromDiscard();

        }
        else if (specialCardType == SpecialCardType.Weakness)
        {
            DrawSpecialFromDiscard();

        }
        else if (specialCardType == SpecialCardType.ThickWoolenCoat)
        {
            if (TurnManager.instance.isPlayerTurn)
            {
                ActivateChoice(2);
            }
            
            //else AI logic
            //if under target then negative
            //if over target then positive

        }
        else if (specialCardType == SpecialCardType.Setup)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("discard", "opponent");

        }
        else if (specialCardType == SpecialCardType.Bribe)
        {
            foreach(GameObject g in NumberManager.instance.duplicates)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("gift", "opponent");

        }
        else if (specialCardType == SpecialCardType.Fist)
        {
            if(NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0)
            {
                foreach(GameObject g in NumberManager.instance.OPPblues)
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

                foreach(GameObject g in NumberManager.instance.blues)
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
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("swap", "opponent");
            

        }
        else if (specialCardType == SpecialCardType.InCahoots)
        {
            SpecialCardManager.instance.Give(4, "player");
            SpecialCardManager.instance.Give(4, "opponent");
        }
        else if (specialCardType == SpecialCardType.Search)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("discard", "opponent");

        }
        else if (specialCardType == SpecialCardType.Poison)
        {
            //get random special card
            GameObject randomSpecial = opponentHand.GetChild(Random.Range(0, opponentHand.childCount)).gameObject;
            //DiscardSpecial(randomSpecial, "opponent");
            StartCoroutine(DiscardAnimation(randomSpecial, "opponent"));

        }
        else if (specialCardType == SpecialCardType.BackstabDiscard)
        {
            ActivateChoice(2);

        }
        else if (specialCardType == SpecialCardType.Scam)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("duplicate", "opponent");

        }
        else if (specialCardType == SpecialCardType.GiveItUp)
        {
            
            GameObject randomSpecial = opponentHand.GetChild(Random.Range(0, opponentHand.childCount)).gameObject;
            StartCoroutine(DiscardAnimation(randomSpecial, "opponent"));

        }
        else if (specialCardType == SpecialCardType.Rotation)
        {
            foreach (GameObject g in NumberManager.instance.negatives)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("swap", "player");

        }
        else if (specialCardType == SpecialCardType.DirtyTrickIV)
        {


        }
        else if (specialCardType == SpecialCardType.BaitAndSwitch)
        {
            foreach (GameObject g in NumberManager.instance.OPPallNumbers)
            {
                if (g.GetComponent<NumberStats>().value == 2 || g.GetComponent<NumberStats>().value == -2)
                {
                    g.GetComponent<NumberStats>().selectable = true;
                }
            }
            CardSelectionController.instance.CallButtons("changeBait", "opponent", 2);

        }
        else if (specialCardType == SpecialCardType.SelfHarm)
        {
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("flip", "player");

            PlayerStats.instance.flipped = true;

        }
        else if (specialCardType == SpecialCardType.BackstabSwap)
        {
            ActivateChoice(2);

        }
        else if (specialCardType == SpecialCardType.ThereThere)
        {
            ActivateChoice(2);

        }
        else if (specialCardType == SpecialCardType.NotMyProblem)
        {
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("gift", "opponent");

        }
        else if (specialCardType == SpecialCardType.Frostbite)
        {
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                if(g.GetComponent<NumberStats>().value == 2)
                {
                    //remove 2 and add 4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, 4, "player"));
                }
                if(g.GetComponent<NumberStats>().value == -2)
                {
                    //remove -2 and add -4
                    StartCoroutine(CardSelectionController.instance.ChangeNumber(g, -4, "player"));
                }
            }
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
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
            foreach (GameObject g in NumberManager.instance.reds)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach (GameObject g in NumberManager.instance.OPPreds)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("change", "player", 2);


        }
        else if (specialCardType == SpecialCardType.DirtyTrickII)
        {
            foreach (GameObject g in NumberManager.instance.yellows)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach (GameObject g in NumberManager.instance.OPPyellows)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("change", "player", 4);

        }
        else if (specialCardType == SpecialCardType.DirtyTrickIII)
        {
            foreach (GameObject g in NumberManager.instance.blues)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach (GameObject g in NumberManager.instance.OPPblues)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("change", "player", 9);

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
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("flip", "opponent");

            OpponentStats.instance.flipped = true;

        }
        else if (specialCardType == SpecialCardType.ExtraWork)
        {
            CardPlacementController.instance.DealOneCard("opponent");

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
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("change", "opponent", 5);

        }
        else if (specialCardType == SpecialCardType.GoodDeal)
        {


        }
        else if (specialCardType == SpecialCardType.EasyDay)
        {


        }
        else if (specialCardType == SpecialCardType.GoodFeeling)
        {
            if(TurnManager.instance.isPlayerTurn)
            {
                if(CardPlacementController.instance.numberDeck[0].GetComponent<NumberStats>().yellow)
                {
                    CardPlacementController.instance.DealOneCard("player");
                    yield return new WaitForSeconds(0.7f);
                    CardPlacementController.instance.DealOneCard("opponent");
                }
                else
                {
                    CardPlacementController.instance.DealOneCard("player");
                }
                
            }

            if (!TurnManager.instance.isPlayerTurn)
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


        }
        else if (specialCardType == SpecialCardType.Forgery)
        {
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("duplicate", "player");

        }
        else if (specialCardType == SpecialCardType.Pushover)
        {
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("discard", "player");

        }
        else if (specialCardType == SpecialCardType.Scavenge)
        {
            if (discardedCards.Count > 0)
            {
                ChoiceController.instance.ShowDiscardedCards(discardedCards, playerHand);

            }

        }
        else if (specialCardType == SpecialCardType.Overwhelmed)
        {
            foreach (GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("discard", "player");

        }
        else if (specialCardType == SpecialCardType.VitaminShotII)
        {
            //if current turn is player
            if (TurnManager.instance.isPlayerTurn)
            {
                SpecialCardManager.instance.Give(2, "player");
            }
            else if (!TurnManager.instance.isPlayerTurn)
            {
                //if current turn is opponent
                SpecialCardManager.instance.Give(2, "opponent");
            }

        }
        else if (specialCardType == SpecialCardType.VitaminShotV)
        {
            //if current turn is player
            if(TurnManager.instance.isPlayerTurn)
            {
                SpecialCardManager.instance.Give(5, "player");
            }
            else if(!TurnManager.instance.isPlayerTurn)
            {
                //if current turn is opponent
                SpecialCardManager.instance.Give(5, "opponent");
            }

        }

    }

    public void DiscardSpecial(GameObject g, string target)
    {
        if(target == "player")
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
            yield return new WaitForSeconds(1.5f);

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
            g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FlipOverCard(g));
            yield return new WaitForSeconds(1.5f);

            g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            g.transform.SetParent(opponentDiscardZone.transform);
            g.transform.position = opponentDiscardZone.transform.position;

            OpponentStats.instance.discarded = true;
        }
        
    }

    IEnumerator FlipOverCard(GameObject g)
    {
        float timer = 0f;
        float halfTime = 0.1f;
        CardPlace cp = g.GetComponent<CardPlace>();
        if(cp != null )
        {
            float originalScaleX = g.GetComponent<CardPlace>().correspondingImage.transform.localScale.x;

            for (timer = 0; timer < halfTime; timer += Time.deltaTime)
            {
                float scaleX = Mathf.Lerp(originalScaleX, 0f, timer / halfTime);
                g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(scaleX, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.z);
                yield return null;
            }

            g.GetComponent<CardPlace>().correspondingImage.GetComponent<Image>().sprite = grey;
            g.GetComponent<CardPlace>().correspondingImage.transform.Find("Image").GetComponent<Image>().enabled = true;
            g.GetComponent<CardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);

            for (timer = 0; timer < halfTime; timer += Time.deltaTime)
            {
                float scaleX = Mathf.Lerp(0f, originalScaleX, timer / halfTime);
                g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(scaleX, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.z);
                yield return null;
            }

            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(originalScaleX, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.y, g.GetComponent<CardPlace>().correspondingImage.transform.localScale.z);
        }
        else
        {
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


    }

    public void DrawSpecialFromDiscard()
    {
        if (discardedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, discardedCards.Count);
            GameObject chosenCard = discardedCards[randomIndex];
            discardedCards.RemoveAt(randomIndex);
            chosenCard.GetComponent<CardPlace>().beingPlayed = false;
            chosenCard.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            chosenCard.transform.SetParent(playerHand);
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
