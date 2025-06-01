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

    // Start is called before the first frame update
    void Start()
    {
        ShuffleSpecials();
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                Debug.LogWarning("deck is empty, stopping coroutine.");
                
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
                Debug.LogWarning("deck is empty, stopping coroutine.");
            }
            else
            {
                Instantiate(opponentSpecialDeck[0], opponentHand.transform);
                opponentSpecialDeck.RemoveAt(0);
            }

            
        }


        UpdateHands();
    }

    IEnumerator InitialOpponentDraw()
    {
        for (int i = 0; i < 6; i++)
        {
            if (opponentSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more cards
            }

            Instantiate(opponentSpecialDeck[0], opponentHand.transform);
            opponentSpecialDeck.RemoveAt(0);
            

            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
        UpdateHands();
    }

    IEnumerator InitialPlayerDraw()
    {
        for (int i = 0; i < 6; i++)
        {
            if (playerSpecialDeck.Count == 0)
            {
                Debug.LogWarning("deck is empty, stopping coroutine.");
                yield break; // Stop the coroutine if there are no more cards
            }

            Instantiate(playerSpecialDeck[0], playerHand.transform);
            playerSpecialDeck.RemoveAt(0);


            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
        UpdateHands();
    }

}
