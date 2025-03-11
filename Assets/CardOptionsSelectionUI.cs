using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardOptionsSelectionUI : MonoBehaviour
{
    public static CardOptionsSelectionUI Instance;

    public GameObject MainObject;
    public GameObject DisplayCardPrefab;
    public RectTransform CardOrganizer;
    public GameObject FadeBG;
    public GameObject HideButton;

    public bool Active;

    public List<DisplayCard> CurrentCards;

    private void Awake()
    {
        Instance = this;
    }

    public void FillInCards(List<DisplayCard> cards, CharacterInstance cardPlayer, int numberOfCards, Effect effect, NumberOfCardsQuantifier quantifier)
    {
        Debug.Log("Fill In Cards!");
        Active = true;
        MainObject.SetActive(true);
        CardOrganizer.gameObject.SetActive(true);
        FadeBG.gameObject.SetActive(true);

        HideButton.SetActive(true);

        List<DisplayCard> cardList = new List<DisplayCard>();
        for (int i = 0; i < cards.Count; i++)
        {
            if(cards[i].SpecialCard != null)
            {
                GameObject cardOption = Instantiate(DisplayCardPrefab, CardOrganizer);
                cardList.Add(cardOption.GetComponent<DisplayCard>());
                cardOption.GetComponent<DisplayCard>().InitSpecialCard(cards[i].SpecialCard, cardPlayer);
            }
        }
        CurrentCards = cardList;

        CardSelectSettings newSettings = new CardSelectSettings(cardList, numberOfCards, cardPlayer, TargetCharacter.None, true, 0, effect, quantifier);
        CardGameManager.Instance.cardSelectStack.Push(newSettings);
        CardGameManager.Instance.StartSelecting();
    }

    public void FillInCards(List<SpecialDeckCard> cards, CharacterInstance cardPlayer, int numberOfCards, Effect effect, NumberOfCardsQuantifier quantifier)
    {
        Active = true;
        MainObject.SetActive(true);
        CardOrganizer.gameObject.SetActive(true);
        FadeBG.gameObject.SetActive(true);

        HideButton.SetActive(true);

        List<DisplayCard> cardList = new List<DisplayCard>();
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardOption = Instantiate(DisplayCardPrefab, CardOrganizer);
            cardList.Add(cardOption.GetComponent<DisplayCard>());
            cardOption.GetComponent<DisplayCard>().InitSpecialCard(cards[i], cardPlayer);
        }
        CurrentCards = cardList;

        CardSelectSettings newSettings = new CardSelectSettings(cardList, numberOfCards, cardPlayer, TargetCharacter.None, true, 0, effect, quantifier);
        CardGameManager.Instance.cardSelectStack.Push(newSettings);
        CardGameManager.Instance.StartSelecting();
    }

    public void ToggleHide()
    {
        MainObject.SetActive(!MainObject.activeSelf);
    }

    public void EndSelection()
    {
        Debug.Log("End Selection!");
        Active = false;
        CardOrganizer.gameObject.SetActive(false);
        FadeBG.gameObject.SetActive(false);
        MainObject.SetActive(false);
        HideButton.SetActive(false);
        for(int i = 0; i < CurrentCards.Count; i++)
        {
            CardGameManager.Instance.RemoveCardFromPlay(CurrentCards[i], false);
        }
        CurrentCards.Clear();
    }
}
