using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Init(CharacterInstance currCharacter)
    {
        CurrentCharacter = currCharacter;
        CardGameManager.Instance.OnCharacterChange += CheckActiveCharacter;
    }

    public void CheckActiveCharacter(CharacterInstance character)
    {
        //Band-aid fix, should just change state method in game manager to outisde of Update.
        if (Active && character == CurrentCharacter) return;
        Active = character == CurrentCharacter;
        if(Active)
        {
            CardGameManager.Instance.state = CardGameManager.State.SELECTCARDS;
            StartCoroutine(WaitForNextAction());
        }
    }

    void ChooseNextAction()
    {
        SelectableCards = CardGameManager.Instance.GetSelectableCards();
        if (SelectableCards.Count > 0)
        {
            if(!CardSelectionHandler.Instance.SelectingCards)
            {
                ChooseInitialCard();
            }
            else
            {
                ChooseStackCards();
            }
        }
        else
        {
            CardGameManager.Instance.EndTurn();
        }
    }

    void ChooseInitialCard()
    {
        int selectedIndex = Random.Range(0, SelectableCards.Count);
        CardGameManager.Instance.PlayCard(SelectableCards[selectedIndex]);
        CardGameManager.Instance.state = CardGameManager.State.SELECTCARDS;
        StartCoroutine(WaitForNextAction());
    }

    void ChooseStackCards()
    {
        int selectedIndex = Random.Range(0, SelectableCards.Count);
        CardSelectionHandler.Instance.SelectCard(SelectableCards[selectedIndex]);
        Debug.Log(CurrentCharacter.character.name + " selects card: " + SelectableCards[selectedIndex].baseCard.name);
        if (CardSelectionHandler.Instance.CheckConditionsMet())
        {
            CardSelectionHandler.Instance.ConfirmSelection();
            StartCoroutine(WaitForNextAction());
        }
        else
        {
            StartCoroutine(WaitForNextAction());
        }
    }

    IEnumerator WaitForNextAction()
    {
        Debug.Log("Wait For Next Action!");
        yield return new WaitForSecondsRealtime(3f);
        ChooseNextAction();
    }
}
