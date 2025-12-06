using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public List<GameObject> playerSpecialHand;
    public List<GameObject> opponentSpecialHand;

    public List<GameObject> playerSpecialDeck;
    public List<GameObject> opponentSpecialDeck;

    public GameObject playerHand;
    public GameObject opponentHand;

    public Transform imagesParent;

    public bool startGame = false;

    public GameObject endTurnButton;
    public GameObject actionButton;

    public GameObject sfxObj;

    public GameObject oppDiscardButton;
    public GameObject playerDiscardButton;

    public PassAnimationController passAnimationController;

    // Start is called before the first frame update
    void Start()
    {

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(startGame)
        {
            startGame = false;
            //set action button and end button active
            actionButton.SetActive(true);
            endTurnButton.SetActive(true);

            playerSpecialDeck = CardInventoryController.instance.playerDeck;
            ShuffleSpecials();
        }
    }

    void ShuffleSpecials()
    {
        for (int i = playerSpecialDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = playerSpecialDeck[i];
            playerSpecialDeck[i] = playerSpecialDeck[j];
            playerSpecialDeck[j] = temp;
        }

        for (int i = opponentSpecialDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = opponentSpecialDeck[i];
            opponentSpecialDeck[i] = opponentSpecialDeck[j];
            opponentSpecialDeck[j] = temp;
        }

        opponentSpecialDeck = opponentSpecialDeck.GetRange(0, Mathf.Min(10, opponentSpecialDeck.Count));

        //start draw
        StartCoroutine(InitialPlayerDraw());
        StartCoroutine(InitialOpponentDraw());
    }

    public void UpdateHands()
    {
        playerSpecialHand = new List<GameObject>();
        opponentSpecialHand = new List<GameObject>();

        foreach (Transform child in playerHand.transform)
        {
            playerSpecialHand.Add(child.gameObject);
        }

        foreach (Transform child in opponentHand.transform)
        {
            opponentSpecialHand.Add(child.gameObject);
        }
    }

    public void DrawToHand(string target)
    {
        if (target == "player")
        {
            if (playerSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty");
                
            }
            else
            {
                Instantiate(playerSpecialDeck[0], playerHand.transform);
                playerSpecialDeck.RemoveAt(0);
            }

            
        }
        else if (target == "opponent")
        {
            if (opponentSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty");
            }
            else
            {
                Instantiate(opponentSpecialDeck[0], opponentHand.transform);
                opponentSpecialDeck.RemoveAt(0);
            }

            
        }

        AkSoundEngine.PostEvent("Play_Trick_Card", sfxObj);
        UpdateHands();
    }

    IEnumerator InitialOpponentDraw()
    {
        yield return new WaitForSeconds(1.3f);
        for (int i = 0; i < 6; i++)
        {
            if (opponentSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more cards
            }

            Instantiate(opponentSpecialDeck[0], opponentHand.transform);
            opponentSpecialDeck.RemoveAt(0);

            AkSoundEngine.PostEvent("Play_Trick_Card", sfxObj);
            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
        UpdateHands();
    }

    IEnumerator InitialPlayerDraw()
    {
        yield return new WaitForSeconds(1.3f);
        for (int i = 0; i < 6; i++)
        {
            if (playerSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more cards
            }

            Instantiate(playerSpecialDeck[0], playerHand.transform);
            playerSpecialDeck.RemoveAt(0);

            AkSoundEngine.PostEvent("Play_Trick_Card", sfxObj);
            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
        UpdateHands();
    }

}
