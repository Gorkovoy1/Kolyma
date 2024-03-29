using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class DisplayCard : MonoBehaviour
{
    public GenericCard baseCard;
    [HideInInspector] public CardGameCharacter owner; //This will be used to track WHOSE card this is in its current iteration. It is stored here instead of in the card data so that each instance of a card can have its own owner.
    //doing it this way allows there to be two instances of the In Cahoots card in play, for example, where the player owns one and the AI owns the other.
    [SerializeField] private Image artwork;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text description;
    [SerializeField] private int value;
    [SerializeField] private GameObject selectionEffect;
    [SerializeField] private GameObject tooltip;
    public Transform NumberAnchor;
    void Start() {
        selectionEffect.SetActive(false);
        tooltip.SetActive(false);
    }
    void Update() {
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
    }

    private void Card(NumberCard card) {
        name.text = card.name;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;
        description.text = "";
        /*calculate the location of NumberAnchor based on the cards value, use local space so that it's relative to the display card's universe and not the scene as a whole*/
    }
    private void Card(SpecialDeckCard card){
        name.text = card.name;
        description.text = card.description;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;

    }

    public void ToggleSelected() {
        selectionEffect.SetActive(!selectionEffect.activeSelf);
    }
    public void ToggleHover() {
        tooltip.SetActive(!tooltip.activeSelf);
    }
}
