using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIAction {PlayCard = 0, Flip = 1, Swap = 2, SelectCard = 3, None = 4}


/*
 * Normal AIState route
 * 1) Inactive (Opponent Turn)
 * 2) Evaluate Options 
 * 3) Do card sequence. Do a-c as neccessary until sequence is completed
 *      a. Do Action 
 *      b. Opponent selects card for card effect (discard, etc.)
 *      c. AI selects Card for card effect (discard, etc.)
 * 4) Inactive (End Turn)
 * 
 */
public enum AIState { Inactive = 0, Evaluating = 1, DoingAction = 2, WaitingForOpponent = 3, SelectingCard = 4}

public class CardGameAI : MonoBehaviour
{
    public AIState CurrentState;

    public List<DisplayCard> SelectableCards;

    //1) Detect if Active
    //2) Once Active, check all possible actions.
    //  For now: Just chooses a move randomly
    //  Next version: Chooses based on current best move without any RNG. Moves that do have RNG, will be done last.
    //    2a)Simulate selecting each possible card currently selecatable. Once there are no possible selections, check if that sequence resulted in a positive outcome.
    //    2b) If it did, return the sequence of cards chosen. Once all positive outcomes are simulated, choose the best one.
    //  **For the future: Calculate the all of the outcomes for the entire turn (not just one single selection) and choose based on supposed AI difficulty
    //3) Wait for x amount of time and then repeat from step 2 if there are still  options
    //4) Once there are no more positive outcomes, end turn.

    public List<DisplayCard> CurrentSelectedCards;
    
    public List<CardSequence> CardSequences;

    public CardSequence ChosenSequence;

    private CharacterInstance CurrentCharacter, Opponent;
    private AIAction ChosenAction;
    private DisplayCard ChosenCard;
    private AIEvaluator Evaluator;

    public void Init(CharacterInstance currCharacter, CharacterInstance opponent)
    {
        CurrentCharacter = currCharacter;
        Opponent = opponent;
        CardGameManager.Instance.OnCharacterChange += CheckActiveCharacter;
        CardSequences = new List<CardSequence>();
        Evaluator = gameObject.AddComponent<AIEvaluator>();
        Evaluator.Initialize(CurrentCharacter, Opponent);
    }

    public void SetState(AIState newState)
    {
        if (newState == CurrentState) return;
        CurrentState = newState;
        switch (CurrentState)
        {
            case AIState.Inactive:
                CurrentState = AIState.Inactive;
                ChosenAction = AIAction.None;
                CardSequences.Clear();
                CardSequences = null;
                CardGameManager.Instance.CharacterEndTurn(CurrentCharacter);
                break;
            case AIState.Evaluating:
                CardGameManager.Instance.StartSelecting();
                CardSequences = Evaluator.Evaluate();
                ChosenSequence = CardSequences[Random.Range(0, (int)CurrentCharacter.character.AIPersonality.AIDifficulty + 1)];
                StartCoroutine(DramaticPause(AIState.DoingAction));
                break;
            case AIState.DoingAction:
                DoNextAction();
                break;
            case AIState.WaitingForOpponent:
                break;
            case AIState.SelectingCard:
                SelectCards();
                break;
        }
    }

    public void CheckActiveCharacter(CharacterInstance character)
    {
        if(character == CurrentCharacter)
        {
            SetState(AIState.Evaluating);
        }
    }


    void DoNextAction()
    {
        if (CardGameManager.Instance.SelectingCharacter != CurrentCharacter)
            return;

        if (ChosenSequence.Actions.Count > 0)
        {
            ChosenAction = ChosenSequence.Actions.Dequeue();
            ChosenCard = ChosenSequence.Cards.Dequeue();
        }

        CurrentCharacter.DidAnAction = ChosenAction != AIAction.None;

        switch (ChosenAction)
        {
            case AIAction.PlayCard:
                PlayCard();
                break;
            case AIAction.Flip:
                FlipCard();
                break;
            case AIAction.Swap:
                SwapCard();
                CurrentCharacter.DidAnAction = true;
                break;
            case AIAction.None:
                //Pass
                SetState(AIState.Inactive);
                break;
        }
    }

    void PlayCard()
    {
        CardGameManager.Instance.PlayCard(ChosenCard);
    }

    void FlipCard()
    {
        CardGameManager.Instance.SetFlip(true);
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        while (!CardSelectionHandler.Instance.CheckConditionsMet())
        {
            CardSelectionHandler.Instance.SelectCard(ChosenCard, CurrentCharacter);
        }
        CardSelectionHandler.Instance.ConfirmSelection();

        EndTurn();
    }

    void SwapCard()
    {
        CardGameManager.Instance.SetSwap(true);
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        while(!CardSelectionHandler.Instance.CheckConditionsMet())
        {
            CardSelectionHandler.Instance.SelectCard(ChosenCard, CurrentCharacter);
        }
        CardSelectionHandler.Instance.ConfirmSelection();

        EndTurn();
    }

    void EndTurn()
    {
        SetState(AIState.Inactive);
    }

    /// <summary>
    /// Meant for when AI has to choose cards for purposes other than playing
    /// </summary>
    void SelectCards()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        do
        {
            DisplayCard selectedCard = SelectableCards[Random.Range(0, SelectableCards.Count)];
            CardSelectionHandler.Instance.SelectCard(selectedCard, CurrentCharacter);
            CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " selects card: " + selectedCard.baseCard.name);
        } while (!CardSelectionHandler.Instance.CheckConditionsMet());

        CardSelectionHandler.Instance.ConfirmSelection();
    }

    IEnumerator DramaticPause(AIState nextState)
    {
        yield return new WaitForSecondsRealtime(3f);
        SetState(nextState);
    }
}
