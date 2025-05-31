using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCards : MonoBehaviour
{
    public int value;
    public string target;

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
}
