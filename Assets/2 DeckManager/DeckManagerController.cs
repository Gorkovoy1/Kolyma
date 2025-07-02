using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckManagerController : MonoBehaviour
{

    public TextMeshProUGUI cardCount;
    public RectTransform deckManagerPanel;
    public Button expandButton;

    // Start is called before the first frame update
    void Start()
    {
        CardInventoryController.instance.ManageDeck();
    }

    // Update is called once per frame
    void Update()
    {
        cardCount.text = "" + CardInventoryController.instance.playerDeck.Count + " /15";
    }

    public void PullCanvas()
    {
        if(deckManagerPanel.anchoredPosition.y > 0)
        {
            deckManagerPanel.anchoredPosition = new Vector2(deckManagerPanel.anchoredPosition.x, deckManagerPanel.anchoredPosition.y - 400f);
            expandButton.GetComponentInChildren<TextMeshProUGUI>().text = "^";
        }
        else
        {
            deckManagerPanel.anchoredPosition = new Vector2(deckManagerPanel.anchoredPosition.x, deckManagerPanel.anchoredPosition.y + 400f);
            expandButton.GetComponentInChildren<TextMeshProUGUI>().text = "V";
        }
    }
}
