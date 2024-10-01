using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class DisplayCard : MonoBehaviour
{
    public GenericCard baseCard;
    public CharacterInstance owner; //This will be used to track WHOSE card this is in its current iteration. It is stored here instead of in the card data so that each instance of a card can have its own owner.
    //doing it this way allows there to be two instances of the In Cahoots card in play, for example, where the player owns one and the AI owns the other.
    [SerializeField] private Image artwork;
    [SerializeField] private TMP_Text name;
    [SerializeField] public TMP_Text description;
    [SerializeField] public int value;
    [SerializeField] private GameObject selectionEffect;
    [SerializeField] private GameObject tooltip;
    public Transform NumberAnchor;

    public NumberCardOrganizer NumberCardOrganizer;

    public SpecialDeckCard SpecialCard;

    public Transform Artwork;

    public Button SelectButton;

    private float OGScale = 0.25f;
    private float OGWidth = 500f, OGHeight = 300f;

    public Color OgColor, SelectedColor;

    public bool SwappedThisTurn, FlippedThisTurn, Given;

    public bool Hidden;

    public GenericCard CurrCard;

    public bool Playable;
    public GameObject PlayableIndicator;

    void Start() {
        selectionEffect.SetActive(false);
        tooltip.SetActive(false);
        transform.localScale = new Vector3(OGScale, OGScale, 1);
        GetComponent<RectTransform>().sizeDelta = new Vector2(OGWidth, OGHeight);
    }

    private void Update()
    {
        PlayableIndicator.SetActive(Playable);
    }
    /*void Update() {
        if(baseCard != null) {
           if(baseCard is NumberCard){
                NumberCard card = (NumberCard) baseCard;
                Card(card);
            }
            else {
                SpecialDeckCard card = (SpecialDeckCard) baseCard;
                description.text = card.description;
                Card(card);
            } 
        } 
    }*/

    public void InitNumberCard(NumberCard card, CharacterInstance owner)
    {
        CurrCard = card;
        gameObject.name = card.name;
        baseCard = card;
        name.text = card.name;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;
        description.text = "";
        value = card.value;
        this.owner = owner;
        SelectButton.onClick.RemoveAllListeners();
        SelectButton.onClick.AddListener(delegate { CardGameManager.Instance.CardSelectionHandler.SelectCard(this, CardGameManager.Instance.player);});
    }

    public void InitSpecialCard(SpecialDeckCard card, CharacterInstance owner)
    {
        CurrCard = card;
        gameObject.name = card.name;
        baseCard = card;
        SpecialCard = card;
        name.text = card.name;
        description.text = card.description;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;
        this.owner = owner;
        SelectButton.onClick.RemoveAllListeners();
        SelectButton.onClick.AddListener(delegate { CardGameManager.Instance.CardSelectionHandler.SelectCard(this, CardGameManager.Instance.player); });
    }

    public void SetHidden(bool hidden)
    {
        Hidden = hidden;
        artwork.sprite = Hidden ? null : CurrCard.artwork;
    }

    public void ToggleSelectionColor(bool selected)
    {
        SelectButton.GetComponent<Image>().color = selected ? SelectedColor : OgColor;
    }

    public void ResetFlags()
    {
        FlippedThisTurn = false;
        SwappedThisTurn = false;
        Given = false;
    }

    /*private void Card(NumberCard card) {
        name.text = card.name;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;
        description.text = "";
        value = card.value;
        /*calculate the location of NumberAnchor based on the cards value, use local space so that it's relative to the display card's universe and not the scene as a whole*/
    /*}
    private void Card(SpecialDeckCard card){
        name.text = card.name;
        description.text = card.description;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;

    }*/

    public void ToggleSelected() {
        selectionEffect.SetActive(!selectionEffect.activeSelf);
    }
    public void SetHover(bool hovered) {
        tooltip.SetActive(hovered);
    }
}
