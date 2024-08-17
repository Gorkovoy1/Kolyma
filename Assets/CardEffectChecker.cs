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
        ConditionalStack = new List<DisplayCard>();

        if (statement.ConditonalTarget == TargetCharacter.All)
        {
            switch (statement.Condition)
            {
                case Condition.None:
                    break;
                case Condition.IfHasClass:
                    success = ConditionalHasClass(playerOfCard, statement.ConditionalNumberClass);
                    success = ConditionalHasClass(opponentOfPlayer, statement.ConditionalNumberClass);
                    break;
                case Condition.IfHasCards:
                    success = ConditionalHasCard(playerOfCard, statement.ConditionalValue);
                    success = ConditionalHasCard(opponentOfPlayer, statement.ConditionalValue);
                    break;
                case Condition.IfHasDuplicate:
                    success = ConditionalHasDuplicate(playerOfCard, statement.ConditionalCardType);
                    success = ConditionalHasDuplicate(opponentOfPlayer, statement.ConditionalCardType);
                    break;
                case Condition.IfDiscarded:
                    success = ConditionalDiscardFlag(playerOfCard);
                    success = ConditionalDiscardFlag(opponentOfPlayer);
                    break;
                case Condition.IfSwapped:
                    success = ConditionalSwapFlag(playerOfCard);
                    success = ConditionalSwapFlag(opponentOfPlayer);
                    break;
                case Condition.IfFlipped:
                    success = ConditionalFlipFlag(playerOfCard);
                    success = ConditionalFlipFlag(opponentOfPlayer);
                    break;
                case Condition.IfGaveACard:
                    success = ConditionalGaveACard(playerOfCard);
                    success = ConditionalGaveACard(opponentOfPlayer);
                    break;
                case Condition.IfHasQuantity:
                    success = ConditionalAtLeastCardQuantity(playerOfCard, statement.ConditionalCardType, statement.ConditionalValue);
                    success = ConditionalAtLeastCardQuantity(opponentOfPlayer, statement.ConditionalCardType, statement.ConditionalValue);
                    break;
                case Condition.IfGreaterThanOrEqualToTarget:
                    success = ConditionalGreaterThanOrEqualToTarget(playerOfCard);
                    success = ConditionalGreaterThanOrEqualToTarget(opponentOfPlayer);
                    break;
                case Condition.IfHasNegativeCard:
                    success = ConditionalHasNegativeCard(playerOfCard);
                    success = ConditionalHasNegativeCard(opponentOfPlayer);
                    break;
                default:
                    break;
            }
        }
        else if (statement.ConditonalTarget != TargetCharacter.None)
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
                    success = ConditionalHasCard(targetCharacter, statement.ConditionalValue);
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
                    success = ConditionalAtLeastCardQuantity(targetCharacter, statement.ConditionalCardType, statement.ConditionalValue);
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
            //for (int i = 0; i < statement.EffectOnSuccess.Count; i++)
            //{
                if(simulated)
                {
                    SimulateEffect(statement.EffectOnSuccess, statement.TargetOnSuccess == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypeOnSuccess, statement.NumberOfCardsOnSuccess, statement.MiscValueOnSuccess);
                }
                else
                {
                    DoEffect(statement.EffectOnSuccess, statement.SelectingCharacterOnSuccess, statement.TargetOnSuccess, statement.CardTypeOnSuccess, statement.NumberOfCardsOnSuccess, statement.MiscValueOnSuccess, statement.SelectingOnSuccess, statement.CardClassOnSuccess);
                }
            //}
            if(statement.StatementOnSuccess != null)
            {
                ExecuteEffectStatement(statement.StatementOnSuccess, playerOfCard, opponentOfPlayer, simulated, false);
            }
        }
        else
        {
            //Debug.Log("Failed statement: " + statement.name + " - " + statement.Condition.ToString());
            //for (int i = 0; i < statement.EffectsOnFail.Count; i++)
            //{
                if (simulated)
                {
                    SimulateEffect(statement.EffectOnFail, statement.TargetOnFail == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypeOnFail, statement.NumberOfCardsOnFail, statement.MiscValueOnFail);
                }
                else
                {
                    DoEffect(statement.EffectOnFail, statement.SelectingCharacterOnFail, statement.TargetOnFail, statement.CardTypeOnFail, statement.NumberOfCardsOnFail, statement.MiscValueOnFail, statement.SelectingOnFail, statement.CardClassOnFail);
                }
            //}
            if (statement.StatementOnFail != null)
            {
                ExecuteEffectStatement(statement.StatementOnFail, playerOfCard, opponentOfPlayer, simulated, false);
            }
        }
    }

    public void DoEffect(Effect effect, TargetCharacter selectingCharacter, TargetCharacter targetCharacter, CardType cardType, int numberOfCards, int miscValue, CardSelecting cardSelectingSettings, NumberClass numberClass)
    {
        if(cardSelectingSettings != CardSelecting.Random && cardSelectingSettings != CardSelecting.None)
        {
            CardSelectSettings newSettings;
            CharacterInstance selector;
            if(selectingCharacter == TargetCharacter.PlayerOfCard)
            {
                selector = CardGameManager.Instance.CurrentCharacter;
            }
            else
            {
                selector = CardGameManager.Instance.WaitingCharacter;
            }

            if (cardSelectingSettings == CardSelecting.Conditional)
            {
                newSettings = new CardSelectSettings(ConditionalStack, numberOfCards, selector, targetCharacter, selector == CardGameManager.Instance.player, miscValue, effect);
            }
            else
            {
                newSettings = new CardSelectSettings(numberOfCards, cardType, effect, selector, targetCharacter, selector == CardGameManager.Instance.player, miscValue, numberClass);
            }
            CardGameManager.Instance.cardSelectStack.Push(newSettings);
            CardGameManager.Instance.StartSelecting();
        }
        else if(cardSelectingSettings == CardSelecting.Random)
        {

            CharacterInstance target;
            if (targetCharacter == TargetCharacter.PlayerOfCard)
            {
                target = CardGameManager.Instance.CurrentCharacter;
            }
            else
            {
                target = CardGameManager.Instance.WaitingCharacter;
            }
            switch (effect)
            {
                case Effect.None:
                    break;
                case Effect.AddValue:
                    Debug.Log("Card Effect Add Value");
                    target.AddValue(miscValue);
                    break;
                case Effect.Draw:
                    if (cardType == CardType.Special)
                        CardGameManager.Instance.DrawSpecialCards(target, numberOfCards);
                    else
                        CardGameManager.Instance.DrawNumberCards(target, numberOfCards);
                    break;
                case Effect.Discard:
                    break;
                case Effect.Flip:
                    CardGameManager.Instance.SetFlip(true);
                    target.ToggleForceFlip(true);
                    break;
                case Effect.Swap:
                    CardGameManager.Instance.SetSwap(true);
                    target.ToggleForcedSwap(true);
                    break;
                case Effect.Give:
                    break;
                case Effect.Trade:
                    break;
                case Effect.Change:
                    break;
                case Effect.Steal:
                    DisplayCard stolenCard  = CardSelectionHandler.Instance.ChooseRandomCard(cardType, target);
                    CharacterInstance oppositeCharacter = stolenCard.owner == CardGameManager.Instance.player ? CardGameManager.Instance.opponent : CardGameManager.Instance.player;
                    if (stolenCard.baseCard is NumberCard)
                    {
                        oppositeCharacter.AddValue(stolenCard.value);
                    }
                    else
                    {
                        CardGameManager.Instance.AddSpecialCard((SpecialDeckCard)stolenCard.baseCard, oppositeCharacter);
                    }
                    CardGameManager.Instance.DiscardCard(stolenCard);
                    break;
            }
        }
        else
        {
            CharacterInstance target;
            if (targetCharacter == TargetCharacter.PlayerOfCard)
            {
                target = CardGameManager.Instance.CurrentCharacter;
            }
            else
            {
                target = CardGameManager.Instance.WaitingCharacter;
            }

            switch (effect)
            {
                case Effect.None:
                    break;
                case Effect.AddValue:
                    Debug.Log("Card Effect Add Value");
                    target.AddValue(miscValue);
                    break;
                case Effect.Draw:
                    if (cardType == CardType.Special)
                        CardGameManager.Instance.DrawSpecialCards(target, numberOfCards);
                    else
                        CardGameManager.Instance.DrawNumberCards(target, numberOfCards);
                    break;
                case Effect.Discard:
                    break;
                case Effect.Flip:
                    CardGameManager.Instance.SetFlip(true);
                    target.ToggleForceFlip(true);
                    break;
                case Effect.Swap:
                    CardGameManager.Instance.SetSwap(true);
                    target.ToggleForcedSwap(true);
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

    public void SimulateEffect(Effect effect, CharacterInstance targetCharacter, CardType cardType, int numberOfCards, int miscValue)
    {
        int newValue = targetCharacter == Player ? SimulatedPlayerValue : SimulatedOpponentValue;
        switch (effect)
        {
            case Effect.None:
                break;
            case Effect.AddValue:
                newValue += miscValue;
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
                newValue -= (miscValue * 2);
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
        ConditionalStack.AddRange(tmpStack);
        return ConditionalStack.Count > 0;
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
        ConditionalStack.AddRange(tmpStack);
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
        ConditionalStack.AddRange(tmpStack);
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
    public bool ConditionalAtLeastCardQuantity(CharacterInstance target, CardType type, int min)
    {
        if (type == CardType.Special)
        {
            return (target.specialDisplayHand.Count >= min);
        }
        else
        {
            return (target.numberDisplayHand.Count >= min);
        }
    }

    /* TARGET -> -1 (N/A) */
    public bool ConditionalGreaterThanOrEqualToTarget(CharacterInstance target)
    {
        return target.currValue >= CardGameManager.Instance.targetValue;
    }

    public bool ConditionalHasNegativeCard(CharacterInstance target)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.value < 0)
            {
                tmpStack.Add(d);
            }
        }
        ConditionalStack.AddRange(tmpStack);
        return ConditionalStack.Count > 0;
    }

}
