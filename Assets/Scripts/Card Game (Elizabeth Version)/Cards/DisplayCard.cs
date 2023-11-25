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

    void Card(NumberCard card) {
        name.text = card.name;
        artwork.sprite = card.artwork;
        artwork.color = Color.white;
        description.text = "";
    }
    void Card(SpecialDeckCard card){
        name.text = card.name;
        description.text = card.description;
        artwork.sprite = card.artwork;
        
        if(card.cardClass == SpecialClass.NONE) {
            artwork.color = Color.white;
        }
        else if(card.cardClass == SpecialClass.BLACK) {
            artwork.color = Color.grey;
        }
        else if(card.cardClass == SpecialClass.BLUE) {
            artwork.color = Color.blue;
        }
        else if(card.cardClass == SpecialClass.RED) {
            artwork.color = Color.red;
        }
        else if(card.cardClass == SpecialClass.GREEN) {
            artwork.color = Color.green;
        }
        else if(card.cardClass == SpecialClass.YELLOW) {
            artwork.color = Color.yellow;
        }

    }
}
