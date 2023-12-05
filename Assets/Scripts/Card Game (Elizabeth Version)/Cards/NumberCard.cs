using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Number Card")]
public class NumberCard : GenericCard
{
    public enum NumberClass {
        RED,
        BLUE,
        GREEN,
        YELLOW,
        BLACK,
        NONE
    };

    public NumberClass cardClass;
    public int value;
}
