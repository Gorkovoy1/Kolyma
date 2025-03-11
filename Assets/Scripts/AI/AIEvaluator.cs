using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEvaluator : MonoBehaviour
{
    public List<string> DebugSequences;

    private CharacterInstance CurrentCharacter, Opponent;

    private List<CardSequence> evaluatedSequences;

    public void Initialize(CharacterInstance currentCharacter, CharacterInstance opponent)
    {
        this.CurrentCharacter = currentCharacter;
        this.Opponent = opponent;
    }


    /// <summary>
    /// Evaluates all options
    /// </summary>
    public List<CardSequence> Evaluate()
    {
        evaluatedSequences = new List<CardSequence>();
        EvaluatePass();
        EvaluateSwapCardOptions();
        EvaluateFlipCardOptions();
        EvaluatePlayCardOptions();
        SortCardSequences();
        return evaluatedSequences;
    }

    /// <summary>
    /// Evaluates the pass option
    /// </summary>
    void EvaluatePass()
    {
        Queue<AIAction> actions = new Queue<AIAction>();
        actions.Enqueue(AIAction.None);
        Queue<DisplayCard> cards = new Queue<DisplayCard>();
        cards.Enqueue(null);
        CardEffectChecker.Instance.ResetSimulatedValues(CurrentCharacter, Opponent);
        CardEffectChecker.Instance.SimulateEffect(Effect.None, CurrentCharacter, CardType.None, 0, 0);
        AddNewSequence(actions, cards);
    }

    /// <summary>
    /// Evaluates all of the swap options
    /// </summary>
    void EvaluateSwapCardOptions()
    {
        for (int i = 0; i < CurrentCharacter.numberDisplayHand.Count; i++)
        {
            DisplayCard currentCard = CurrentCharacter.numberDisplayHand[i];
            CardEffectChecker.Instance.ResetSimulatedValues(CurrentCharacter, Opponent);
            CardEffectChecker.Instance.SimulateEffect(Effect.Swap, CurrentCharacter, CardType.Number, 1, currentCard.value);
            Queue<AIAction> actions = new Queue<AIAction>();
            Queue<DisplayCard> cards = new Queue<DisplayCard>();
            actions.Enqueue(AIAction.Flip);
            cards.Enqueue(currentCard);
            AddNewSequence(actions, cards);
        }
    }

    /// <summary>
    /// Evaluates all of the flip options
    /// </summary>
    void EvaluateFlipCardOptions()
    {
        for (int i = 0; i < CurrentCharacter.numberDisplayHand.Count; i++)
        {
            DisplayCard currentCard = CurrentCharacter.numberDisplayHand[i];
            CardEffectChecker.Instance.ResetSimulatedValues(CurrentCharacter, Opponent);
            CardEffectChecker.Instance.SimulateEffect(Effect.Flip, CurrentCharacter, CardType.Number, 1, currentCard.value);
            Queue<AIAction> actions = new Queue<AIAction>();
            Queue<DisplayCard> cards = new Queue<DisplayCard>();
            actions.Enqueue(AIAction.Flip);
            cards.Enqueue(currentCard);
            AddNewSequence(actions, cards);
        }
    }

    /// <summary>
    /// Evaluates all special card options
    /// </summary>
    void EvaluatePlayCardOptions()
    {
        for (int i = 0; i < CurrentCharacter.specialDisplayHand.Count; i++)
        {
            SpecialDeckCard currCard = (SpecialDeckCard)CurrentCharacter.specialDisplayHand[i].baseCard;
            if (CardEffectChecker.Instance.CheckConditional(currCard.InitialEffectStatement, CurrentCharacter, CurrentCharacter.Opponent))
            {
                CardEffectChecker.Instance.ExecuteEffectStatement(currCard.InitialEffectStatement, CurrentCharacter, Opponent, true, true);
                Queue<AIAction> actions = new Queue<AIAction>();
                Queue<DisplayCard> cards = new Queue<DisplayCard>();
                actions.Enqueue(AIAction.PlayCard);
                cards.Enqueue(CurrentCharacter.specialDisplayHand[i]);
                AddNewSequence(actions, cards);
            }
        }
    }

    /// <summary>
    /// Adds a sequence to
    /// </summary>
    /// <param name="actions"></param>
    /// <param name="cards"></param>
    void AddNewSequence(Queue<AIAction> actions, Queue<DisplayCard> cards)
    {
        evaluatedSequences.Add(new CardSequence(actions, cards, CurrentCharacter.character.AIPersonality));
    }

    void SortCardSequences()
    {
        evaluatedSequences.Sort(delegate (CardSequence c1, CardSequence c2) { return c1.EvaluationValue.CompareTo(c2.EvaluationValue); });
        DebugSequences = new List<string>();

        for (int i = 0; i < evaluatedSequences.Count; i++)
        {
            string sequence = evaluatedSequences[i].ToString();
            DebugSequences.Add(sequence);
        }
    }
}
