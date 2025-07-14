using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeckManagerController : MonoBehaviour
{

    public TextMeshProUGUI cardCount;
    public RectTransform deckManagerPanel;

    private Vector2 startPos;
    private Vector2 endPos;

    public bool finishDeck;
    public GameObject sfxObj;

    // Start is called before the first frame update
    void Start()
    {
        CardInventoryController.instance.ManageDeck();
        startPos = deckManagerPanel.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        cardCount.text = "" + CardInventoryController.instance.playerDeck.Count + " /15";
    }
    
    public void ShowCanvas()
    {
        //lerp deckmanagerpanel from -450 to 0 and back
        
        StartCoroutine(LerpPullPanel());
    }

    public void TutorialFinishDeck()
    {
        if(CardInventoryController.instance.playerDeck.Count == 6)
        {
            ShowCanvas();
            finishDeck = true;
        }
    }

    public void FinishDeck()
    {
        ShowCanvas();
        finishDeck = true;
        //prevent changes to deck mid game
    }

    public IEnumerator LerpPullPanel()
    {
        if(deckManagerPanel.anchoredPosition.y < 0)
        {
            endPos = new Vector2(startPos.x, 0f);
            AkSoundEngine.PostEvent("Play_Open_Trick_Collection", sfxObj);
        }
        else
        {
            endPos = new Vector2(startPos.x, -450f);
            AkSoundEngine.PostEvent("Play_Close_Trick_Collection", sfxObj);
        }

        for (float t = 0; t < 0.45f; t += Time.deltaTime)
        {
            float progress = t / 0.45f;
            deckManagerPanel.anchoredPosition = Vector2.Lerp(deckManagerPanel.anchoredPosition, endPos, progress);
            yield return null;
        }
        deckManagerPanel.anchoredPosition = endPos;
    }
    
}
