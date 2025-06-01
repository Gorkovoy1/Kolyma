using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using TMPro;

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

    public Transform playerHand;

    private bool numberCard;

    public int childCount;

    public SpecialCardType specialCardType;

    private float duration = 1f;

    public bool isPlayable; //for the player


    public Material defaultMat;
    public Material playableMat;

    // Start is called before the first frame update
    void Awake()
    {
        
        playerHand = this.transform.parent;
        parentReturnTo = playerHand;
        //set images parent
        //set discard zone obj

        if (imagePrefab != null)
        {
            //this means its a special card
            correspondingImage = Instantiate(imagePrefab, imagesParent);
            correspondingImage.GetComponent<SpecialCardMovement>().target = this.gameObject.GetComponent<RectTransform>();
            defaultMat = correspondingImage.GetComponent<Image>().material;
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
                if(isPlayable)
                {
                    //outline card in green
                    //change material to outline shader
                    //change back when playing card
                    correspondingImage.GetComponent<Image>().material = playableMat;
                }
                else
                {
                    isPlayable = false;
                }
            }
            
        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gameObject.TryGetComponent<NumberStats>(out var component))
        {
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
        if (!gameObject.TryGetComponent<NumberStats>(out var component))
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
        if (!gameObject.TryGetComponent<NumberStats>(out var component))
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
        if (!gameObject.TryGetComponent<NumberStats>(out var component))
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
        if (!gameObject.TryGetComponent<NumberStats>(out var component))
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
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(400f, 0f, 0);
        correspondingImage.transform.localScale = new Vector3(0.17f, 0.17f, 0.17f);
        yield return new WaitForSeconds(1f);

        correspondingImage.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        this.transform.SetParent(playerDiscardZone.transform);
        this.transform.position = playerDiscardZone.transform.position;

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
        else if (specialCardType == SpecialCardType.Bribe) //in process
        {
            
            List<GameObject> duplicates = NumberManager.instance.OPPallNumbers.GroupBy(obj => obj.GetComponent<NumberStats>().value)
                .Where (g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();
            

            foreach (GameObject go in duplicates)
            {
                Debug.Log("Duplicate GameObject: " + go.name);
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
            isPlayable = true;

        }
        else if (specialCardType == SpecialCardType.SelfHarm)
        {
            isPlayable = true;

        }
        else if (specialCardType == SpecialCardType.BackstabSwap)
        {
            //if opp swapped then playable

        }
        else if (specialCardType == SpecialCardType.ThereThere)
        {
            List<GameObject> duplicates = NumberManager.instance.allNumbers.GroupBy(obj => obj.GetComponent<NumberStats>().value)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();


            foreach (GameObject go in duplicates)
            {
                Debug.Log("Duplicate GameObject: " + go.name);
            }

        }
        else if (specialCardType == SpecialCardType.NotMyProblem)
        {
            isPlayable = true;

        }
        else if (specialCardType == SpecialCardType.Frostbite)
        {
            //if ahve 2 or -2

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

        }
        else if (specialCardType == SpecialCardType.Scratch)
        {
            //if opp gave u something

        }
        else if (specialCardType == SpecialCardType.Leftovers)
        {
            //if opp discarded

        }
        else if (specialCardType == SpecialCardType.Glare)
        {
            isPlayable = true;

        }
        else if (specialCardType == SpecialCardType.Snitch)
        {
            //if opp discarded

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
            //check is done in checkplayable
            //highlight all number cards, access a highlighter obj that highlights all red, blue, yellow, centrally controlled
            //allow player to select those yellow cards
            //destroy it, discard, and deal new number
            
        }
        else if (specialCardType == SpecialCardType.EmptyPockets)
        {
            

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


        }
        else if (specialCardType == SpecialCardType.SmokeBreak)
        {


        }
        else if (specialCardType == SpecialCardType.Weakness)
        {


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


        }
        else if (specialCardType == SpecialCardType.Bribe)
        {


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


        }
        else if (specialCardType == SpecialCardType.InCahoots)
        {
            SpecialCardManager.instance.Give(4, "player");
            SpecialCardManager.instance.Give(4, "opponent");
        }
        else if (specialCardType == SpecialCardType.Search)
        {


        }
        else if (specialCardType == SpecialCardType.Poison)
        {


        }
        else if (specialCardType == SpecialCardType.BackstabDiscard)
        {


        }
        else if (specialCardType == SpecialCardType.Scam)
        {


        }
        else if (specialCardType == SpecialCardType.GiveItUp)
        {


        }
        else if (specialCardType == SpecialCardType.Rotation)
        {


        }
        else if (specialCardType == SpecialCardType.DirtyTrickIV)
        {


        }
        else if (specialCardType == SpecialCardType.BaitAndSwitch)
        {


        }
        else if (specialCardType == SpecialCardType.SelfHarm)
        {


        }
        else if (specialCardType == SpecialCardType.BackstabSwap)
        {


        }
        else if (specialCardType == SpecialCardType.ThereThere)
        {


        }
        else if (specialCardType == SpecialCardType.NotMyProblem)
        {


        }
        else if (specialCardType == SpecialCardType.Frostbite)
        {


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


        }
        else if (specialCardType == SpecialCardType.ExtraWork)
        {


        }
        else if (specialCardType == SpecialCardType.Scratch)
        {


        }
        else if (specialCardType == SpecialCardType.Leftovers)
        {


        }
        else if (specialCardType == SpecialCardType.Glare)
        {


        }
        else if (specialCardType == SpecialCardType.Snitch)
        {


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


        }
        else if (specialCardType == SpecialCardType.Pushover)
        {


        }
        else if (specialCardType == SpecialCardType.Scavenge)
        {


        }
        else if (specialCardType == SpecialCardType.Overwhelmed)
        {


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
