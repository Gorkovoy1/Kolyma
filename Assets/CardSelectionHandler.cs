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

    private void Awake()
    {
        Instance = this;
    }

    public void StartSelectingCards(CardSelectSettings cardSelectSettings)
    {
        Debug.Log("START SELECTING CARDS!");
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
        ToggleCardsSelectable(true);
        CardGameManager.Instance.UIToggleSelectionMode(true);
    }

    public void SelectCard(DisplayCard card)
    {
        if(SelectedCards.Contains(card))
        {
            //Debug.Log("Unselect Card: " + card.baseCard.name);
            SelectedCards.Remove(card);
            card.ToggleSelectionColor(false);
        }
        else if(SelectedCards.Count < CurrSettings.numCards)
        {
            //Debug.Log("Select Card: " + card.baseCard.name);
            SelectedCards.Add(card);
            card.ToggleSelectionColor(true);
        }
    }

    public void EndSelectingCards()
    {
        CardGameManager.Instance.UIToggleSelectionMode(false);
        SelectingCards = false;
        ToggleCardsSelectable(false);
    }

    public bool CheckConditionsMet()
    {
        return SelectedCards.Count == CurrSettings.numCards;
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
