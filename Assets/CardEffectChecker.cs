using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffectChecker : MonoBehaviour
{
    public static CardEffectChecker Instance;

    public CharacterInstance Player, Opponent;
    public int SimulatedPlayerValue, SimulatedOpponentValue;
    public int PlayerDrawnCardsNum, PlayerDiscardedCardsNum, OpponentDrawnCardsNum, OpponentDiscardedCardsNum;

    public List<DisplayCard> ConditionalStack;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetSimulatedValues(CharacterInstance playerOfCard, CharacterInstance opponentOfPlayer)
    {
        SimulatedPlayerValue = playerOfCard.currValue;
        SimulatedOpponentValue = opponentOfPlayer.currValue;
        Player = playerOfCard;
        Opponent = opponentOfPlayer;
        PlayerDiscardedCardsNum = 0;
        PlayerDrawnCardsNum = 0;
        OpponentDrawnCardsNum = 0;
        OpponentDiscardedCardsNum = 0;
    }

    public void ExecuteEffectStatement(EffectStatement statement, CharacterInstance playerOfCard, CharacterInstance opponentOfPlayer, bool simulated, bool initial)
    {
        if (initial)
        {
            ResetSimulatedValues(playerOfCard, opponentOfPlayer);
        }

        //Debug.Log("Execute statement: " + statement.name + " - " + statement.Condition.ToString());
        bool success = true;

        if(statement.ConditonalTarget != TargetCharacter.None)
        {
            CharacterInstance targetCharacter = statement.ConditonalTarget == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer;
            switch (statement.Condition)
            {
                case Condition.None:
                    break;
                case Condition.IfHasClass:
                    success = ConditionalHasClass(targetCharacter, statement.ConditionalNumberClass);
                    break;
                case Condition.IfHasCards:
                    success = ConditionalHasCard(targetCharacter, statement.ConditionalValues[0]);
                    break;
                case Condition.IfHasDuplicate:
                    success = ConditionalHasDuplicate(targetCharacter, statement.ConditionalCardType);
                    break;
                case Condition.IfDiscarded:
                    success = ConditionalDiscardFlag(targetCharacter);
                    break;
                case Condition.IfSwapped:
                    success = ConditionalSwapFlag(targetCharacter);
                    break;
                case Condition.IfFlipped:
                    success = ConditionalFlipFlag(targetCharacter);
                    break;
                case Condition.IfGaveACard:
                    success = ConditionalGaveACard(targetCharacter);
                    break;
                case Condition.IfHasQuantity:
                    success = ConditionalCardQuantity(targetCharacter, statement.ConditionalCardType, statement.ConditionalValues[0], statement.ConditionalValues[1]);
                    break;
                case Condition.IfGreaterThanOrEqualToTarget:
                    success = ConditionalGreaterThanOrEqualToTarget(targetCharacter);
                    break;
                case Condition.IfHasNegativeCard:
                    success = ConditionalHasNegativeCard(targetCharacter);
                    break;
                default:
                    break;
            }
        }

        if(success)
        {
            //Debug.Log("Successful statement: " + statement.name + " - " + statement.Condition.ToString());
            for (int i = 0; i < statement.EffectsOnSuccess.Count; i++)
            {
                if(simulated)
                {
                    SimulateEffect(statement.EffectsOnSuccess[i], statement.TargetsOnSuccess[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnSuccess[i], statement.ValuesOnSuccess[i]);
                }
                else
                {
                    DoEffect(statement.EffectsOnSuccess[i], statement.TargetsOnSuccess[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnSuccess[i], statement.ValuesOnSuccess[i], statement.SelectingOnSuccess[i]);
                }
            }
            if(statement.StatementOnSuccess != null)
            {
                ExecuteEffectStatement(statement.StatementOnSuccess, playerOfCard, opponentOfPlayer, simulated, false);
            }
        }
        else
        {
            //Debug.Log("Failed statement: " + statement.name + " - " + statement.Condition.ToString());
            for (int i = 0; i < statement.EffectsOnFail.Count; i++)
            {
                if (simulated)
                {
                    SimulateEffect(statement.EffectsOnFail[i], statement.TargetsOnFail[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnFail[i], statement.ValuesOnFail[i]);
                }
                else
                {
                    DoEffect(statement.EffectsOnFail[i], statement.TargetsOnFail[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnFail[i], statement.ValuesOnFail[i], statement.SelectingOnSuccess[i]);
                }
            }
            if (statement.StatementOnFail != null)
            {
                ExecuteEffectStatement(statement.StatementOnFail, playerOfCard, opponentOfPlayer, simulated, false);
            }
        }
    }

    void DoEffect(Effect effect, CharacterInstance targetCharacter, CardType cardType, int effectValue, CardSelecting cardSelectingSettings)
    {
        if(cardSelectingSettings != CardSelecting.Random)
        {
            CardSelectSettings newSettings;
            if (cardSelectingSettings == CardSelecting.Conditional)
            {
                newSettings = new CardSelectSettings(ConditionalStack, effectValue, targetCharacter, targetCharacter == CardGameManager.Instance.player);
            }
            else
            {
                newSettings = new CardSelectSettings(effectValue, cardType, effect, targetCharacter, targetCharacter == CardGameManager.Instance.player);
            }
            CardGameManager.Instance.cardSelectStack.Push(newSettings);
            CardGameManager.Instance.StartSelecting(CardGameManager.Instance.CurrentCharacter);

            switch (effect)
            {
                case Effect.None:
                    break;
                case Effect.AddValue:
                    targetCharacter.AddValue(effectValue);
                    break;
                case Effect.Draw:
                    if (cardType == CardType.Special)
                        CardGameManager.Instance.DrawSpecialCards(targetCharacter, effectValue);
                    else
                        CardGameManager.Instance.DrawNumberCards(targetCharacter, effectValue);
                    break;
                case Effect.Discard:
                    break;
                case Effect.Flip:
                    CardGameManager.Instance.SetFlip(true);
                    targetCharacter.ToggleForceFlip(true);
                    break;
                case Effect.Swap:
                    CardGameManager.Instance.SetSwap(true);
                    targetCharacter.ToggleForcedSwap(true);
                    break;
                case Effect.Give:
                    break;
                case Effect.Trade:
                    break;
                case Effect.Change:
                    break;
            }
        }
        else
        {
            switch (effect)
            {
                case Effect.None:
                    break;
                case Effect.AddValue:
                    targetCharacter.AddValue(effectValue);
                    break;
                case Effect.Draw:
                    if (cardType == CardType.Special)
                        CardGameManager.Instance.DrawSpecialCards(targetCharacter, effectValue);
                    else
                        CardGameManager.Instance.DrawNumberCards(targetCharacter, effectValue);
                    break;
                case Effect.Discard:
                    break;
                case Effect.Flip:
                    CardGameManager.Instance.SetFlip(true);
                    targetCharacter.ToggleForceFlip(true);
                    break;
                case Effect.Swap:
                    CardGameManager.Instance.SetSwap(true);
                    targetCharacter.ToggleForcedSwap(true);
                    break;
                case Effect.Give:
                    break;
                case Effect.Trade:
                    break;
                case Effect.Change:
                    break;
            }
        }
    }

    public void SimulateEffect(Effect effect, CharacterInstance targetCharacter, CardType cardType, int effectValue)
    {
        int newValue = targetCharacter == Player ? SimulatedPlayerValue : SimulatedOpponentValue;
        switch (effect)
        {
            case Effect.None:
                break;
            case Effect.AddValue:
                newValue += effectValue;
                break;
            case Effect.Draw:
                if(targetCharacter == Player)
                {
                    PlayerDrawnCardsNum++;
                }
                else
                {
                    OpponentDrawnCardsNum++;
                }
                break;
            case Effect.Discard:
                if (targetCharacter == Player)
                {
                    PlayerDiscardedCardsNum++;
                }
                else
                {
                    OpponentDiscardedCardsNum++;
                }
                break;
            case Effect.Swap:
                List<NumberCard> possibleSwaps = CardGameManager.Instance.numberDeck;
                int averageOutcome = 0;
                for (int j = 0; j < possibleSwaps.Count; j++)
                {
                    averageOutcome += possibleSwaps[j].value;
                }
                averageOutcome /= possibleSwaps.Count;
                newValue = averageOutcome;
                break;
            case Effect.Flip:
                newValue -= (effectValue * 2);
                break;
        }
        if(targetCharacter == Player)
        {
            SimulatedPlayerValue = newValue;
        }
        else
        {
            SimulatedOpponentValue = newValue;
        }
    }

                /*TARGET -> color
                STORE INT -> count*/
     public bool ConditionalHasClass(CharacterInstance target, NumberClass color)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.cardClass == color)
            {
                tmpStack.Add(d);
            }
        }
        return tmpStack.Count > 0;
    }
    /*TARGET -> value
    STORE INT -> count*/
    public bool ConditionalHasCard(CharacterInstance target, int value)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.value == value)
            {
                tmpStack.Add(d);
            }
        }
        ConditionalStack = tmpStack;
        return ConditionalStack.Count > 0;
    }

    /*TARGET -> -1 (N/A)*/
    public bool ConditionalHasDuplicate(CharacterInstance target, CardType type)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        if (type == CardType.Special)
        {
            for (int i = 0; i < target.specialDisplayHand.Count - 1; i++)
            {
                for (int j = i + 1; j < target.specialDisplayHand.Count; j++)
                {
                    if (target.specialDisplayHand[i].name == target.specialDisplayHand[j].name)
                    {
                        if(!tmpStack.Contains(target.specialDisplayHand[i]))
                        {
                            tmpStack.Add(target.specialDisplayHand[i]);
                        }

                        if (!tmpStack.Contains(target.specialDisplayHand[j]))
                        {
                            tmpStack.Add(target.specialDisplayHand[j]);
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < target.numberDisplayHand.Count - 1; i++)
            {
                for (int j = i + 1; j < target.numberDisplayHand.Count; j++)
                {
                    if (target.numberDisplayHand[i].baseCard.name == target.numberDisplayHand[j].baseCard.name)
                    {
                        if (!tmpStack.Contains(target.numberDisplayHand[i]))
                        {
                            tmpStack.Add(target.numberDisplayHand[i]);
                        }

                        if (!tmpStack.Contains(target.numberDisplayHand[j]))
                        {
                            tmpStack.Add(target.numberDisplayHand[j]);
                        }
                    }
                }
            }
        }
        ConditionalStack = tmpStack;
        return ConditionalStack.Count > 0;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalDiscardFlag(CharacterInstance target)
    {
        //NYI
        return target.DiscardedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalSwapFlag(CharacterInstance target)
    {
        //NYI
        return target.SwappedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalFlipFlag(CharacterInstance target)
    {
        //NYI
        return target.FlippedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalGaveACard(CharacterInstance target)
    {
        //NYI
        return target.GaveThisTurn;
    }
    /*TARGET -> min
    STORE INT -> max*/
    public bool ConditionalCardQuantity(CharacterInstance target, CardType type, int min, int max)
    {
        if (type == CardType.Special)
        {
            return (target.specialDisplayHand.Count >= min && target.specialDisplayHand.Count <= max);
        }
        else
        {
            return (target.numberDisplayHand.Count >= min && target.numberDisplayHand.Count <= max);
        }
    }

    /* TARGET -> -1 (N/A) */
    public bool ConditionalGreaterThanOrEqualToTarget(CharacterInstance target)
    {
        return target.currValue >= CardGameManager.Instance.targetValue;
    }

    public bool ConditionalHasNegativeCard(CharacterInstance target)
    {
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.value < 0)
            {
                return true;
            }
        }
        return false;
    }

}
