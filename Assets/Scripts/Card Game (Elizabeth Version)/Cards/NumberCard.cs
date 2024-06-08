using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Number Card")]
public class NumberCard : GenericCard
{
    public enum NumberClass {
        NONE = 0,
        RED = 1,
        BLUE = 2,
        GREEN = 3,
        YELLOW = 4,
        BLACK = 5
    };

    public NumberClass cardClass;
    public int value;
    public NumberCard oppositeCard;
}
