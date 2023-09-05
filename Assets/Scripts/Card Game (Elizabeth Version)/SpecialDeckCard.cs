using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Card")]
public class SpecialDeckCard : ScriptableObject
{
    public enum Keyword {

    };

    public bool activate;
    public bool condition;
    public string description;
    public Sprite cardBack;
    public Sprite artwork;

}
