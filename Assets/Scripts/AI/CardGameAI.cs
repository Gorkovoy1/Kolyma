using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIAction {PlayCard, Flip, Swap, SelectCard, None}

public enum AIState { Evaluating, Playing, Waiting }

public class CardGameAI : MonoBehaviour
{
    private CharacterInstance CurrentCharacter, Opponent;

    public int Difficulty;

    private bool Active;

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

    private bool Waiting;

    public AIAction ChosenAction;
    public DisplayCard ChosenCard;

    public List<CardSequence> CardSequences;
    public CardSequence ChosenSequence;

    //Lower value = Better
    public int EvaluationValPlayerDraw = -1, EvaluationValPlayerDiscard = 2, EvaluationValOppDraw = 2, EvaluationValOppDiscard = -2;

    public void Init(CharacterInstance currCharacter, CharacterInstance opponent)
    {
        CurrentCharacter = currCharacter;
        Opponent = opponent;
        CardGameManager.Instance.OnCharacterChange += CheckActiveCharacter;
        CardSequences = new List<CardSequence>();
    }

    private void Update()
    {
        if(Waiting && Active)
        {
            ChooseNextAction();
        }
        else if(CardGameManager.Instance.SelectingCharacter == CurrentCharacter && !Active)
        {
            SelectCards();
        }
    }

    public void CheckActiveCharacter(CharacterInstance character)
    {
        //Band-aid fix, should just change state method in game manager to outisde of Update.
        if (Active && character == CurrentCharacter) return;
        Active = character == CurrentCharacter && character.IsAI;
        if(Active)
        {
            CardGameManager.Instance.StartSelecting();
            ChosenAction = AIAction.None;
            Evaluate();
        }
    }

    void Evaluate()
    {
        EvaluateSwapCardOptions();
        EvaluateFlipCardOptions();
        EvaluatePlayCardOptions();
        SortCardSequences();
        ChooseNextCardSequence();
        StartCoroutine(WaitForNextAction());
    }

    void EvaluateSwapCardOptions()
    {
        for(int i = 0; i < CurrentCharacter.numberDisplayHand.Count; i++)
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

    void EvaluateFlipCardOptions()
    {
        for(int i = 0; i < CurrentCharacter.numberDisplayHand.Count; i++)
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

    void EvaluatePlayCardOptions()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        for (int i = 0; i < SelectableCards.Count; i++)
        {
            SpecialDeckCard currCard = (SpecialDeckCard)SelectableCards[i].baseCard;
            CardEffectChecker.Instance.ExecuteEffectStatement(currCard.InitialEffectStatement, CurrentCharacter, Opponent, true, true);
            Queue<AIAction> actions = new Queue<AIAction>();
            Queue<DisplayCard> cards = new Queue<DisplayCard>();
            actions.Enqueue(AIAction.PlayCard);
            cards.Enqueue(SelectableCards[i]);
            AddNewSequence(actions, cards);
        }
    }

    void AddNewSequence(Queue<AIAction> actions, Queue<DisplayCard> cards)
    {
        int playerValue = CardEffectChecker.Instance.SimulatedPlayerValue;
        int opponentValue = CardEffectChecker.Instance.SimulatedOpponentValue;
        int distanceFromTarget = Mathf.Abs(CardGameManager.Instance.targetValue - playerValue) - Mathf.Abs(CardGameManager.Instance.targetValue - opponentValue);
        int evaluationValue = distanceFromTarget;

        if(CurrentCharacter == CardEffectChecker.Instance.Player)
        {
            evaluationValue += CardEffectChecker.Instance.PlayerDrawnCardsNum * EvaluationValPlayerDraw;
            evaluationValue += CardEffectChecker.Instance.PlayerDiscardedCardsNum * EvaluationValPlayerDiscard;
            evaluationValue += CardEffectChecker.Instance.OpponentDrawnCardsNum * EvaluationValOppDraw;
            evaluationValue += CardEffectChecker.Instance.OpponentDiscardedCardsNum * EvaluationValOppDiscard;
        }
        else
        {
            evaluationValue += CardEffectChecker.Instance.PlayerDrawnCardsNum * EvaluationValOppDraw;
            evaluationValue += CardEffectChecker.Instance.PlayerDiscardedCardsNum * EvaluationValOppDiscard;
            evaluationValue += CardEffectChecker.Instance.OpponentDrawnCardsNum * EvaluationValPlayerDraw;
            evaluationValue += CardEffectChecker.Instance.OpponentDiscardedCardsNum * EvaluationValPlayerDiscard;
        }

        CardSequences.Add(new CardSequence(playerValue, opponentValue, distanceFromTarget, actions, cards, evaluationValue));
    }

    void SortCardSequences()
    {
        CardSequences.Sort(delegate (CardSequence c1, CardSequence c2) { return c1.EvaluationValue.CompareTo(c2.EvaluationValue); });
    }

    void ChooseNextCardSequence()
    {
        float randChoice = 0f;

        for(int i = 0; i < 100; i++)
        {
            randChoice += Random.Range(0, 100 / 10);
        }
        randChoice /= 100f;

        if(randChoice >= CardSequences.Count)
        {
            randChoice = CardSequences.Count - 1;
        }

        ChosenSequence = CardSequences[Mathf.FloorToInt(randChoice)];
    }


    void ChooseNextAction()
    {
        if (CardGameManager.Instance.SelectingCharacter != CurrentCharacter)
            return;

        if (ChosenSequence.Actions.Count > 0)
        {
            ChosenAction = ChosenSequence.Actions.Dequeue();
            ChosenCard = ChosenSequence.Cards.Dequeue();
        }
        else
        {
            EndTurn();
        }

        switch (ChosenAction)
        {
            case AIAction.PlayCard:
                PlayingCard();
                CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " AI wants to play a card!");
                break;
            case AIAction.Flip:
                CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " AI wants to flip a card!");
                FlippingCard();
                break;
            case AIAction.Swap:
                CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " AI wants to swap a card!");
                SwappingCard();
                break;
        }
    }

    void PlayingCard()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        if (SelectableCards.Count > 0)
        {
            if (CardSelectionHandler.Instance.SelectingCards)
            {
                ChooseEffectCards();
            }
            else
            {
                ChooseInitialCard();
            }
        }
        else
        {
            EndTurn();
        }
    }

    void FlippingCard()
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

    void SwappingCard()
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
        CardGameManager.Instance.EndTurn();
        Active = false;
        Waiting = false;
    }

    void SelectCards()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        if (SelectableCards.Count > 0)
        {
            ChooseEffectCards(false);
        }
    }

    void ChooseInitialCard(bool nextAction = true)
    {
        CardGameManager.Instance.StartSelecting();
        CardGameManager.Instance.PlayCard(ChosenCard);
        if(nextAction)
            StartCoroutine(WaitForNextAction());
    }

    void ChooseEffectCards(bool nextAction = true)
    {
        int selectedIndex = Random.Range(0, SelectableCards.Count);
        CardSelectionHandler.Instance.SelectCard(SelectableCards[selectedIndex], CurrentCharacter);
        CardGameLog.Instance.AddToLog(CurrentCharacter.character.name + " selects card: " + SelectableCards[selectedIndex].baseCard.name);
        if (CardSelectionHandler.Instance.CheckConditionsMet())
        {
            CardSelectionHandler.Instance.ConfirmSelection();

            if(nextAction)
                EndTurn();
            //if (nextAction)
            //    StartCoroutine(WaitForNextAction());
        }
        else
        {
            if (nextAction)
                StartCoroutine(WaitForNextAction());
        }
    }

    IEnumerator WaitForNextAction()
    {
        Waiting = false;
        yield return new WaitForSecondsRealtime(3f);
        Waiting = true;
    }
}
