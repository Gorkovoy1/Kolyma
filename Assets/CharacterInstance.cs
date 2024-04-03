using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance : MonoBehaviour
{
    public CardGameCharacter character;

    public List<SpecialDeckCard> deck; //the remaining special cards in the character's current deck (used during  an instance of gameplay)
    public List<SpecialDeckCard> hand; //the current hand
    //public List<NumberCard> numberHand; //the number cards the player has currently
    public List<DisplayCard> numberDisplayHand; //Using display cards to store values since values can change without changing the base number value i.e. Flip mechanic
    public List<GameObject> test;
    public int currValue; //current total value
    public bool discardFlag, swapFlag, transferFlag, flipFlag, playFlag; //flag booleans to be raised when certain card actions have been performed

    public void Init(CardGameCharacter character)
    {
        this.character = character;
    }

    public void FlushFlags()
    {
        discardFlag = false;
        swapFlag = false;
        transferFlag = false;
        flipFlag = false;
        playFlag = false;
    }
    public void FlushGameplayVariables()
    {
        deck = new List<SpecialDeckCard>();
        hand = new List<SpecialDeckCard>();
        numberDisplayHand = new List<DisplayCard>();
        test = new List<GameObject>();
        deck.Clear();
        hand.Clear();
        numberDisplayHand.Clear();
        test.Clear();
        currValue = 0;
        FlushFlags();
    }
}
