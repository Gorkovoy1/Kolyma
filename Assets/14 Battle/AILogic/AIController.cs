using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AIController : MonoBehaviour
{

    public GameObject selectedCardToPlay;
    public Difficulty difficulty;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DelayAction()
    {
        yield return new WaitForSeconds(1.5f);
    }

    public void PlayCard()
    {
        StartCoroutine(DelayAction());
        var picker = GetComponent<AICardPicker>();
        var cards = picker.playableCards;

        TurnManager.instance.opponentPlayedCard = true;

        var selected = selectedCardToPlay.GetComponent<AICardPlace>();
        selected.difficulty = difficulty;

        //adding value, check if will bust
        if (selected.selfValue != 0)
        {
            int projected = NumberManager.instance.oppVal + selected.selfValue;

            if (projected <= NumberManager.instance.targetVal)
            {
                //wont bust so play
                selected.AnimateBeingPlayed();
                return;
            }

            //find other card to play
            Debug.Log("Current card would bust, checking alternatives…");

            foreach (var c in cards)
            {
                var card = c.GetComponent<AICardPlace>();
                if (card.selfValue == 0 ||
                    NumberManager.instance.oppVal + card.selfValue <= NumberManager.instance.targetVal)
                {
                    selectedCardToPlay = c;
                    card.difficulty = difficulty;
                    card.AnimateBeingPlayed();
                    return;
                }
            }

            
            Debug.Log("All cards would bust — skipping turn.");
            StartCoroutine(DelaySkipTurn());
            return;
        }

        //value is 0, safe card
        selected.AnimateBeingPlayed();
    }

    IEnumerator DelaySkipTurn()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.5f); //account for delay to read card text that was added
        TurnManager.instance.opponentPlayedCard = false;
        TurnManager.instance.opponentPassed = true;
        this.gameObject.GetComponent<AICardPicker>().passAnimationController.oppPass = true;
        StartCoroutine(this.gameObject.GetComponent<AICardPicker>().DelayTurn());

    }
}



