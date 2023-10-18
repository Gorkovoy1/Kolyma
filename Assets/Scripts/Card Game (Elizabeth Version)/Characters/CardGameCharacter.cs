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
    public int currValue; //current total value
}
