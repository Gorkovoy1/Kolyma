using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSequence : MonoBehaviour
{
    /// <summary>
    /// Lower evaluation value is more favorable
    /// </summary>
    public int EvaluationValue;
    public int PlayerValue, OpponentValue;

    public Queue<AIAction> Actions;
    public Queue<DisplayCard> Cards;

    private int DistanceFromTargetComparedToOpponent;
    private int PlayerDrawnCardsNum, PlayerDiscardedCardsNum, PlayerSwapCardsNum, PlayerFlipCardsNum;
    private int OpponentDrawnCardsNum, OpponentDiscardedCardsNum, OpponentSwapCardsNum, OpponentFlipCardsNum;
    private AIPersonality aiPersonality;


    public CardSequence(Queue<AIAction> actions, Queue<DisplayCard> cards, AIPersonality aiPersonality)
    {
        PlayerValue = CardEffectChecker.Instance.SimulatedPlayerValue;
        OpponentValue = CardEffectChecker.Instance.SimulatedOpponentValue;
        PlayerDrawnCardsNum = CardEffectChecker.Instance.SimulatedPlayerDrawnCardsNum;
        PlayerDiscardedCardsNum = CardEffectChecker.Instance.SimulatedPlayerDiscardedCardsNum;
        PlayerSwapCardsNum = CardEffectChecker.Instance.SimulatedPlayerSwapCardsNum;
        PlayerFlipCardsNum = CardEffectChecker.Instance.SimulatedPlayerFlipCardsNum;
        OpponentDrawnCardsNum = CardEffectChecker.Instance.SimulatedOpponentDrawnCardsNum;
        OpponentDiscardedCardsNum = CardEffectChecker.Instance.SimulatedOpponentDiscardedCardsNum;
        OpponentSwapCardsNum = CardEffectChecker.Instance.SimulatedPlayerSwapCardsNum;
        OpponentFlipCardsNum = CardEffectChecker.Instance.SimulatedOpponentFlipCardsNum;

        Actions = actions;
        Cards = cards;
        actions.CopyTo(Actions.ToArray(), 0);
        cards.CopyTo(Cards.ToArray(), 0);
        CalculateDistanceFromTarget();
        this.aiPersonality = aiPersonality;

        Evaluate();
    }

    void CalculateDistanceFromTarget()
    {
        DistanceFromTargetComparedToOpponent = Mathf.Abs(CardGameManager.Instance.targetValue - PlayerValue) - Mathf.Abs(CardGameManager.Instance.targetValue - OpponentValue);
    }

    public void Evaluate()
    {
        EvaluationValue = DistanceFromTargetComparedToOpponent;
        if (PlayerValue == CardGameManager.Instance.targetValue)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AIAtTarget);
        }
        
        if(OpponentValue == CardGameManager.Instance.targetValue)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentAtTarget);
        }
        
        if (PlayerValue > CardGameManager.Instance.targetValue)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AIOverTarget);
        }
        
        if (OpponentValue > CardGameManager.Instance.targetValue)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentOverTarget);
        }

        if (PlayerDrawnCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AIDrawsACard);
        }
        if (OpponentDrawnCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentDrawsACard);
        }

        if (PlayerDiscardedCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AIDiscardsACard);
        }
        if (OpponentDiscardedCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentDiscardsACard);
        }
        if (PlayerSwapCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AISwapsACard);
        }
        if (OpponentSwapCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentSwapsACard);
        }
        if (PlayerFlipCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.AIFlipsACard);
        }
        if (OpponentFlipCardsNum > 0)
        {
            EvaluationValue += AIPersonality.GetPriorityValue(aiPersonality.OpponentFlipsACard);
        }
    }

    public override string ToString()
    {
        string sequence = "[EVAL: " + EvaluationValue + ", PLAYER: " + PlayerValue + ", OPP: " + OpponentValue + "] = ";
        AIAction[] ActionsArr = Actions.ToArray();
        DisplayCard[] CardsArr = Cards.ToArray();
        for (int j = 0; j < ActionsArr.Length; j++)
        {
            string cardName;
            if(CardsArr[j] != null)
            {
                if (CardsArr[j].SpecialCard != null)
                {
                    cardName = CardsArr[j].SpecialCard.name;
                }
                else
                {
                    cardName = CardsArr[j].value + "";
                }
                sequence += ActionsArr[j] + " " + cardName + " => ";
            }
        }
        return sequence;
    }
}
