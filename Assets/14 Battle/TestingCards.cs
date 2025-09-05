using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCards : MonoBehaviour
{
    public int value;
    public string target;
    public GameObject AIHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        TurnManager.instance.isPlayerTurn = !TurnManager.instance.isPlayerTurn;
    }
}
