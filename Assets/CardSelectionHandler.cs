using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardSelectionHandler : MonoBehaviour
{
    public CardSelectSettings CurrSettings;

    public bool SelectingCards;

    private bool SelectConditionsMet;

    public List<DisplayCard> SelectableCards;

    public List<DisplayCard> SelectedCards;

    public static CardSelectionHandler Instance;

    public CharacterInstance SelectingCharacter;

    private List<DisplayCard> stagedTradeCards;

    private void Awake()
    {
        Instance = this;
    }

    public DisplayCard ChooseRandomCard(CardType cardType, CharacterInstance targetCharacter)
    {
        DisplayCard[] possibleCards = new DisplayCard[0];
        switch(cardType)
        {
            case CardType.Number:
                possibleCards = targetCharacter.numberDisplayHand.ToArray();
                break;
            case CardType.Special:
                possibleCards = targetCharacter.specialDisplayHand.ToArray();
                break;
        }
        int chosenIndex = Random.Range(0, targetCharacter.numberDisplayHand.Count);
        return possibleCards[chosenIndex];
    }


    public void ProcessSelect()
    {
        if (!SelectingCards && CardGameManager.Instance.cardSelectStack.Count > 0)
        {
            CardSelectSettings curr = CardGameManager.Instance.cardSelectStack.Pop();
            StartSelectingCards(curr);
            SelectingCharacter = curr.selector;
            if (SelectingCharacter == CardGameManager.Instance.player)
                CardGameUIManager.Instance.ChangeUIMode(UIMode.PlayerSelecting);
            if (SelectingCharacter.IsAI)
            {
                SelectingCharacter.AIScript.SetState(AIState.SelectingCard);
            }

            if(SelectingCharacter.Opponent.IsAI)
            {
                SelectingCharacter.Opponent.AIScript.SetState(AIState.WaitingForOpponent);
            }
        }
    }

    public void StartSelectingCards(CardSelectSettings cardSelectSettings)
    {
        SelectingCards = true;
        CurrSettings = cardSelectSettings;
        if (cardSelectSettings.SpecificCards != null)
        {
            SelectableCards = cardSelectSettings.SpecificCards;
            if(cardSelectSettings.target == TargetCharacter.All)
            {
                SelectedCards = SelectableCards;
                ConfirmSelection(true);
                return;
            }
        }
        else
        {
            Debug.Log("Not Specific Card Selectable");
            CardType cardType = cardSelectSettings.cardType;
            switch (cardSelectSettings.target)
            {
                case TargetCharacter.None:
                    break;
                case TargetCharacter.Anyone:
                    if (cardType == CardType.Number || cardType == CardType.PositiveNumber || cardType == CardType.NegativeNumber)
                    {
                        SelectableCards = new List<DisplayCard>(CardGameManager.Instance.CurrentCharacter.numberDisplayHand);
                        SelectableCards.AddRange(CardGameManager.Instance.WaitingCharacter.numberDisplayHand);
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = new List<DisplayCard>(CardGameManager.Instance.CurrentCharacter.specialDisplayHand);
                        SelectableCards.AddRange(CardGameManager.Instance.WaitingCharacter.specialDisplayHand);
                    }
                    break;
                case TargetCharacter.PlayerOfCard:
                    CharacterInstance currentCharacter = CardGameManager.Instance.CurrentCharacter;
                    if (cardType == CardType.Number || cardType == CardType.PositiveNumber || cardType == CardType.NegativeNumber)
                    {
                        SelectableCards = new List<DisplayCard>(currentCharacter.numberDisplayHand);
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = new List<DisplayCard>(currentCharacter.specialDisplayHand);
                    }
                    break;
                case TargetCharacter.OpponentOfPlayer:
                    CharacterInstance waitingCharacter = CardGameManager.Instance.WaitingCharacter;
                    if (cardType == CardType.Number || cardType == CardType.PositiveNumber || cardType == CardType.NegativeNumber)
                    {
                        SelectableCards = new List<DisplayCard>(waitingCharacter.numberDisplayHand);
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectableCards = new List<DisplayCard>(waitingCharacter.specialDisplayHand);
                    }
                    break;
                case TargetCharacter.All:
                    CharacterInstance currCharacter = CardGameManager.Instance.CurrentCharacter;
                    CharacterInstance waitCharacter = CardGameManager.Instance.WaitingCharacter;
                    SelectedCards = new List<DisplayCard>();
                    if (cardType == CardType.Number || cardType == CardType.PositiveNumber || cardType == CardType.NegativeNumber)
                    {
                        SelectedCards.AddRange(currCharacter.numberDisplayHand);
                        SelectedCards.AddRange(waitCharacter.numberDisplayHand);
                    }
                    else if (cardType == CardType.Special)
                    {
                        SelectedCards.AddRange(currCharacter.specialDisplayHand);
                        SelectedCards.AddRange(waitCharacter.specialDisplayHand);
                    }
                    ConfirmSelection(true);
                    return;
                    break;
            }

            if (cardType == CardType.Number && cardSelectSettings.cardClass != NumberClass.NONE)
            {
                SelectableCards.RemoveAll(s => ((NumberCard)s.baseCard).cardClass != cardSelectSettings.cardClass);
            }

            if(cardType == CardType.PositiveNumber)
            {
                SelectableCards.RemoveAll(s => ((NumberCard)s.baseCard).value < 0);
            }
            else if(cardType == CardType.NegativeNumber)
            {
                SelectableCards.RemoveAll(s => ((NumberCard)s.baseCard).value > 0);
            }


            //if (cardType == CardType.Number)
            //    SelectableCards = from card in SelectableCards where ((NumberCard)card.baseCard).cardClass == cardSelectSettings.
        }

        if (cardSelectSettings.target != TargetCharacter.All)
        {
            SelectedCards = new List<DisplayCard>();

            ToggleCardsSelectable(true);
        }
    }

    public void SelectCard(DisplayCard card, CharacterInstance selectingPlayer)
    {
        if (SelectedCards.Contains(card))
        {
            //CardGameLog.Instance.AddToLog(selectingPlayer.character.name + " unselects card: " + card.baseCard.name);
            //Debug.Log("Unselect Card: " + card.baseCard.name);
            SelectedCards.Remove(card);
            card.ToggleSelectionColor(false);
        }
        else if (SelectedCards.Count < CurrSettings.numCards)
        {
            //CardGameLog.Instance.AddToLog(selectingPlayer.character.name + " selects card: " + card.baseCard.name);
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
        bool conditionsMet = true;
        switch(CurrSettings.quantifier)
        {
            case NumberOfCardsQuantifier.EqualTo:
                conditionsMet = SelectedCards.Count == CurrSettings.numCards;
                break;
            case NumberOfCardsQuantifier.LessThanOrEqualTo:
                conditionsMet = SelectedCards.Count <= CurrSettings.numCards;
                break;
        }
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
        foreach (DisplayCard card in SelectableCards)
        {
            if(card != null)
                card.SelectButton.interactable = on;
        }
    }

    public List<DisplayCard> GetSelectableCards()
    {
        return SelectableCards;
    }

    public void ConfirmSelection(bool forced = false)
    {
        bool confirmed = false;

        if (CardOptionsSelectionUI.Instance.Active)
            CardOptionsSelectionUI.Instance.EndSelection();

        if(forced)
        {
            confirmed = true;
        }
        else if(CheckConditionsMet())
        {
            confirmed = true;
        }

        if (confirmed)
        {
            switch (CurrSettings.selectionPurpose)
            {
                case Effect.PlayCard:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.PlayCard(card);
                    }
                    break;
                case Effect.Discard:
                    SelectedCards[0].owner.NumberOfNewlyDiscardCards = SelectedCards.Count;
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
                        CardGameManager.Instance.ChangeCard(card, CurrSettings.miscValue);
                    }
                    break;
                case Effect.Duplicate:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.DuplicateCard(card);
                    }
                    break;
                case Effect.Give:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.GiveCard(card);
                    }
                    break;
                case Effect.Steal:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.StealCard(card);
                    }
                    break;
                case Effect.TradePart1:
                    stagedTradeCards = SelectedCards;
                    break;
                case Effect.TradePart2:
                    foreach (DisplayCard card in stagedTradeCards)
                    {
                        CardGameManager.Instance.GiveCard(card);
                    }

                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.GiveCard(card);
                    }
                    break;
                case Effect.GiveCopy:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.GiveCard(card, true);
                    }
                    break;
                case Effect.DrawFromDiscard:
                    foreach (DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.DrawFromDiscardPile(card, CurrSettings.selector);
                    }
                    break;
                case Effect.BuildDeck:
                    foreach(DisplayCard card in SelectedCards)
                    {
                        CardGameManager.Instance.player.AddCardToDeck(card);
                    }
                    CardGameManager.Instance.StartRound();
                    break;
            }
            foreach(DisplayCard card in SelectedCards)
            {
                card.ToggleSelectionColor(false);
            }
            EndSelectingCards();
            CardEffectChecker.Instance.DoNextStatement(CurrSettings.selectionPurpose != Effect.BuildDeck);
        }
    }
}
