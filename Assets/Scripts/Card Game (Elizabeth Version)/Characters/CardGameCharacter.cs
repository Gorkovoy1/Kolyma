using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Game Character")]
public class CardGameCharacter : ScriptableObject
{
    [Header("Character Info")]
    public string name;
    public Sprite portrait;
    public List<SpecialDeckCard> deckList; //The cards a character will use in the card game

    [Header("Ingame Variables - Code Use Only, Do Not Fill Out Here")]
    public List<SpecialDeckCard> deck; //the remaining special cards in the character's current deck (used during  an instance of gameplay)
    public List<SpecialDeckCard> hand; //the current hand
    public List<NumberCard> numberHand; //the number cards the player has currently
    public int currValue; //current total value
    public bool discardFlag, swapFlag, transferFlag, flipFlag, playFlag; //flag booleans to be raised when certain card actions have been performed

    public void FlushFlags(){
        discardFlag = false;
        swapFlag = false;
        transferFlag = false;
        flipFlag = false;
        playFlag = false;
    }
    public void FlushGameplayVariables()
    {
        deck.Clear();
        hand.Clear();
        numberHand.Clear();
        currValue = 0;
        FlushFlags();
    }
}
