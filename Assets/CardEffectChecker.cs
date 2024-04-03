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

    //Very PROTOTYPE version of the card decision tree
    public void ExecuteCardEffect(List<SpecialKeyword> keywords, List<int> values, CharacterInstance playerTarget, CharacterInstance opponentTarget)
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
        SpecialKeyword effectType = keywords[0];

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
                for (int i = 1; i < keywords.Count; i++)
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
                SpecialKeyword cardType = keywords[keywords.Count - 1];

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
                for (int i = 1; i < keywords.Count - 1; i++)
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
    }

    /*TARGET -> color
    STORE INT -> count*/
    public bool ConditionalHasClassCard(CharacterInstance target, NumberCard.NumberClass color, int count = 1)
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
        return x >= count;
    }
    /*TARGET -> value
    STORE INT -> count*/
    public bool ConditionalHasValueCard(CharacterInstance target, int value, int count = 1)
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
        return x >= count;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalHasDuplicate(CharacterInstance target, SpecialKeyword type)
    {
        if (type == SpecialKeyword.TYPE_SPECIAL)
        {
            for (int i = 0; i < target.hand.Count - 1; i++)
            {
                for (int j = i + 1; j < target.hand.Count; j++)
                {
                    if (target.hand[i].name == target.hand[j].name)
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
        return target.discardFlag;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalSwapFlag(CharacterInstance target)
    {
        return target.swapFlag;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalFlipFlag(CharacterInstance target)
    {
        return target.flipFlag;
    }
    /*TARGET -> -1 (N/A)*/
    public bool ConditionalTransferFlag(CharacterInstance target)
    {
        return target.transferFlag;
    }
    /*TARGET -> min
    STORE INT -> max*/
    public bool ConditionalCardQuantity(CharacterInstance target, SpecialKeyword type, int min, int max)
    {
        if (type == SpecialKeyword.TYPE_SPECIAL)
        {
            return (target.hand.Count >= min && target.hand.Count <= max);
        }
        else
        {
            return (target.numberDisplayHand.Count >= min && target.numberDisplayHand.Count <= max);
        }
    }

    /* TARGET -> -1 (N/A) */
    public bool ConditionalCompareAgainstTarget(CharacterInstance target)
    {
        return target.currValue >= CardGameManager.Instance.targetValue;
    }

}
