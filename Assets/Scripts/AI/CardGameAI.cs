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
    public List<string> DebugCardSequenceActions;

    public CardSequence ChosenSequence;

    //Lower value = Better
    public int EvaluationValPlayerDraw = -1, EvaluationValPlayerDiscard = 2, EvaluationValOppDraw = 2, EvaluationValOppDiscard = -2;

    private bool Evaluated;

    public void Init(CharacterInstance currCharacter, CharacterInstance opponent)
    {
        CurrentCharacter = currCharacter;
        Opponent = opponent;
        CardGameManager.Instance.OnCharacterChange += CheckActiveCharacter;
        CardSequences = new List<CardSequence>();
    }

    private void Update()
    {
        if (!Active) return;

        if(Waiting && ChosenAction == AIAction.None && Evaluated)
        {
            ChooseNextAction();
        }
        else if(CardGameManager.Instance.SelectingCharacter == CurrentCharacter && CardGameManager.Instance.CurrentCharacter == CardGameManager.Instance.player)
        {
            SelectCards();
        }
    }

    public void CheckActiveCharacter(CharacterInstance character)
    {
        if (Active && character == CurrentCharacter) return;
        Active = character == CurrentCharacter;
        if(Active)
        {
            CardGameManager.Instance.StartSelecting();
            ChosenAction = AIAction.None;
            Evaluated = false;
            CardSequences.Clear();
            Evaluate();
        }
    }

    void Evaluate()
    {
        EvaluatePass();
        EvaluateSwapCardOptions();
        EvaluateFlipCardOptions();
        EvaluatePlayCardOptions();
        SortCardSequences();
        ChooseNextCardSequence();
        StartCoroutine(WaitForNextAction());
        Evaluated = true;
    }

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
        //SelectableCards = CardGameManager.Instance.GetSelectableCards();
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

    void AddNewSequence(Queue<AIAction> actions, Queue<DisplayCard> cards)
    {
        //int playerValue = CardEffectChecker.Instance.SimulatedPlayerValue;
        //int opponentValue = CardEffectChecker.Instance.SimulatedOpponentValue;

        /*if(CurrentCharacter == CardEffectChecker.Instance.Player)
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
        }*/

        CardSequences.Add(new CardSequence(actions, cards, CurrentCharacter.character.AIPersonality));
    }

    void SortCardSequences()
    {
        CardSequences.Sort(delegate (CardSequence c1, CardSequence c2) { return c1.EvaluationValue.CompareTo(c2.EvaluationValue); });
        DebugCardSequenceActions = new List<string>();

        for(int i = 0; i < CardSequences.Count; i++)
        {
            string sequence = CardSequences[i].ToString();
            DebugCardSequenceActions.Add(sequence);
        }
    }

    void ChooseNextCardSequence()
    {
        /*float randChoice = 0f;

        for(int i = 0; i < 100; i++)
        {
            randChoice += Random.Range(0, 100 / 10);
        }
        randChoice /= 100f;

        if(randChoice >= CardSequences.Count)
        {
            randChoice = CardSequences.Count - 1;
        }*/

        //ChosenSequence = CardSequences[Mathf.FloorToInt(randChoice)];



        ChosenSequence = CardSequences[Random.Range(0, (int)CurrentCharacter.character.AIPersonality.AIDifficulty + 1)];

        CardGameLog.Instance.AddToLog("Chosen Sequence: " + ChosenSequence.ToString());
    }


    void ChooseNextAction()
    {
        if (CardGameManager.Instance.SelectingCharacter != CurrentCharacter)
            return;

        /*if(Random.Range(0,2) == 0)
        {
            PassTurn();
            return;
        }*/

        if (ChosenSequence.Actions.Count > 0)
        {
            ChosenAction = ChosenSequence.Actions.Dequeue();
            ChosenCard = ChosenSequence.Cards.Dequeue();
        }

        //if(ChosenSequence.Actions.Count == 0)
        //    EndTurn();

        switch (ChosenAction)
        {
            case AIAction.PlayCard:
                PlayingCard();
                CurrentCharacter.DidAnAction = true;
                break;
            case AIAction.Flip:
                FlippingCard();
                CurrentCharacter.DidAnAction = true;
                break;
            case AIAction.Swap:
                SwappingCard();
                CurrentCharacter.DidAnAction = true;
                break;
            case AIAction.None:
                CardGameManager.Instance.CharacterEndTurn();
                CurrentCharacter.DidAnAction = false;
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

    public void PassTurn()
    {
        CardGameManager.Instance.CharacterEndTurn();
        EndTurn();
    }

    void EndTurn()
    {
        if(Active)
        {
            Active = false;
            Waiting = false;
        }
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
