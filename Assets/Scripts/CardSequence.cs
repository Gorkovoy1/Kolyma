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

    public int EvaluationValue;


    public CardSequence(int playerValue, int opponentValue, int distanceFromTarget, Queue<AIAction> actions, Queue<DisplayCard> cards, int evaluationValue)
    {
        PlayerValue = playerValue;
        OpponentValue = opponentValue;
        Actions = actions;
        Cards = cards;
        actions.CopyTo(Actions.ToArray(), 0);
        cards.CopyTo(Cards.ToArray(), 0);
        DistanceFromTarget = distanceFromTarget;
        EvaluationValue = evaluationValue;
        Debug.Log("New Card Sequence!");
    }
}
