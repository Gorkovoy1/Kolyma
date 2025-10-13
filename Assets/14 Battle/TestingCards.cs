using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingCards : MonoBehaviour
{
    public int value;
    public string target;
    public GameObject AIHandler;
    public bool firstTurn = false;
    public HandController handController;
    public PassAnimationController passAnimationController;
    public GameObject sfxObj;

    public bool pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(firstTurn)
        {
            firstTurn = false;
            //draw card
            handController.DrawToHand("player");
        }
        /*
        if(pressed)
        {
            pressed = false;
        }
        */
    }

    public void TutorialEndTurn()
    {
        pressed = true;
        AkSoundEngine.PostEvent("Play_Click_2", sfxObj);
        GetComponent<Button>().interactable = false;
    }

    public void TestDeal()
    {
        CardPlacementController.instance.DealOneCard(target);
    }

    public void TestGive()
    {
        SpecialCardManager.instance.Give(value, target);
    }

    public void RandomGive()
    {
        int random = Random.Range(0, 2);
        if(random == 0)
        {
            CardPlacementController.instance.DealOneCard("player");
        }
        else
        {
            CardPlacementController.instance.DealOneCard("opponent");
        }
        
        
    }

    public void CurrentTest()
    {
        //insert current testing call 

        foreach(GameObject g in NumberManager.instance.reds)
        {
            g.GetComponent<NumberStats>().selectable = true;
        }

        foreach (GameObject g in NumberManager.instance.OPPreds)
        {
            g.GetComponent<NumberStats>().selectable = true;
        }

        CardSelectionController.instance.CallButtons("discard", "opponent");
    }

    public void PlayAICard()
    {
        AIHandler.GetComponent<AIController>().PlayCard();
    }

    public void ToggleTurn()
    {
        firstTurn = true;
        TurnManager.instance.isPlayerTurn = !TurnManager.instance.isPlayerTurn;
        TurnManager.instance.checkedPlayable = false;
        if(!TurnManager.instance.playerPlayedCard)
        {
            //animate pass text
            passAnimationController.playerPass = true;
        }
        handController.DrawToHand("opponent");
        TurnManager.instance.playerPlayedCard = false;
    }
}
