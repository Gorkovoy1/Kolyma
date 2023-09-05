using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Game Character")]
public class CardGameCharacter : ScriptableObject
{
    [Header("Character Data")]
    public string name;
    public Sprite portrait;
    public List<SpecialDeckCard> deckList; //the list of special cards in the character's decklist
}
