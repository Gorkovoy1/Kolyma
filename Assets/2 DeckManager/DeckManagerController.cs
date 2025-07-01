using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckManagerController : MonoBehaviour
{

    public TextMeshProUGUI cardCount;

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
}
