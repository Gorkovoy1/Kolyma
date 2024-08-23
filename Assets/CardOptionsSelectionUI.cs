using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardOptionsSelectionUI : MonoBehaviour
{
    public static CardOptionsSelectionUI Instance;

    public GameObject DisplayCardPrefab;
    public RectTransform CardOrganizer;
    public GameObject FadeBG;

    public bool Active;

    private void Awake()
    {
        Instance = this;
    }

    public void FillInCards(List<SpecialDeckCard> cards, CharacterInstance cardPlayer, int numberOfCards)
    {
        Debug.Log("Fill In Cards!");
        Active = true;
        CardOrganizer.gameObject.SetActive(true);
        FadeBG.gameObject.SetActive(true);

        List<DisplayCard> cardList = new List<DisplayCard>();
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardOption = Instantiate(DisplayCardPrefab, CardOrganizer);
            cardList.Add(cardOption.GetComponent<DisplayCard>());
            cardOption.GetComponent<DisplayCard>().InitSpecialCard(cards[i], cardPlayer);
        }

        CardSelectSettings newSettings = new CardSelectSettings(cardList, numberOfCards, cardPlayer, TargetCharacter.None, true, 0, Effect.PlayCard);
        CardGameManager.Instance.cardSelectStack.Push(newSettings);
        CardGameManager.Instance.StartSelecting();
    }

    public void EndSelection()
    {
        Debug.Log("End Selection!");
        Active = false;
        CardOrganizer.gameObject.SetActive(false);
        FadeBG.gameObject.SetActive(false);
    }
}
