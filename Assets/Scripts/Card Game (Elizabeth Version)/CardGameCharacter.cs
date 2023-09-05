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
}
