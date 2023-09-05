using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Game Character")]
public class CardGameCharacter : ScriptableObject
{
    [Header("Character Data FILL THIS OUT")]
    public string name;
    public Sprite portrait;
    public List<SpecialDeckCard> deckList; //the list of cards in the character's decklist AT THE START OF THE GAME. Never change this during a game.

    [Header("Gameplay Variables DON'T TOUCH")]
    public List<SpecialDeckCard> currDeck; //the state of the character's deck during the game. Will be set to a copy of deckList on game start. Drawn cards are removed, this list can be shuffled in random order, whatever you have to do during a game. 
    public List<SpecialDeckCard> hand; //tracks the cards currently in the character's hand during gameplay. 
    public List<NumberCard> numberCards; //tracks the active number cards this character has during gameplay.
    public int value; //character's current value during gameplay
}
