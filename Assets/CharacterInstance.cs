using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance : MonoBehaviour
{
    public CardGameCharacter character;

    public NumberCardOrganizer PositiveCardsZone, NegativeCardsZone;

    public List<SpecialDeckCard> deck; //the remaining special cards in the character's current deck (used during  an instance of gameplay)
    public List<SpecialDeckCard> hand; //the current hand
    //public List<NumberCard> numberHand; //the number cards the player has currently
    public List<DisplayCard> numberDisplayHand; //Using display cards to store values since values can change without changing the base number value i.e. Flip mechanic
    public List<GameObject> test;
    public int currValue; //current total value
    public int addedValues; //Added from card effects
    public bool DiscardedThisTurn, SwappedThisTurn, GaveThisTurn, FlippedThisTurn, PlayedThisTurn; //flag booleans to be raised when certain card actions have been performed

    public bool CurrentlySwapping, CurrentlyFlipping;
    public bool SwappingForced, FlippingForced;

    public void Init(CardGameCharacter character, NumberCardOrganizer positiveCardZone, NumberCardOrganizer negativeCardZone)
    {
        this.character = character;
        PositiveCardsZone = positiveCardZone;
        NegativeCardsZone = negativeCardZone;
    }

    public void AddValue(int value)
    {
        addedValues += value;
        CardGameManager.Instance.UpdateValues();
    }

    public void ToggleForcedSwap(bool on)
    {
        SwappingForced = on;
    }

    public void ToggleForceFlip(bool on)
    {
        FlippingForced = on;
    }

    public void FlushFlags()
    {
        DiscardedThisTurn = false;
        SwappedThisTurn = false;
        GaveThisTurn = false;
        FlippedThisTurn = false;
        PlayedThisTurn = false;
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
