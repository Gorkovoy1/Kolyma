using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardEffectChecker : MonoBehaviour
{
    public static CardEffectChecker Instance;

    public CharacterInstance Player, Opponent;
    public int SimulatedPlayerValue, SimulatedOpponentValue;
    public int PlayerDrawnCardsNum, PlayerDiscardedCardsNum, OpponentDrawnCardsNum, OpponentDiscardedCardsNum;

    private EffectStatement currentStatement, nextStatement;
    private CharacterInstance currentPlayerOfCard, currentOpponentOfPlayer;

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

    public void DoNextStatement()
    {
        if(nextStatement != null)
        {
            Debug.Log("Do next statement!");
            ExecuteEffectStatement(nextStatement, currentPlayerOfCard, currentOpponentOfPlayer, false, false);
        }
    }

    public void ExecuteEffectStatement(EffectStatement statement, CharacterInstance playerOfCard, CharacterInstance opponentOfPlayer, bool simulated, bool initial)
    {
        if (initial)
        {
            ResetSimulatedValues(playerOfCard, opponentOfPlayer);
            nextStatement = null;
        }

        //Debug.Log("Execute statement: " + statement.name + " - " + statement.Condition.ToString());
        bool success = true;
        ConditionalStack = new List<DisplayCard>();

        currentStatement = statement;
        currentPlayerOfCard = playerOfCard;
        currentOpponentOfPlayer = opponentOfPlayer;

        if (statement.ConditonalTarget == TargetCharacter.All)
        {
            switch (statement.Condition)
            {
                case Condition.None:
                    break;
                case Condition.IfHasClass:
                    success = ConditionalHasClass(playerOfCard, statement.ConditionalNumberClass) || ConditionalHasClass(opponentOfPlayer, statement.ConditionalNumberClass);
                    break;
                case Condition.IfHasCards:
                    success = ConditionalHasCard(playerOfCard, statement.ConditionalValue) || ConditionalHasCard(opponentOfPlayer, statement.ConditionalValue);
                    break;
                case Condition.IfHasDuplicate:
                    success = ConditionalHasDuplicate(playerOfCard, statement.ConditionalCardType) || ConditionalHasDuplicate(opponentOfPlayer, statement.ConditionalCardType);
                    break;
                case Condition.IfDiscarded:
                    success = ConditionalDiscardFlag(playerOfCard) || ConditionalDiscardFlag(opponentOfPlayer);
                    break;
                case Condition.IfSwapped:
                    success = ConditionalSwapFlag(playerOfCard) || ConditionalSwapFlag(opponentOfPlayer);
                    break;
                case Condition.IfFlipped:
                    success = ConditionalFlipFlag(playerOfCard) || ConditionalFlipFlag(opponentOfPlayer);
                    break;
                case Condition.IfGaveACard:
                    success = ConditionalGaveACard(playerOfCard) || ConditionalGaveACard(opponentOfPlayer);
                    break;
                case Condition.IfHasQuantity:
                    success = ConditionalAtLeastCardQuantity(playerOfCard, statement.ConditionalCardType, statement.ConditionalValue) || ConditionalAtLeastCardQuantity(opponentOfPlayer, statement.ConditionalCardType, statement.ConditionalValue);
                    break;
                case Condition.IfGreaterThanOrEqualToTarget:
                    success = ConditionalGreaterThanOrEqualToTarget(playerOfCard) || ConditionalGreaterThanOrEqualToTarget(opponentOfPlayer);
                    break;
                case Condition.IfHasNegativeCard:
                    success = ConditionalHasNegativeCard(playerOfCard) || ConditionalHasNegativeCard(opponentOfPlayer);
                    break;
                case Condition.IfReceivedACard:
                    success = ConditionalReceivedCard(playerOfCard) || ConditionalReceivedCard(opponentOfPlayer);
                    break;
                case Condition.IfNewlyDrawnCardsHasClass:
                    success = ConditianalNewlyDrawnCardHasClass(playerOfCard, statement.ConditionalNumberClass) || ConditianalNewlyDrawnCardHasClass(opponentOfPlayer, statement.ConditionalNumberClass);
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
                case Condition.IfReceivedACard:
                    success = ConditionalReceivedCard(targetCharacter);
                    break;
                case Condition.IfNewlyDrawnCardsHasClass:
                    Debug.Log("HELLO!");
                    success = ConditianalNewlyDrawnCardHasClass(targetCharacter, statement.ConditionalNumberClass);
                    break;
                default:
                    break;
            }
        }

        if(success)
        {
            Debug.Log("Successful statement: " + statement.name + " - " + statement.Condition.ToString());

            nextStatement = statement.StatementOnSuccess;

            if (simulated)
            {
                SimulateEffect(statement.EffectOnSuccess, statement.TargetOnSuccess == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypeOnSuccess, statement.NumberOfCardsOnSuccess, statement.MiscValueOnSuccess);
            }
            else
            {
                DoEffect(statement.EffectOnSuccess, statement.SelectingCharacterOnSuccess, statement.TargetOnSuccess, statement.CardTypeOnSuccess, statement.NumberOfCardsOnSuccess, statement.MiscValueOnSuccess, statement.SelectingOnSuccess, statement.CardClassOnSuccess, statement.CardOptionsOnSuccess, statement.QuantifierOnSuccess);
            }

            //if(statement.StatementOnSuccess != null)
            //{
                //ExecuteEffectStatement(statement.StatementOnSuccess, playerOfCard, opponentOfPlayer, simulated, false);
            //}
        }
        else
        {
            Debug.Log("Failed statement: " + statement.name + " - " + statement.Condition.ToString());

            nextStatement = statement.StatementOnFail;

            if (simulated)
            {
                SimulateEffect(statement.EffectOnFail, statement.TargetOnFail == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypeOnFail, statement.NumberOfCardsOnFail, statement.MiscValueOnFail);
            }
            else
            {
                DoEffect(statement.EffectOnFail, statement.SelectingCharacterOnFail, statement.TargetOnFail, statement.CardTypeOnFail, statement.NumberOfCardsOnFail, statement.MiscValueOnFail, statement.SelectingOnFail, statement.CardClassOnFail, statement.CardOptionsOnFail, statement.QuantifierOnFail);
            }

            /*if (statement.StatementOnFail != null)
            {
                ExecuteEffectStatement(statement.StatementOnFail, playerOfCard, opponentOfPlayer, simulated, false);
            }*/
        }
    }

    public void DoEffect(Effect effect, TargetCharacter selectingCharacter, TargetCharacter targetCharacter, CardType cardType, int numberOfCards, int miscValue, CardSelecting cardSelectingSettings, NumberClass numberClass, List<SpecialDeckCard> cardOptions, NumberOfCardsQuantifier quantifier)
    {
        CharacterInstance selector;
        if (selectingCharacter != TargetCharacter.OpponentOfPlayer)
        {
            selector = CardGameManager.Instance.CurrentCharacter;
        }
        else
        {
            selector = CardGameManager.Instance.WaitingCharacter;
        }
        
        if(cardSelectingSettings == CardSelecting.SelectFromDiscard)
        {
            CardOptionsSelectionUI.Instance.FillInCards(CardGameManager.Instance.discardPile, selector, numberOfCards, effect, quantifier);
        }
        else if (cardOptions.Count > 0)
        {
            CardOptionsSelectionUI.Instance.FillInCards(cardOptions, selector, numberOfCards, effect, quantifier);
        }
        else if (cardSelectingSettings == CardSelecting.Selected || cardSelectingSettings == CardSelecting.Conditional)
        {
            CardSelectSettings newSettings;
            if (cardSelectingSettings == CardSelecting.Conditional)
            {
                newSettings = new CardSelectSettings(ConditionalStack, numberOfCards, selector, targetCharacter, selector == CardGameManager.Instance.player, miscValue, effect, quantifier);
            }
            else
            {
                newSettings = new CardSelectSettings(numberOfCards, cardType, effect, selector, targetCharacter, selector == CardGameManager.Instance.player, miscValue, numberClass, quantifier);
            }
            CardGameManager.Instance.cardSelectStack.Push(newSettings);
            CardGameManager.Instance.StartSelecting();
        }
        else if (cardSelectingSettings == CardSelecting.AllConditional)
        {
            switch (effect)
            {
                case Effect.Discard:
                    foreach(DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.DiscardCard(card);
                    }
                    break;
                case Effect.Flip:
                    foreach (DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.FlipCard(card);
                    }
                    break;
                case Effect.Swap:
                    foreach (DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.SwapCard(card);
                    }
                    break;
                case Effect.Give:
                    foreach (DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.GiveCard(card);
                    }
                    break;
                case Effect.Change:
                    foreach (DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.ChangeCard(card, miscValue);
                    }
                    break;
                case Effect.Steal:
                    foreach (DisplayCard card in ConditionalStack)
                    {
                        CardGameManager.Instance.StealCard(card);
                    }
                break;
                DoNextStatement();
            }
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
            DoNextStatement();
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
                    Debug.Log("Card Effect Draw");
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
                case Effect.Change:
                    break;
                case Effect.DrawPerDiscarded:
                    CardGameManager.Instance.DrawSpecialCards(target, target.NumberOfNewlyDiscardCards);
                    break;
            }
            DoNextStatement();
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
        List<DisplayCard> swappedCards = new List<DisplayCard>();
        foreach(DisplayCard card in target.numberDisplayHand)
        {
            if(card.SwappedThisTurn)
            {
                swappedCards.Add(card);
            }
        }
        ConditionalStack.AddRange(swappedCards);
        return ConditionalStack.Count > 0;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalFlipFlag(CharacterInstance target)
    {
        List<DisplayCard> flippedCards = new List<DisplayCard>();
        foreach (DisplayCard card in target.numberDisplayHand)
        {
            if (card.FlippedThisTurn)
            {
                flippedCards.Add(card);
            }
        }
        ConditionalStack.AddRange(flippedCards);
        return ConditionalStack.Count > 0;
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

    public bool ConditionalReceivedCard(CharacterInstance target)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        foreach (DisplayCard card in target.numberDisplayHand)
        {
            if(card.Given)
            {
                tmpStack.Add(card);
            }
        }
        ConditionalStack.AddRange(tmpStack);
        return ConditionalStack.Count > 0;
    }

    public bool ConditianalNewlyDrawnCardHasClass(CharacterInstance target, NumberClass color)
    {
        List<DisplayCard> tmpStack = new List<DisplayCard>();
        Debug.Log("Checking newly drawn!");
        foreach (DisplayCard d in target.NewlyDrawnNumberCards)
        {
            Debug.Log("Checking card: " + d.baseCard.name);
            NumberCard c = (NumberCard)d.baseCard;
            if (c.cardClass == color)
            {
                Debug.Log("Card " + d.baseCard.name + " is of class " + color.ToString());
                tmpStack.Add(d);
            }
        }
        ConditionalStack.AddRange(tmpStack);
        return ConditionalStack.Count > 0;
    }

}
