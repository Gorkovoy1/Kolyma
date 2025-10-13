using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCardManager : MonoBehaviour
{
    private Transform parentObj;

    public static SpecialCardManager instance;

    public GameObject sfxObj;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
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
        AkSoundEngine.PostEvent("Play_Number_Card", sfxObj);
    }

    

    
}
