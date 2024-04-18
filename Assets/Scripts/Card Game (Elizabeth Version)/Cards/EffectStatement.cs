using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Condition
{
    None, IfHasClass, IfHasCards, IfHasDuplicate, IfDiscarded, IfSwapped, IfFlipped, IfGaveACard, IfHasQuantity, IfGreaterThanOrEqualToTarget, IfHasNegativeCard
}

public enum TargetCharacter
{
    None, PlayerOfCard, OpponentOfPlayer
}

public enum Effect
{
    None, AddValue, Draw, Discard, Swap, Flip
}

public enum CardType
{
    None, Number, Special
}


[CreateAssetMenu(fileName = "New Effect Statement")]
public class EffectStatement : ScriptableObject
{
    [Header("C O N D I T I O N A L")]
    public Condition Condition;
    public TargetCharacter ConditonalTarget;
    public CardType ConditionalCardType;
    public List<int> ConditionalValues;
    public NumberCard.NumberClass ConditionalNumberClass;

    [Header("S U C C E S S")]
    public EffectStatement StatementOnSuccess;
    public List<Effect> EffectsOnSuccess;
    public List<TargetCharacter> TargetsOnSuccess;
    public List<CardType> CardTypesOnSuccess;
    public List<int> ValuesOnSuccess;

    [Header("F A I L")]
    public EffectStatement StatementOnFail;
    public List<Effect> EffectsOnFail;
    public List<TargetCharacter> TargetsOnFail;
    public List<CardType> CardTypesOnFail;
    public List<int> ValuesOnFail;

}
