using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour
{
    public GameManager gm;
    public int integerValueToMatch;
    public GameObject newObj;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
        //GameObject tempBoard = GameObject.Find("Canvas/Panel");
        //board = tempBoard.transform; 
    }

    // Update is called once per frame
   
    public void if9FlipLowest()
    {
        GameObject nine = gm.AIValues.Find(obj => obj.GetComponent<ValueCard>().value == 9);
        if(nine != null)
        {
            List<GameObject> combinedList = new List<GameObject>();
            combinedList.AddRange(gm.AIValues);
            combinedList.AddRange(gm.tempNegsAI);

            int lowestValue = int.MaxValue;
            GameObject lowestObject = null;

            foreach (GameObject obj in combinedList)
            {
                // Get the integer component of the current game object
                int value = obj.GetComponent<ValueCard>().value;

                // Check if this integer value is the lowest one found so far
                if (value < lowestValue)
                {
                    lowestValue = value;
                    lowestObject = obj;
                }
            }

            integerValueToMatch = 0 - lowestValue;

            // Find all objects in the scene with the specified component
            ValueCard[] components = FindObjectsOfType<ValueCard>();

            if (components != null && components.Length > 0)
            {
                // Get the first object with the specified integer value for the component
                GameObject obj = GetObjectWithValueCard(components, integerValueToMatch);

                if (obj != null)
                {
                    // Instantiate the found object
                    newObj = Instantiate(obj);

                    // Use the new object however you need to in your script
                    Debug.Log("New object instantiated: " + newObj.name);
                }
                else
                {
                    Debug.Log("No object found with integer value of " + integerValueToMatch + " in ValueCard component");
                }
            }

            if(lowestObject.GetComponent<ValueCard>().value > 0)
            {
                gm.AIValues.Remove(lowestObject);
                Destroy(lowestObject);
                GameObject flipped = newObj;
                gm.tempNegsAI.Add(flipped);
                gm.OrganizeCards();
            }
            else
            {
                gm.tempNegsAI.Remove(lowestObject);
                Destroy(lowestObject);
                GameObject flipped = newObj;
                gm.AIValues.Add(flipped);
                gm.OrganizeCards();
            }
        }
        DestroyMe();
    }

    private GameObject GetObjectWithValueCard(ValueCard[] components, int valueToMatch)
    {
        foreach (ValueCard component in components)
        {
            if (component.value == valueToMatch)
            {
                return component.gameObject;
            }
        }

        return null;
    }
        
    void DestroyMe()
    {
        gm.playerSpecials.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
    
}


