using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Number Card")]
public class NumberCard : GenericCard
{
    public enum NumberClass {
        RED = 1,
        BLUE = 2,
        GREEN = 3,
        YELLOW = 4,
        BLACK = 5,
        NONE = 6
    };

    public NumberClass cardClass;
    public int value;
}
