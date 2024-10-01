using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance : MonoBehaviour
{
    public CardGameCharacter character;

    public NumberCardOrganizer PositiveCardsZone, NegativeCardsZone;

    public List<SpecialDeckCard> deck; //the remaining special cards in the character's current deck (used during  an instance of gameplay)
    //public List<SpecialDeckCard> hand; //the current hand
    //public List<NumberCard> numberHand; //the number cards the player has currently

    public List<DisplayCard> specialDisplayHand;
    public List<DisplayCard> numberDisplayHand; //Using display cards to store values since values can change without changing the base number value i.e. Flip mechanic
    public int currValue; //current total value
    //public int addedValues; //Added from card effects

    public int DiscardedFlag, SwappedFlag, GaveFlag, FlippedFlag, PlayedFlag, MulliganedFlag; //flag booleans to be raised when certain card actions have been performed by self. Set to 2 to last a full turn cycle

    public bool CurrentlySwapping, CurrentlyFlipping, CurrentlyMulliganing;
    public bool SwappingForced, FlippingForced, DidAnAction;

    public List<DisplayCard> NewlyDrawnNumberCards, NewlyDrawnSpecialCards;

    //Only used for Easy Day for now. Only coded for that specific instance
    public int NumberOfNewlyDiscardCards;

    public bool IsAI;
    private CardGameAI AIScript;

    public int Dice1, Dice2;

    public CharacterInstance Opponent;

    public bool HadATurn;

    public void Init(CardGameCharacter character, NumberCardOrganizer positiveCardZone, NumberCardOrganizer negativeCardZone, bool isAI, CharacterInstance opponent)
    {
        this.character = character;
        PositiveCardsZone = positiveCardZone;
        NegativeCardsZone = negativeCardZone;
        IsAI = isAI;
        if(IsAI)
        {
            AIScript = gameObject.AddComponent<CardGameAI>();
            AIScript.Init(this, opponent);
        }
        FlushGameplayVariables();
    }

    public void RandomlyChooseDeck()
    {
        List<SpecialDeckCard> possibleCards = new List<SpecialDeckCard>(character.deckList);
        for(int i = 0; i < 15; i++)
        {
            int chosenCardIndex = Random.Range(0, possibleCards.Count);
            deck.Add(possibleCards[chosenCardIndex]);
            possibleCards.RemoveAt(chosenCardIndex);
        }
    }

    public void AddCardToDeck(DisplayCard card)
    {
        deck.Add(card.SpecialCard);
    }

    public DisplayCard AddValue(int value)
    {
        Debug.Log("Add Value: " + value);
        GameObject newCard = NumberCardPool.Instance.GetValue(value, this);
        if(value < 0)
        {
            NegativeCardsZone.PlaceCard(newCard);
        }
        else
        {
            PositiveCardsZone.PlaceCard(newCard);
        }
        numberDisplayHand.Add(newCard.GetComponent<DisplayCard>());
        //addedValues += value;
        CardGameManager.Instance.UpdateValues();
        return newCard.GetComponent<DisplayCard>();
    }

    public void ToggleForcedSwap(bool on)
    {
        SwappingForced = on;
    }

    public void ToggleForceFlip(bool on)
    {
        FlippingForced = on;
    }

    public void ApplyFlagsAtEndOfTurn()
    {

    }

    public void DecrementFlags()
    {
        if (DiscardedFlag > 0) DiscardedFlag--;
        if (FlippedFlag > 0) FlippedFlag--;
        if (GaveFlag > 0) GaveFlag--;
        if (SwappedFlag > 0) SwappedFlag--;
        if (PlayedFlag > 0) PlayedFlag--;
        if (MulliganedFlag > 0) MulliganedFlag--;
    }

    public void FlushGameplayVariables()
    {
        deck = new List<SpecialDeckCard>();
        //hand = new List<SpecialDeckCard>();
        specialDisplayHand = new List<DisplayCard>();
        numberDisplayHand = new List<DisplayCard>();
        //test = new List<GameObject>();
        deck.Clear();
        //hand.Clear();
        specialDisplayHand.Clear();
        numberDisplayHand.Clear();
        //test.Clear();
        currValue = 0;
        DecrementFlags();
        DecrementFlags();
    }
}
