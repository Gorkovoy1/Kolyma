using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionHandler : MonoBehaviour
{
    public CardSelectSettings CurrSettings;

    public bool SelectingCards;

    private bool SelectConditionsMet;

    public List<DisplayCard> SelectableCards;

    public List<DisplayCard> SelectedCards;

    public static CardSelectionHandler Instance;

    public CharacterInstance SelectingCharacter;

    private void Awake()
    {
        Instance = this;
    }


    public void ProcessSelect(CharacterInstance selectingCharacter)
    {
        this.SelectingCharacter = selectingCharacter;
        Debug.Log(SelectingCharacter.character.name + " is selecting!");

        if (!SelectingCards && CardGameManager.Instance.cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = CardGameManager.Instance.cardSelectStack.Pop();
            StartSelectingCards(curr);
        }
        if (selectingCharacter == CardGameManager.Instance.player)
            CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerSelecting);
    }

    public void StartSelectingCards(CardSelectSettings cardSelectSettings)
    {
        SelectingCards = true;
        CurrSettings = cardSelectSettings;
        if (cardSelectSettings.SpecificCards != null)
        {
            SelectableCards = cardSelectSettings.SpecificCards;            
        }
        else
        {
            CardType cardType = cardSelectSettings.cardType;
            switch (cardSelectSettings.target)
            {
                case TargetCharacter.None:
                    break;
                case TargetCharacter.Anyone:
                    if (cardType == CardType.Number)
                    {
                        SelectableCards = CardGameManager.Instance.CurrentCharacter.numberDisplayHand;
                        SelectableCards.AddRange(CardGameManager.Instance.WaitingCharacter.numberDisplayHand);
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = CardGameManager.Instance.CurrentCharacter.specialDisplayHand;
                        SelectableCards.AddRange(CardGameManager.Instance.WaitingCharacter.specialDisplayHand);
                    }
                    break;
                case TargetCharacter.PlayerOfCard:
                    CharacterInstance currentCharacter = CardGameManager.Instance.CurrentCharacter;
                    if (cardType == CardType.Number)
                    {
                        SelectableCards = currentCharacter.numberDisplayHand;
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = currentCharacter.specialDisplayHand;
                    }
                    break;
                case TargetCharacter.OpponentOfPlayer:
                    CharacterInstance waitingCharacter = CardGameManager.Instance.WaitingCharacter;
                    if (cardType == CardType.Number)
                    {
                        SelectableCards = waitingCharacter.numberDisplayHand;
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = waitingCharacter.specialDisplayHand;
                    }
                    break;
            }
            SelectedCards = new List<DisplayCard>();

            if (SelectingCharacter == CardGameManager.Instance.player)
                ToggleCardsSelectable(true);
        }
    }

    public void SelectCard(DisplayCard card, CharacterInstance selectingPlayer)
    {
        if(SelectedCards.Contains(card))
        {
            CardGameLog.Instance.AddToLog(selectingPlayer.character.name + " unselects card: " + card.baseCard.name);
            //Debug.Log("Unselect Card: " + card.baseCard.name);
            SelectedCards.Remove(card);
            card.ToggleSelectionColor(false);
        }
        else if(SelectedCards.Count < CurrSettings.numCards)
        {
            CardGameLog.Instance.AddToLog(selectingPlayer.character.name + " selects card: " + card.baseCard.name);
            //Debug.Log("Select Card: " + card.baseCard.name);
            SelectedCards.Add(card);
            card.ToggleSelectionColor(true);
        }
    }

    public void EndSelectingCards()
    {
        CardGameManager.Instance.EndSelection();
        this.SelectingCharacter = null;
        SelectingCards = false;
        ToggleCardsSelectable(false);
    }

    public bool CheckConditionsMet()
    {
        bool conditionsMet = SelectedCards.Count == CurrSettings.numCards;
        CardGameUIManager.Instance.ToggleSelectConfirmButton(conditionsMet);

        return conditionsMet;
    }

    public int GetNumberOfCardsToSelect()
    {
        if (CurrSettings.numCards == 0) return 1;
        return CurrSettings.numCards;
    }

    void ToggleCardsSelectable(bool on)
    {
        foreach(DisplayCard card in SelectableCards)
        {
            card.SelectButton.interactable = on;
        }
    }

    public List<DisplayCard> GetSelectableCards()
    {
        return SelectableCards;
    }

    public void ConfirmSelection()
    {
        if(CheckConditionsMet())
        {
            switch (CurrSettings.selectionPurpose)
            {
                case Effect.Discard:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.DiscardCard(card);
                    }
                    break;
                case Effect.Swap:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.SwapCard(card);
                    }
                    CardGameManager.Instance.EndSwap();
                    break;
                case Effect.Flip:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.FlipCard(card);
                    }
                    CardGameManager.Instance.EndFlip();
                    break;
                case Effect.Change:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CharacterInstance cardOwner = card.owner;
                        CardGameManager.Instance.DiscardCard(card);
                        cardOwner.AddValue(CurrSettings.miscValue);
                    }
                    break;
                case Effect.Duplicate:
                    foreach(DisplayCard card in SelectedCards)
                    {
                        card.owner.AddValue(card.value);
                    }
                    break;
            }
            foreach(DisplayCard card in SelectedCards)
            {
                card.ToggleSelectionColor(false);
            }
            EndSelectingCards();
        }
    }
}
