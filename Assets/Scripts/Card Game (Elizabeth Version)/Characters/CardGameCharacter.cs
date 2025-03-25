using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Game Character")]
public class CardGameCharacter : ScriptableObject
{
    [Header("Character Info")]
    public string name;
    public Sprite portrait;
    public List<SpecialDeckCard> deckList; //The cards a character will use in the card game. Replace with owned cards for player
    public AIPersonality AIPersonality;
}
