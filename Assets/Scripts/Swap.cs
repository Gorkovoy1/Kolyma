using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{

    public GameManager gm;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Swap5()
    {
        // Loop through the list of game objects
        foreach (var gameObject in gm.AIValues)
        {
            // Check if the game object has the component
            if (gameObject.GetComponent<ValueCard>().value == 5)
            {
                // Remove the game object from the list
                gm.AIValues.Remove(gameObject);
                Destroy(gameObject);
                GameObject nextCard = (GameObject) Instantiate (gm.valueDeck[0]);
                gm.AIValues.Add(nextCard);
                gm.valueDeck.RemoveAt(0);
                gm.OrganizeCards();
                gm.cardsSwapped+= 1;
                break;
            }
        }
        DestroyMe();
    }

    public void SwapHighest()
    {
        int highestValue = int.MinValue;
        GameObject highestObject = null;
        
        List<GameObject> combinedList = new List<GameObject>();
        combinedList.AddRange(gm.AIValues);
        combinedList.AddRange(gm.tempNegsAI);

        foreach (GameObject obj in combinedList)
        {
            // Get the integer component of the current game object
            int value = obj.GetComponent<ValueCard>().value;

            // Check if this integer value is the highest one found so far
            if (value > highestValue)
            {
                highestValue = value;
                highestObject = obj;
            }
        }
        gm.AIValues.Remove(highestObject);
        combinedList.Remove(highestObject);
        Destroy(highestObject);
        GameObject nextCard = (GameObject) Instantiate (gm.valueDeck[0]);
        gm.AIValues.Add(nextCard);
        gm.valueDeck.RemoveAt(0);
        gm.OrganizeCards();
        gm.cardsSwapped += 1;

        DestroyMe();
        //at a certain point it says gameobject has been destroyed but youre stil trying to access it
    }

    void DestroyMe()
    {
        gm.playerSpecials.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
