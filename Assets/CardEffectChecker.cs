using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectChecker : MonoBehaviour
{
    public static CardEffectChecker Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ExecuteEffectStatement(EffectStatement statement, CharacterInstance playerOfCard, CharacterInstance opponentOfPlayer)
    {
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
                DoEffect(statement.EffectsOnSuccess[i], statement.TargetsOnSuccess[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnSuccess[i], statement.ValuesOnSuccess[i]);
            }
            if(statement.StatementOnSuccess != null)
            {
                ExecuteEffectStatement(statement.StatementOnSuccess, playerOfCard, opponentOfPlayer);
            }
        }
        else
        {
            //Debug.Log("Failed statement: " + statement.name + " - " + statement.Condition.ToString());
            for (int i = 0; i < statement.EffectsOnFail.Count; i++)
            {
                DoEffect(statement.EffectsOnFail[i], statement.TargetsOnFail[i] == TargetCharacter.PlayerOfCard ? playerOfCard : opponentOfPlayer, statement.CardTypesOnFail[i], statement.ValuesOnFail[i]);
            }
            if (statement.StatementOnFail != null)
            {
                ExecuteEffectStatement(statement.StatementOnFail, playerOfCard, opponentOfPlayer);
            }
        }
    }

    void DoEffect(Effect effect, CharacterInstance targetCharacter, CardType cardType, int effectValue)
    {
        switch (effect)
        {
            case Effect.None:
                break;
            case Effect.AddValue:
                targetCharacter.AddValue(effectValue);
                break;
            case Effect.Draw:
                if(cardType == CardType.Special)
                    CardGameManager.Instance.DrawSpecialCards(targetCharacter, effectValue);
                else
                    CardGameManager.Instance.DrawNumberCards(targetCharacter, effectValue);
                break;
            case Effect.Discard:
                CardSelectSettings newSettings = new CardSelectSettings(effectValue, cardType, effect, targetCharacter, true);
                CardGameManager.Instance.cardSelectStack.Push(newSettings);
                CardGameManager.Instance.StartSelecting(CardGameManager.Instance.CurrentCharacter);
                break;
            case Effect.Swap:
                CardGameManager.Instance.SetFlip(true);
                targetCharacter.ToggleForceFlip(true);
                break;
            case Effect.Flip:
                CardGameManager.Instance.SetSwap(true);
                targetCharacter.ToggleForcedSwap(true);
                break;
        }
    }

    //Very PROTOTYPE version of the card decision tree
    //public void ExecuteCardEffect(List<SpecialKeyword> keywords, List<int> values, CharacterInstance playerTarget, CharacterInstance opponentTarget)
    /*public void ExecuteEffectStatement(EffectStatement statement, CharacterInstance playerTarget, CharacterInstance opponentTarget)
    {
        List<SpecialKeyword> currKeys = new List<SpecialKeyword>();
        List<int> currValues = new List<int>();
        List<SpecialKeyword> truncatedKeys = new List<SpecialKeyword>();
        List<int> truncatedValues = new List<int>();
        int skippedValues = 0;
        for (int i = 0; i < keywords.Count; i++)
        {
            if (keywords[i] == SpecialKeyword.END_COMMAND)
            {
                for (int j = i + 1; j < keywords.Count; j++)
                {
                    truncatedKeys.Add(keywords[j]);
                }
                for (int k = i - skippedValues; k < values.Count; k++)
                {
                    truncatedValues.Add(values[k]);
                }
                ExecuteCardEffect(truncatedKeys, truncatedValues, playerTarget, opponentTarget);
                break;
            }
            else if (keywords[i] == SpecialKeyword.TARGET_PLAYER || keywords[i] == SpecialKeyword.TARGET_OPPONENT || keywords[i] == SpecialKeyword.CON_STORE_ADDITIONAL_INTEGER)
            {
                currKeys.Add(keywords[i]);
                currValues.Add(values[i - skippedValues]);
            }
            else
            {
                currKeys.Add(keywords[i]);
                skippedValues += 1;
            }
        }
        keywords = currKeys;
        values = currValues;
        /*Debug.Log("Keywords: " );
        foreach(SpecialKeyword k in currKeys) {
            Debug.Log(k);
        }
        Debug.Log("Values: " );
        foreach(int v in currValues) {
            Debug.Log(v);
        }*/
                /*SpecialKeyword effectType = keywords[0];

                switch (effectType)
                {
                    case SpecialKeyword.EFFECT_NONE:
                        Debug.Log("This card has no effect");
                        break;
                    case SpecialKeyword.EFFECT_ADDVALUE:
                        /*EFFECT_ADDVALUE ANTICPATED SYNTAX:
                        keywords[i] = target to add value to
                        values[i - 1] = amount to add to keywords[i]
                        */
                /*for (int i = 1; i < keywords.Count; i++)
                {
                    if (keywords[i] == SpecialKeyword.TARGET_PLAYER)
                    {
                        playerTarget.currValue += values[i - 1];
                    }
                    else
                    {
                        opponentTarget.currValue += values[i - 1];
                    }
                }
                break;
            case SpecialKeyword.EFFECT_DRAW:
                /* EFFECT_DRAWCARD ANTICIPATED SYNTAX
                keywords[last item] = type of card to draw
                keywords[i] -> keywords[2nd to last item] = target to draw to
                values[i-1] = # cards to draw*/
                /*SpecialKeyword cardType = keywords[keywords.Count - 1];

                for (int i = 1; i < keywords.Count - 1; i++)
                {
                    if (keywords[i] == SpecialKeyword.TARGET_PLAYER && cardType == SpecialKeyword.TYPE_SPECIAL)
                    {
                        CardGameManager.Instance.DrawSpecialCards(playerTarget, values[i - 1]);
                    }
                    else if (keywords[i] == SpecialKeyword.TARGET_PLAYER && cardType == SpecialKeyword.TYPE_NUMBER)
                    {
                        Debug.Log("drawing number cards NYI");
                    }
                    else if (keywords[i] == SpecialKeyword.TARGET_OPPONENT && cardType == SpecialKeyword.TYPE_NUMBER)
                    {
                        Debug.Log("drawing number cards NYI");
                    }
                    else if (keywords[i] == SpecialKeyword.TARGET_OPPONENT && cardType == SpecialKeyword.TYPE_SPECIAL)
                    {
                        CardGameManager.Instance.DrawSpecialCards(opponentTarget, values[i - 1]);
                    }
                }

                break;
            case SpecialKeyword.EFFECT_DISCARD:
                /*EFFECT_DISCARD ANTICIPATED SYNTAX
                keywords[last item] = type of card to discard
                keywords[1 -> 2nd to last item] = the discard target
                values[0] = # to discard.
                
                done in a coroutine and selectcards state to allow everyone to select their cards which may take multiple framesit is presently set up 
                only to work if there is only one card type that needs discarding*/
                /*for (int i = 1; i < keywords.Count - 1; i++)
                {
                    if (keywords[i] == SpecialKeyword.TARGET_PLAYER)
                    {
                        CardSelectSettings newSettings = new CardSelectSettings(values[i - 1], keywords[keywords.Count - 1], keywords[0], playerTarget == CardGameManager.Instance.player);
                        CardGameManager.Instance.cardSelectStack.Push(newSettings);
                    }
                    else
                    {
                        CardSelectSettings newSettings = new CardSelectSettings(values[i - 1], keywords[keywords.Count - 1], keywords[0], !playerTarget == CardGameManager.Instance.player);
                        CardGameManager.Instance.cardSelectStack.Push(newSettings);
                    }
                }
                break;
            case SpecialKeyword.EFFECT_CONDITIONAL:
                //first, if keyword is conditional, verify the CONDITION. this will return a bool. if true, execute SUCCESS_PATH. if false, execute FAILURE_PATH
                //every conditional card must have EFFECT_CONDITION, CONDITION_[target] and then SUCCESS_PATH and FAILURE_PATH

                /*MARK
                 * I don't understand what was originally intended here or if some code is missing/unfinished. I think almost all the variables are never given values.
                 * For example, doesn't fill conditionalValues with values, but has valuesBin take on the conditionalValues. 
                 * conditionalValues is then never given any values
                 * 
                 * 
                List<SpecialKeyword> conditionalFlags = new List<SpecialKeyword>();
                List<SpecialKeyword> successCommand = new List<SpecialKeyword>();
                List<SpecialKeyword> failCommand = new List<SpecialKeyword>();

                List<int> conditionalValues = new List<int>();
                List<int> successValues = new List<int>();
                List<int> failValues = new List<int>();
                skippedValues = 0;
                List<SpecialKeyword> sortingBin = conditionalFlags;
                List<int> valuesBin = conditionalValues;



                for (int i = 1; i < keywords.Count; i++)
                {
                    if (keywords[i] == SpecialKeyword.SUCCESS_PATH)
                    {
                        sortingBin = successCommand;
                        valuesBin = successValues;
                    }
                    else if (keywords[i] == SpecialKeyword.FAILURE_PATH)
                    {
                        sortingBin = failCommand;
                        valuesBin = failValues;
                    }
                    else if (keywords[i] == SpecialKeyword.END_COMMAND)
                    {
                        break;
                    }

                    if (keywords[i] == SpecialKeyword.TARGET_PLAYER || keywords[i] == SpecialKeyword.TARGET_OPPONENT || keywords[i] == SpecialKeyword.CON_STORE_ADDITIONAL_INTEGER)
                    {
                        sortingBin.Add(keywords[i]);
                        valuesBin.Add(values[i - 1 - skippedValues]);
                    }
                    else
                    {
                        if (keywords[i] != SpecialKeyword.SUCCESS_PATH && keywords[i] != SpecialKeyword.FAILURE_PATH)
                        {
                            sortingBin.Add(keywords[i]);
                        }
                        skippedValues += 1;
                    }
                }

                bool successCheck = true;
                int flagsConsumed = 0;
                int valuesConsumed = 0;
                CharacterInstance flagTarget;

                for (int i = 0; i < conditionalFlags.Count; i++)
                {
                    flagsConsumed = 0;
                    if (conditionalFlags[i + 1] == SpecialKeyword.TARGET_PLAYER)
                    {
                        flagTarget = playerTarget;
                    }
                    else
                    {
                        flagTarget = opponentTarget;
                    }
                    switch (conditionalFlags[i])
                    {
                        case SpecialKeyword.CON_CARD_QUANTITY:
                            Debug.Log("quantity");
                            successCheck = successCheck & ConditionalCardQuantity(flagTarget, conditionalFlags[i + 2], conditionalValues[valuesConsumed], conditionalValues[valuesConsumed + 1]);
                            flagsConsumed += 3;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_DISCARD_FLAG:
                            Debug.Log("discard");
                            successCheck = successCheck & ConditionalDiscardFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_FLIP_FLAG:
                            Debug.Log("flip");
                            successCheck = successCheck & ConditionalFlipFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_HAS_CLASS_CARD:
                            Debug.Log("has class card");
                            successCheck = successCheck & ConditionalHasClassCard(flagTarget, (NumberCard.NumberClass)conditionalValues[valuesConsumed], conditionalValues[valuesConsumed + 1]);
                            flagsConsumed += 3;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_HAS_DUPLICATE:
                            Debug.Log("has dupe");
                            successCheck = successCheck & ConditionalHasDuplicate(flagTarget, conditionalFlags[i + 2]);
                            flagsConsumed += 2;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_HAS_VALUE_CARD:
                            Debug.Log("has value");
                            successCheck = successCheck & ConditionalHasValueCard(flagTarget, conditionalValues[valuesConsumed], conditionalValues[valuesConsumed + 1]);
                            flagsConsumed += 2;
                            valuesConsumed += 2;
                            break;
                        case SpecialKeyword.CON_SWAP_FLAG:
                            Debug.Log("swap");
                            successCheck = successCheck & ConditionalSwapFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_TRANSFER_FLAG:
                            Debug.Log("transfer");
                            successCheck = successCheck & ConditionalTransferFlag(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                        case SpecialKeyword.CON_COMPARE_AGAINST_TARGET:
                            Debug.Log("compare against target");
                            successCheck = successCheck & ConditionalCompareAgainstTarget(flagTarget);
                            flagsConsumed += 1;
                            valuesConsumed += 1;
                            break;
                    }
                    i += flagsConsumed;
                }

                Debug.Log(successCheck);

                if (successCheck)
                {
                    ExecuteCardEffect(successCommand, successValues, playerTarget, opponentTarget);
                }
                else
                {
                    ExecuteCardEffect(failCommand, failValues, playerTarget, opponentTarget);
                }
                
                break;
        }
    }*/

                /*TARGET -> color
                STORE INT -> count*/
                public bool ConditionalHasClass(CharacterInstance target, NumberCard.NumberClass color)
    {
        int x = 0;
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.cardClass == color)
            {
                x++;
            }
        }
        return x >= 1;
    }
    /*TARGET -> value
    STORE INT -> count*/
    public bool ConditionalHasCard(CharacterInstance target, int value)
    {
        int x = 0;
        foreach (DisplayCard d in target.numberDisplayHand)
        {
            NumberCard c = (NumberCard)d.baseCard;
            if (c.value == value)
            {
                x++;
            }
        }
        return x >= 1;
    }

    /*TARGET -> -1 (N/A)*/
    public bool ConditionalHasDuplicate(CharacterInstance target, CardType type)
    {
        if (type == CardType.Special)
        {
            for (int i = 0; i < target.specialDisplayHand.Count - 1; i++)
            {
                for (int j = i + 1; j < target.specialDisplayHand.Count; j++)
                {
                    if (target.specialDisplayHand[i].name == target.specialDisplayHand[j].name)
                    {
                        return true;
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
                        return true;
                    }
                }
            }
        }
        return false;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalDiscardFlag(CharacterInstance target)
    {
        //NYI
        return true;
        return target.DiscardedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalSwapFlag(CharacterInstance target)
    {
        //NYI
        return true;
        return target.SwappedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalFlipFlag(CharacterInstance target)
    {
        //NYI
        return true;
        return target.FlippedThisTurn;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalGaveACard(CharacterInstance target)
    {
        //NYI
        return true;
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
