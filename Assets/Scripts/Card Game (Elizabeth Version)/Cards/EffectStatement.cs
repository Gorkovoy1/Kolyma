using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Condition
{
    None, IfHasClass, IfHasCards, IfHasDuplicate, IfDiscarded, IfSwapped, IfFlipped, IfGaveACard, IfHasQuantity, IfGreaterThanOrEqualToTarget, IfHasNegativeCard, IfReceivedACard, IfNewlyDrawnCardsHasClass
}


//Current Player == Player of Card
//Waiting Player == Other Player who is playing cards
public enum TargetCharacter
{
    None, PlayerOfCard, OpponentOfPlayer, Anyone, All
}

public enum Effect
{
    None, AddValue, Draw, Discard, Swap, Flip, Duplicate, Change, Give, Steal, PlayCard, TradePart1, TradePart2, GiveCopy, DrawFromDiscard, DrawPerDiscarded, BuildDeck
}

public enum CardSelecting
{
    None, Random, Selected, Conditional, Given, All, AllConditional, SelectFromDiscard
}

public enum CardType
{
    None, Number, Special, Any, PositiveNumber, NegativeNumber
}

public enum NumberOfCardsQuantifier
{
    EqualTo, LessThanOrEqualTo
}

[CreateAssetMenu(fileName = "New Effect Statement")]
public class EffectStatement : ScriptableObject
{
    [Header("C O N D I T I O N A L")]
    public Condition Condition;
    public TargetCharacter ConditonalTarget;
    public CardType ConditionalCardType;
    public int ConditionalValue;
    public int SecondaryConditionalValue;
    public NumberClass ConditionalNumberClass;

    [Header("S U C C E S S")]
    public EffectStatement StatementOnSuccess;
    public Effect EffectOnSuccess;
    public TargetCharacter TargetOnSuccess;
    public CardType CardTypeOnSuccess;
    public NumberClass CardClassOnSuccess;
    public CardSelecting SelectingOnSuccess;
    public TargetCharacter SelectingCharacterOnSuccess;
    public int NumberOfCardsOnSuccess;
    public NumberOfCardsQuantifier QuantifierOnSuccess;
    //Values vary based on effect
    public int MiscValueOnSuccess;
    public List<SpecialDeckCard> CardOptionsOnSuccess;

    [Header("F A I L")]
    public EffectStatement StatementOnFail;
    public Effect EffectOnFail;
    public TargetCharacter TargetOnFail;
    public CardType CardTypeOnFail;
    public NumberClass CardClassOnFail;
    public CardSelecting SelectingOnFail;
    public TargetCharacter SelectingCharacterOnFail;
    public int NumberOfCardsOnFail;
    public NumberOfCardsQuantifier QuantifierOnFail;
    //Values vary based on effect
    public int MiscValueOnFail;
    public List<SpecialDeckCard> CardOptionsOnFail;

}
