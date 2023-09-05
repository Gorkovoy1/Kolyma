using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Number Card")]
public class NumberCard : ScriptableObject
{
    public string name;
    public int value;
    public Sprite artwork;
}
