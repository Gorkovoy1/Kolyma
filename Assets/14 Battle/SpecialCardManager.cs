using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCardManager : MonoBehaviour
{
    private Transform parentObj;

    public static SpecialCardManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DiscardSpecialCard()
    {
        //
    }

    public void Give(int value, string target)            //testing with random values
    {
        

        if(target == "opponent")
        {
            if(value < 0)
            {
                parentObj = CardPlacementController.instance.opponentNegativeArea;
            }
            else
            {
                parentObj = CardPlacementController.instance.opponentPositiveArea;
            }

        }
        else if(target == "player")
        {
            if (value < 0)
            {
                parentObj = CardPlacementController.instance.playerNegativeArea;
            }
            else
            {
                parentObj = CardPlacementController.instance.playerPositiveArea;
            }
        }

        //////////////////////////////
        

        if (value == -5)
        {
            Instantiate(CardPlacementController.instance.negFiveSpace, parentObj);
        }
        if (value == -4)
        {
            Instantiate(CardPlacementController.instance.negFourSpace, parentObj);
        }
        if (value == -3)
        {
            Instantiate(CardPlacementController.instance.negThreeSpace, parentObj);
        }
        if (value == -2)
        {
            Instantiate(CardPlacementController.instance.negTwoSpace, parentObj);
        }
        if (value == 2)
        {
            Instantiate(CardPlacementController.instance.twoSpace, parentObj);
        }
        if (value == 3)
        {
            Instantiate(CardPlacementController.instance.threeSpace, parentObj);
        }
        if (value == 4)
        {
            Instantiate(CardPlacementController.instance.fourSpace, parentObj);
        }
        if (value == 5)
        {
            Instantiate(CardPlacementController.instance.fiveSpace, parentObj);
        }
        if (value == 6)
        {
            Instantiate(CardPlacementController.instance.sixSpace, parentObj);
        }
        if (value == 7)
        {
            Instantiate(CardPlacementController.instance.sevenSpace, parentObj);
        }
        if (value == 8)
        {
            Instantiate(CardPlacementController.instance.eightSpace, parentObj);
        }
        if (value == 9)
        {
            Instantiate(CardPlacementController.instance.nineSpace, parentObj);
        }
    }

    public void ActivateChoice(int one, int two, string target)
    {
        //show two buttons, choice one and choice two
        //on click instantiate right card for right target depending on pos or neg
    }

    public void PlayCaughtRedHanded()
    {
        //if opponent has yellow numbers
        //activate swap
        //make all yellow numbers cickable and outlined
        //wait for something to return click, then take that click object and use it for next code line
        //swap it - aka delete it and deal another card
    }

    public void PlayEmptyPockets()
    {
        //if opponent has blue number
        //discard random special card of theirs
    }

    public void PlayBurden()
    {
        //if opponent has red
        //activate choice
        //on click give +4 or -4
    }

    public void PlayRifleButt()
    {
        //if opponent has red
        //make all numbers clickable and outlined
        //allow selected to flip - change parent and rotate 180
    }

    public void PlaySmokeBreak()
    {
        //access discard pile
        //draw 2 random special cards and add to hand
    }

    public void PlayWeakness()
    {
        //check if opponent has 2
        //draw random specail card from discard
    }

    public void PlayThickWoolenCoat()
    {
        //activate choice -2 or +2
        //on click give to opponent in right place
    }

    public void PlaySetup()
    {
        //if opponent has negative
        //allow all numbers to be clickable
        //on click discard
    }

    public void PlayBribe()
    {
        //check if have duplicate
        //if have, give to opponent in corresponding place
    }

    public void PlayFist()
    {
        //find all blue numbers on table, swap them all out - replace each one one by one
    }

    public void PlayCondensedMilk()
    {
        //
    }
}
