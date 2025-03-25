using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum UIMode { PlayerTurn, OpponentTurn, PlayerSelecting}

public class CardGameUIManager : MonoBehaviour
{
    public static CardGameUIManager Instance;

    private CharacterInstance player, opponent;
    public Image PlayerPortrait, OpponentPortrait;

    public GameObject CardPlayZone, DiscardZone;

    public Button endTurnButton, selectConfirmButton;
    public NumberCardOrganizer OpponentPositiveCardZone, PlayerPositiveCardZone, OpponentNegativeCardZone, PlayerNegativeCardZone;
    public Transform PlayerHandTransform, OpponentHandTransform;
    public TextMeshProUGUI opponentSumText, playerSumText, targetValueText, selectionText, EndTurnText;
    public TextMeshProUGUI PlayerNameText, OpponentNameText;

    public TextMeshProUGUI FlipText, SwapText, MulliganText; 
    public Button FlipButton, SwapButton, MulliganButton;

    private UIMode CurrentMode;

    public bool Swapping, Flipping, Mulliganing;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(CharacterInstance player, CharacterInstance opponent)
    {
        this.player = player;
        this.opponent = opponent;
        targetValueText.text = "" + 0;
        PlayerNameText.text = player.character.name;
        OpponentNameText.text = opponent.character.name;
        PlayerPortrait.sprite = player.character.portrait;
        OpponentPortrait.sprite = opponent.character.portrait;
    }

    public void UpdateValues()
    {
        opponent.currValue = 0;
        foreach (DisplayCard card in opponent.numberDisplayHand)
        {
            opponent.currValue += card.value;
        }
        player.currValue = 0;
        foreach (DisplayCard card in player.numberDisplayHand)
        {
            player.currValue += card.value;
        }
        opponentSumText.text = "" + opponent.currValue;
        playerSumText.text = "" + player.currValue;
        opponent.PositiveCardsZone.Reorganize();
        opponent.NegativeCardsZone.Reorganize();
        player.PositiveCardsZone.Reorganize();
        player.NegativeCardsZone.Reorganize();

        //EndTurnText.text = player.DidAnAction ? "End Turn" : "Pass Turn";

        bool playerTurn = CardGameManager.Instance.GameState == CardGameManager.State.PLAYERTURN;

        FlipButton.gameObject.SetActive(playerTurn);
        SwapButton.gameObject.SetActive(playerTurn);
        MulliganButton.gameObject.SetActive(playerTurn);
        endTurnButton.gameObject.SetActive(playerTurn);

        endTurnButton.interactable = !player.DidAnAction;
        FlipButton.interactable = !player.DidAnAction;
        SwapButton.interactable = !player.DidAnAction;
        MulliganButton.interactable = !player.DidAnAction;

        PlayerNameText.color = CardGameManager.Instance.GameState == CardGameManager.State.PLAYERTURN ? Color.red : Color.white;
        OpponentNameText.color = CardGameManager.Instance.GameState == CardGameManager.State.OPPONENTTURN ? Color.red : Color.white;
    }

    public void ChangeUIMode(UIMode newMode)
    {
        CurrentMode = newMode;
        switch(CurrentMode)
        {
            case UIMode.PlayerTurn:
                endTurnButton.gameObject.SetActive(true);
                selectionText.gameObject.SetActive(false);
                selectConfirmButton.gameObject.SetActive(false);
                break;
            case UIMode.OpponentTurn:
                endTurnButton.gameObject.SetActive(false);
                selectionText.gameObject.SetActive(false);
                selectConfirmButton.gameObject.SetActive(false);
                break;
            case UIMode.PlayerSelecting:
                endTurnButton.gameObject.SetActive(false);
                selectionText.gameObject.SetActive(true);
                CardSelectSettings currSettings = CardGameManager.Instance.CardSelectionHandler.CurrSettings;
                selectionText.text = "Select " + currSettings.numCards + " " + currSettings.cardType.ToString() + " " + (currSettings.numCards > 1 ? "cards" : "card") + " to " + currSettings.selectionPurpose.ToString() + ".";
                selectConfirmButton.gameObject.SetActive(true);
                break;
        }
    }

    public void ResetUI()
    {
        FlipText.text = "FLIP";
        FlipButton.interactable = true;

        SwapText.text = "SWAP";
        SwapButton.interactable = true;

        MulliganText.text = "MULLIGAN";
        MulliganButton.interactable = true;
    }

    public void ToggleFlipping(bool flipping)
    {
        Flipping = flipping;

        if (player.DidAnAction)
        {
            FlipText.text = "FLIP";
            FlipButton.interactable = false;
        }
        else
        {
            FlipText.text = Flipping ? "FLIPPING..." : "FLIP";
        }
    }

    public void ToggleSwapping(bool swapping)
    {
        Swapping = swapping;

        if(player.DidAnAction)
        {
            SwapText.text = "SWAP";
            SwapButton.interactable = false;
        }
        else
        {
            SwapText.text = Swapping ? "SWAPPING..." : "SWAP";
        }
    }

    public void ToggleMulliganing(bool mulliganing)
    {
        Mulliganing = mulliganing;

        if (player.DidAnAction)
        {
            MulliganText.text = "MULLIGAN";
            MulliganButton.interactable = false;
        }
        else
        {
            MulliganText.text = Mulliganing ? "Mulliganing..." : "MULLIGAN";
        }
    }

    public void ToggleSelectConfirmButton(bool toggle)
    {
        selectConfirmButton.interactable = toggle;
    }
}
