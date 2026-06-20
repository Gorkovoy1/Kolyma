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




    public List<GameObject> discardedCards;
    public bool discardUpdated;

    public Sprite cardBack;
    public Sprite grey;

    //public bool isFlipping;
    public bool isFlipped;

    public AK.Wwise.Event trickSound;
    //public GameObject sfxObj;

    public bool delayImageSpawn = false;

    public CardState cardState;

    public bool inDropZone = false;


    public float baseY;

    public enum CardState
    {
        Idle,
        Dealing,
        Dragging,
        Playing
    }

    void Awake()
    {
        cardState = CardState.Dealing;
    }

    // Start is called before the first frame update
    void Start()
    {
        

        //reset bools and set references
        discardUpdated = TurnManager.instance.discardUpdated;
        isFlipped = false;
        playerHand = this.transform.parent;
        opponentHand = playerHand.parent.Find("OpponentHand");
        parentReturnTo = playerHand;
        //set images parent
        //set discard zone obj
        playerDiscardZone = GameObject.FindWithTag("PlayerDiscard");
        opponentDiscardZone = GameObject.FindWithTag("OpponentDiscard");

        if (imagePrefab != null && !delayImageSpawn)
        {
            SpawnImage();
        }

        baseY = -257.8f;
    }

    void ResetScale()
    {
        this.correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    void DeactivateOutline()
    {
        Transform outline = this.correspondingImage.transform.Find("Outline");
        if (outline != null)
        {
            outline.gameObject.SetActive(false);
        }
    }

    void ActivateOutline()
    {
        Transform outline = this.correspondingImage.transform.Find("Outline");
        if (outline != null)
        {
            outline.gameObject.SetActive(true);
        }
    }

    void StartFlipped()
    {
        //start with card back
        grey = correspondingImage.GetComponent<Image>().sprite;
        correspondingImage.GetComponent<Image>().sprite = cardBack;
        correspondingImage.transform.Find("Image").GetComponent<Image>().enabled = false;
    }

    public void SpawnImage()
    {
        //this means its a special card
        imagesParent = this.GetComponentInParent<HandController>().imagesParent;
        correspondingImage = Instantiate(imagePrefab, imagesParent);
        correspondingImage.GetComponent<SpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();

        DeactivateOutline();

        //start flipped
        if (this.transform.parent == opponentHand)
        {
            StartFlipped();
        }

        //reset scale and description (unhovered)
        ResetScale();
        //deactivate description
        this.correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;


        if (imagePrefab == null) //not special card
            return;

        if(!TurnManager.instance.isPlayerTurn)
        {
            DeactivateOutline();
            return;
        }

        if (transform.parent != playerHand)
            return;

        bool newPlayable = CheckPlayable();
        if (newPlayable != isPlayable)
            isPlayable = newPlayable;

        if (isPlayable && cardState == CardState.Idle)
            ActivateOutline();
        else
            DeactivateOutline();

        discardedCards = NumberManager.instance.discardedCards;
         

        if (hovering && !IsPointerOverMe())
        {
            hovering = false;
            UnHoverOverCard();
        }

        //remove this?
        NumberManager.instance.recalculate = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;

        if (cardState != CardState.Idle)
            return;

        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            hovering = false;
            dragging = true;
            inDropZone = false;
            cardState = CardState.Dragging;

            parentReturnTo = this.transform.parent;
            this.correspondingImage.transform.SetAsLastSibling();
            this.transform.SetParent(parentReturnTo.transform.parent);

            playerHand.GetComponent<HandFanController>().dragging = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;


        if (cardState != CardState.Dragging)
            return;

        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            dragging = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            playerHand.GetComponent<HandFanController>().dragging = false;

            if(CheckPlayable() && inDropZone)
            {
                cardState = CardState.Playing;
                DeactivateOutline();
                AnimateBeingPlayed();
            }
            else
            {
                cardState = CardState.Idle;
                this.transform.SetParent(parentReturnTo);
                UnHoverOverCard();
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;

        if (cardState != CardState.Dragging)
            return;

        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand")
        {
            this.transform.position = eventData.position;
            this.correspondingImage.transform.SetAsLastSibling();
            correspondingImage.transform.SetAsLastSibling();
            this.correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);
            ResetScale();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;

        if (cardState != CardState.Idle)
            return;

        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name != "OpponentHand" && this.gameObject.transform.parent.name != "PlayerDiscard" 
            && !playerHand.GetComponent<HandFanController>().dragging && playerHand.GetComponent<HandFanController>().hoverable)
        {
            hovering = true;
            HoverOverCard();
        }
    }

    void HoverOverCard() //enlarge card and show description
    {
        if (correspondingImage != null && cardState == CardState.Idle)
        {
            correspondingImage.transform.SetAsLastSibling();
            correspondingImage.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            correspondingImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(correspondingImage.GetComponent<RectTransform>().anchoredPosition.x, baseY + hoverOffset, 0f);
            correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    void UnHoverOverCard()
    {
        if (correspondingImage != null && cardState == CardState.Idle)
        {
            correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            correspondingImage.transform.position -= new Vector3(0f, hoverOffset, 0f);
            correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    bool IsPointerOverMe()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            (RectTransform)transform,
            Input.mousePosition,
            null
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //prevent actions when loading scenes
        if (SceneLoader.instance.isLoading)
            return;


        if (cardState != CardState.Idle)
            return;

        if (!gameObject.TryGetComponent<NumberStats>(out var component) && this.gameObject.transform.parent.name == "PlayerHand" && !playerHand.GetComponent<HandFanController>().dragging 
            && hovering && playerHand.GetComponent<HandFanController>().hoverable)
        {
            hovering = false;
            UnHoverOverCard();
        }
    }

    public void AnimateBeingPlayed()
    {
        cardState = CardState.Playing;
        TurnManager.instance.playerPlayedCard = true;

        //play trick sound
        if(trickSound != null)
        {
            trickSound.Post(this.gameObject);
        }

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

    IEnumerator LerpScaleDown(Transform target, float duration)
    {
        Vector3 startScale = new Vector3(0.17f, 0.17f, 0.17f);
        Vector3 endScale = new Vector3(0.01f, 0.01f, 0.01f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = endScale; // ensure final scale is exact
    }

    IEnumerator BeingPlayed()
    {
        playerHand.parent.GetComponent<HandController>().passAnimationController.playerLastTurnPassed = false;

        RectTransform parentRect = opponentDiscardZone.transform.parent as RectTransform;
        this.transform.SetParent(opponentDiscardZone.transform.parent, false);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(parentRect.rect.width / 2f, -parentRect.rect.height / 2f);
        correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
        yield return new WaitForSeconds(1f);

        StartCoroutine(LerpScaleDown(correspondingImage.transform, 0.2f));
        this.transform.SetParent(playerDiscardZone.transform);
        this.transform.position = playerDiscardZone.transform.position;

        cardState = CardState.Idle;

        UpdateDiscardPile();

        StartCoroutine(PlayCorrespondingAction());
    }

    void UpdateDiscardPile()
    {
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
    }

    public bool CheckPlayable()
    {
        if (TurnManager.instance.playerPlayedCard)
            return false;


        switch (specialCardType)
        {
            case SpecialCardType.CaughtRedHanded:
                return NumberManager.instance.OPPyellows.Count > 0;

            case SpecialCardType.EmptyPockets:
                return NumberManager.instance.OPPblues.Count > 0;

            case SpecialCardType.Burden:
                return NumberManager.instance.OPPreds.Count > 0;

            case SpecialCardType.RifleButt:
                return NumberManager.instance.OPPreds.Count > 0;

            case SpecialCardType.SmokeBreak:
                return discardedCards.Count > 0;

            case SpecialCardType.Weakness:
                return discardedCards.Count > 0 && NumberManager.instance.OPPpositives.Exists(g => g.GetComponent<NumberStats>().value == 2);

            case SpecialCardType.ThickWoolenCoat:
                return true;

            case SpecialCardType.Setup:
                return NumberManager.instance.OPPnegatives.Count > 0;

            case SpecialCardType.Bribe:
                return NumberManager.instance.duplicates.Count > 0;

            case SpecialCardType.Fist:
                return NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0;

            case SpecialCardType.CondensedMilk:
                return true;

            case SpecialCardType.InCahoots:
                return true;

            case SpecialCardType.Search:
                return true;

            case SpecialCardType.Poison:
                return true;

            case SpecialCardType.BackstabDiscard:
                return OpponentStats.instance.discarded;

            case SpecialCardType.Scam:
                return true;

            case SpecialCardType.GiveItUp:
                return NumberManager.instance.negatives.Count > 0;

            case SpecialCardType.Rotation:
                return NumberManager.instance.negatives.Count > 0;

            case SpecialCardType.DirtyTrickIV:
                return NumberManager.instance.OPPyellows.Count > 0;

            case SpecialCardType.BaitAndSwitch:
                return NumberManager.instance.OPPallNumbers.Exists(g => g.GetComponent<NumberStats>().value == 2) || NumberManager.instance.OPPallNumbers.Exists(g => g.GetComponent<NumberStats>().value == -2);

            case SpecialCardType.SelfHarm:
                return true;

            case SpecialCardType.BackstabSwap:
                return OpponentStats.instance.swapped;

            case SpecialCardType.ThereThere:
                return NumberManager.instance.duplicates.Count > 0;

            case SpecialCardType.NotMyProblem:
                return true;

            case SpecialCardType.Frostbite:
                return NumberManager.instance.OPPallNumbers.Exists(g => g.GetComponent<NumberStats>().value == 2) || NumberManager.instance.OPPallNumbers.Exists(g => g.GetComponent<NumberStats>().value == -2) ||
                NumberManager.instance.allNumbers.Exists(g => g.GetComponent<NumberStats>().value == 2) || NumberManager.instance.allNumbers.Exists(g => g.GetComponent<NumberStats>().value == -2);

            case SpecialCardType.DirtyTrickI:
                return NumberManager.instance.OPPreds.Count > 0 || NumberManager.instance.reds.Count > 0;

            case SpecialCardType.DirtyTrickII:
                return NumberManager.instance.OPPyellows.Count > 0 || NumberManager.instance.yellows.Count > 0;

            case SpecialCardType.DirtyTrickIII:
                return NumberManager.instance.OPPblues.Count > 0 || NumberManager.instance.blues.Count > 0;

            case SpecialCardType.LousyDeal:
                return true;

            case SpecialCardType.FindersKeepers:
                return NumberManager.instance.reds.Count > 0;

            case SpecialCardType.Gossip:
                return true;

            case SpecialCardType.FairShare:
                return NumberManager.instance.OPPnegatives.Count > 0;

            case SpecialCardType.SleeplessNight:
                return OpponentStats.instance.swapped;

            case SpecialCardType.Payback:
                return OpponentStats.instance.gave;

            case SpecialCardType.Knife:
                return true;

            case SpecialCardType.ExtraWork:
                return OpponentStats.instance.flipped;

            case SpecialCardType.Scratch:
                return OpponentStats.instance.gave;

            case SpecialCardType.Leftovers:
                return OpponentStats.instance.discarded;

            case SpecialCardType.Glare:
                return true;

            case SpecialCardType.Snitch:
                return OpponentStats.instance.discarded;

            case SpecialCardType.GoodDeal:
                return true;

            case SpecialCardType.EasyDay:
                return true;

            case SpecialCardType.GoodFeeling:
                return true;

            case SpecialCardType.Forgery:
                return true;

            case SpecialCardType.Pushover:
                return true;

            case SpecialCardType.Scavenge:
                return true;

            case SpecialCardType.Overwhelmed:
                return NumberManager.instance.allNumbers.Count > 5;

            case SpecialCardType.VitaminShotII:
                return true;

            case SpecialCardType.VitaminShotV:
                return true;

            default:
                return false;

        }
    }

    IEnumerator PlayCorrespondingAction()
    {
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
            foreach(GameObject g in NumberManager.instance.OPPyellows)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("gift", "opponent");

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
            foreach(GameObject g in NumberManager.instance.allNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            foreach(GameObject g in NumberManager.instance.OPPallNumbers)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }
            CardSelectionController.instance.CallButtons("trade", "player");

        }
        else if (specialCardType == SpecialCardType.FindersKeepers)
        {


        }
        else if (specialCardType == SpecialCardType.Gossip)
        {


        }
        else if (specialCardType == SpecialCardType.FairShare)
        {
            foreach (GameObject g in NumberManager.instance.OPPnegatives)
            {
                g.GetComponent<NumberStats>().selectable = true;
            }

            CardSelectionController.instance.CallButtons("gift", "opponent");

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
            GameObject randomSpecial = opponentHand.GetChild(Random.Range(0, opponentHand.childCount)).gameObject;
            StartCoroutine(DiscardAnimation(randomSpecial, "opponent"));

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
        NumberManager.instance.recalculate = true;
    }

    public void DiscardSpecial(GameObject g, string target)
    {
        if(target == "player")
        {
            PlayerStats.instance.discarded = true;
            g.transform.SetParent(playerDiscardZone.transform);
            g.transform.position = playerDiscardZone.transform.position;
        }
        else //opponent
        {
            OpponentStats.instance.discarded = true;
            opponentHand.GetComponentInParent<HandController>().oppDiscardButton.GetComponent<OpponentDiscardButton>().lastPlayed = g.GetComponent<AICardPlace>().correspondingImage.GetComponent<Image>();
            foreach (var tmp in g.GetComponent<AICardPlace>().correspondingImage.GetComponentsInChildren<TextMeshProUGUI>(true)) // true = include inactive
            {
                if (tmp.name == "Text (TMP) (1)") // Replace with your actual object name
                {
                    opponentHand.GetComponentInParent<HandController>().oppDiscardButton.GetComponent<OpponentDiscardButton>().cardDesc = tmp.text;
                    break;
                }
            }
            g.transform.SetParent(opponentDiscardZone.transform);
            g.transform.position = opponentDiscardZone.transform.position;
        }
        
    }

    IEnumerator DiscardAnimation(GameObject g, string target)
    {
        RectTransform parentRect = opponentDiscardZone.transform.parent as RectTransform;

        if (target == "player")
        {
            g.GetComponent<CardPlace>().isPlayable = false;
            g.GetComponent<CardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(true);
            g.transform.SetParent(opponentDiscardZone.transform.parent, false);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(parentRect.rect.width / 2f, -parentRect.rect.height / 2f);
            g.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
            //1.5
            yield return new WaitForSeconds(0.5f);
            //flash trash can for 0.5f
            AkSoundEngine.PostEvent("Play_Discard", playerHand.GetComponentInParent<HandController>().sfxObj);
            g.GetComponent<CardPlace>().correspondingImage.transform.Find("TRASHED").gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            g.GetComponent<CardPlace>().correspondingImage.transform.Find("TRASHED").gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(LerpScaleDown(g.GetComponent<CardPlace>().correspondingImage.transform, 0.2f));
            g.transform.SetParent(playerDiscardZone.transform);
            g.transform.position = playerDiscardZone.transform.position;

            PlayerStats.instance.discarded = true;

            g.GetComponent<CardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            //is card back
            //squeeze to 0 then stretch back
            //when 0, hide cardback and show card image
            g.GetComponent<AICardPlace>().isPlayable = false;
            g.transform.SetParent(opponentDiscardZone.transform.parent, false);
            g.GetComponent<RectTransform>().anchoredPosition = new Vector2(parentRect.rect.width / 2f, -parentRect.rect.height / 2f);
            g.GetComponent<AICardPlace>().correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FlipOverCard(g));
            //1.5
            yield return new WaitForSeconds(0.5f);
            //flash trash can for 0.5f
            AkSoundEngine.PostEvent("Play_Discard", playerHand.GetComponentInParent<HandController>().sfxObj);
            g.GetComponent<AICardPlace>().correspondingImage.transform.Find("TRASHED").gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            g.GetComponent<AICardPlace>().correspondingImage.transform.Find("TRASHED").gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(LerpScaleDown(g.GetComponent<AICardPlace>().correspondingImage.transform, 0.2f));
            g.transform.SetParent(opponentDiscardZone.transform);
            g.transform.position = opponentDiscardZone.transform.position;

            OpponentStats.instance.discarded = true; 
            
            //add discarded card to discard pile
            opponentHand.GetComponentInParent<HandController>().oppDiscardButton.GetComponent<OpponentDiscardButton>().lastPlayed = g.GetComponent<AICardPlace>().correspondingImage.GetComponent<Image>();
            foreach (var tmp in g.GetComponent<AICardPlace>().correspondingImage.GetComponentsInChildren<TextMeshProUGUI>(true)) // true = include inactive
            {
                if (tmp.name == "Text (TMP) (1)") // Replace with your actual object name
                {
                    opponentHand.GetComponentInParent<HandController>().oppDiscardButton.GetComponent<OpponentDiscardButton>().cardDesc = tmp.text;
                    break;
                }
            }


            g.GetComponent<AICardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
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
            int randomIndex = Random.Range(0, discardedCards.Count - 1);
            GameObject chosenCard = discardedCards[randomIndex];
            discardedCards.RemoveAt(randomIndex);
            chosenCard.GetComponent<CardPlace>().beingPlayed = false;
            chosenCard.GetComponent<CardPlace>().correspondingImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            chosenCard.GetComponent<CardPlace>().correspondingImage.GetComponentInChildren<TextMeshProUGUI>(true).gameObject.transform.parent.gameObject.SetActive(false);
            chosenCard.transform.SetParent(playerHand);
            AkSoundEngine.PostEvent("Play_Trick_Card", playerHand.GetComponentInParent<HandController>().sfxObj);
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
