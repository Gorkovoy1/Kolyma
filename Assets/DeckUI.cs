using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckUI : MonoBehaviour
{
    public bool Player;
    public GameObject Tooltip;
    public TextMeshProUGUI CardCountText;

    public void ShowInfo()
    {
        Tooltip.SetActive(true);
        int cardCount = Player ? CardGameManager.Instance.player.deck.Count : CardGameManager.Instance.opponent.deck.Count;
        CardCountText.text = "Cards: " + cardCount;
    }

    public void HideInfo()
    {
        Tooltip.SetActive(false);
    }
}
