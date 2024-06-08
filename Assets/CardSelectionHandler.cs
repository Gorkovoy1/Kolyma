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
        if(selectingCharacter == CardGameManager.Instance.player)
            CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerSelecting);

        if (!SelectingCards && CardGameManager.Instance.cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = CardGameManager.Instance.cardSelectStack.Pop();
            StartSelectingCards(curr);
        }
    }

    public void StartSelectingCards(CardSelectSettings cardSelectSettings)
    {
        SelectingCards = true;
        CurrSettings = cardSelectSettings;

        CardType cardType = cardSelectSettings.cardType;
        CharacterInstance targetCharacter = cardSelectSettings.targetCharacter;

        if(cardType == CardType.Number)
        {
            SelectableCards = targetCharacter.numberDisplayHand;
        }
        else if(cardType == CardType.Special)
        {
            SelectableCards = targetCharacter.specialDisplayHand;
        }
        SelectedCards = new List<DisplayCard>();

        if(SelectingCharacter == CardGameManager.Instance.player)
            ToggleCardsSelectable(true);

        Debug.Log(targetCharacter.character.name + " starts selecting cards!");
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
            }
            foreach(DisplayCard card in SelectedCards)
            {
                card.ToggleSelectionColor(false);
            }
            EndSelectingCards();
        }
    }
}
