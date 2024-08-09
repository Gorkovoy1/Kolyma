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

    public GameObject CardPlayZone, DiscardZone;

    public Button endTurnButton, selectConfirmButton;
    public NumberCardOrganizer OpponentPositiveCardZone, PlayerPositiveCardZone, OpponentNegativeCardZone, PlayerNegativeCardZone;
    public Transform PlayerHandTransform, OpponentHandTransform;
    public TextMeshProUGUI opponentSumText, playerSumText, targetValueText, selectionText;

    public TextMeshProUGUI FlipText, SwapText; 
    public Button FlipButton, SwapButton;

    private UIMode CurrentMode;

    public bool Swapping, Flipping;

    private void Awake()
    {
        Instance = this;
    }

    public void Init(CharacterInstance player, CharacterInstance opponent)
    {
        this.player = player;
        this.opponent = opponent;
    }

    private void Update()
    {
        
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
        opponentSumText.text = opponent.character.name + ":\n" + opponent.currValue;
        playerSumText.text = player.character.name + ":\n" + player.currValue;
        opponent.PositiveCardsZone.Reorganize();
        opponent.NegativeCardsZone.Reorganize();
        player.PositiveCardsZone.Reorganize();
        player.NegativeCardsZone.Reorganize();
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
    }

    public void ToggleFlipping(bool flipping)
    {
        Flipping = flipping;

        if (player.DidAnAction)
        {
            FlipText.text = "Flipped";
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
            SwapText.text = "Swapped";
            SwapButton.interactable = false;
        }
        else
        {
            SwapText.text = Swapping ? "SWAPPING..." : "SWAP";
        }
    }

    public void ToggleSelectConfirmButton(bool toggle)
    {
        selectConfirmButton.interactable = toggle;
    }
}
