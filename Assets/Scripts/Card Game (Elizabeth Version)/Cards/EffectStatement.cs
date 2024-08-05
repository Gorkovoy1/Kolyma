using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Condition
{
    None, IfHasClass, IfHasCards, IfHasDuplicate, IfDiscarded, IfSwapped, IfFlipped, IfGaveACard, IfHasQuantity, IfGreaterThanOrEqualToTarget, IfHasNegativeCard
}

public enum TargetCharacter
{
    None, PlayerOfCard, OpponentOfPlayer, Anyone, All
}

public enum Effect
{
    None, AddValue, Draw, Discard, Swap, Flip, Duplicate, Change, Give, Trade
}

public enum CardSelecting
{
    None, Random, Selected, Conditional, Given, All
}

public enum CardType
{
    None, Number, Special, Any, PositiveNumber, NegativeNumber
}


[CreateAssetMenu(fileName = "New Effect Statement")]
public class EffectStatement : ScriptableObject
{
    [Header("C O N D I T I O N A L")]
    public Condition Condition;
    public TargetCharacter ConditonalTarget;
    public CardType ConditionalCardType;
    public List<int> ConditionalValues;
    public NumberClass ConditionalNumberClass;

    [Header("S U C C E S S")]
    public EffectStatement StatementOnSuccess;
    public List<Effect> EffectsOnSuccess;
    public List<TargetCharacter> TargetsOnSuccess;
    public List<CardType> CardTypesOnSuccess;
    public List<NumberClass> CardClassOnSuccess;
    public List<CardSelecting> SelectingOnSuccess;
    public List<TargetCharacter> SelectingCharacter;
    //Values vary based on effect
    public List<int> ValuesOnSuccess;
    public List<int> SecondaryValuesOnSuccess;

    [Header("F A I L")]
    public EffectStatement StatementOnFail;
    public List<Effect> EffectsOnFail;
    public List<TargetCharacter> TargetsOnFail;
    public List<CardType> CardTypesOnFail;
    public List<NumberClass> CardClassOnFail;
    public List<CardSelecting> SelectingOnFail;
    public List<int> ValuesOnFail;
    public List<int> SecondaryValuesOnFail;

}
