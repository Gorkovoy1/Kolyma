using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Deck Item", order = 2)]
public class SpecialCardSO : ScriptableObject
{
    public string cardName;
    public string description;
    public Sprite cardArt;
}
