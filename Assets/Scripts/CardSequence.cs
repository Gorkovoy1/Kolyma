using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSequence : MonoBehaviour
{
    public Queue<AIAction> Actions;

    public Queue<DisplayCard> Cards;

    public List<Effect> effects;

    public int PlayerValue, OpponentValue;

    public int DistanceFromTarget;

    //Lower the better
    public int EvaluationValue;


    public CardSequence(int playerValue, int opponentValue, Queue<AIAction> actions, Queue<DisplayCard> cards)
    {
        PlayerValue = playerValue;
        OpponentValue = opponentValue;
        Actions = actions;
        Cards = cards;
        actions.CopyTo(Actions.ToArray(), 0);
        cards.CopyTo(Cards.ToArray(), 0);
        CalculateDistanceFromTarget();
        Evaluate();
        Debug.Log("New Card Sequence: " + actions.Count);
    }

    void CalculateDistanceFromTarget()
    {
        DistanceFromTarget = Mathf.Abs(CardGameManager.Instance.targetValue - PlayerValue) - Mathf.Abs(CardGameManager.Instance.targetValue - OpponentValue);
    }

    public void Evaluate()
    {
        AIEvaluationValues evaluationValues = CardGameManager.Instance.AIEvaluationValues;
        EvaluationValue = DistanceFromTarget;
        if (PlayerValue == CardGameManager.Instance.targetValue)
        {
            //If Player Value is equal to target, it's really good
            EvaluationValue += evaluationValues.PlayerAtTarget;
        }
        else if(OpponentValue == CardGameManager.Instance.targetValue)
        {
            //If Opponent Value is equal to target, it's really bad
            EvaluationValue += evaluationValues.OpponentAtTarget;
        }
        else if (PlayerValue > CardGameManager.Instance.targetValue)
        {
            //If Player Value is greater than target, it's bad
            EvaluationValue += evaluationValues.PlayerOverTarget;
        }
        else if (OpponentValue > CardGameManager.Instance.targetValue)
        {
            //If Opponent Value is greater than target, it's good
            EvaluationValue += evaluationValues.OpponentOverTarget;
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
