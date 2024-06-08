using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIAction {PlayCard, Flip, Swap, None}

public class CardGameAI : MonoBehaviour
{
    private CharacterInstance CurrentCharacter;
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

    public void Init(CharacterInstance currCharacter)
    {
        CurrentCharacter = currCharacter;
        CardGameManager.Instance.OnCharacterChange += CheckActiveCharacter;
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
            CardGameManager.Instance.StartSelecting(CurrentCharacter);
            ChosenAction = AIAction.None;
            StartCoroutine(WaitForNextAction());
        }
    }

    void ChooseNextAction()
    {
        if (CardGameManager.Instance.SelectingCharacter != CurrentCharacter)
            return;

        if (!CurrentCharacter.DidAnAction && ChosenAction == AIAction.None)
        {
            int randAction = Random.Range(0, 3);

            ChosenAction = (AIAction)randAction;
        }

        if(CurrentCharacter.DidAnAction && !CardSelectionHandler.Instance.SelectingCards)
        {
            EndTurn();
        }
        else
        {
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
    }

    void PlayingCard()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        if (SelectableCards.Count > 0)
        {
            if (!CardSelectionHandler.Instance.SelectingCards)
            {
                ChooseInitialCard();
            }
            else
            {
                ChooseEffectCards();
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
            int selectedIndex = Random.Range(0, SelectableCards.Count);
            CardSelectionHandler.Instance.SelectCard(SelectableCards[selectedIndex], CurrentCharacter);
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
            int selectedIndex = Random.Range(0, SelectableCards.Count);
            CardSelectionHandler.Instance.SelectCard(SelectableCards[selectedIndex], CurrentCharacter);
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
        int selectedIndex = Random.Range(0, SelectableCards.Count);
        CardGameManager.Instance.StartSelecting(CurrentCharacter);
        CardGameManager.Instance.PlayCard(SelectableCards[selectedIndex]);
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
