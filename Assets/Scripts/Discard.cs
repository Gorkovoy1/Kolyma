using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discard : MonoBehaviour
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

    public void DiscardSpecial()
    {
        if(gm.AISpecials != null && gm.AISpecials.Count > 0)
        {
            int randomIndex = Random.Range(0, gm.AISpecials.Count);
            GameObject tempPrefab = gm.AISpecials[randomIndex];
            gm.AISpecials.RemoveAt(randomIndex);
            Destroy(tempPrefab);
            gm.hasDiscarded = true;
            gm.cardsDiscarded += 1;
        }
        else
        {
            Debug.Log("The opponent's hand is empty!");
        }
    }

    public void ifNegativeDiscard()
    {
        if(gm.tempNegsPlayer.Count > 0)
        {
            if(gm.AISpecials != null && gm.AISpecials.Count > 0)
            {
                int randomIndex = Random.Range(0, gm.tempNegsPlayer.Count);
                GameObject tempPrefab = gm.AISpecials[randomIndex];
                gm.AISpecials.RemoveAt(randomIndex);
                Destroy(tempPrefab);
                gm.hasDiscarded = true;
                gm.cardsDiscarded += 1;
            }
            else
            {
                Debug.Log("The opponent's hand is empty!");
            }
        }
        else
        {
            Debug.Log("No negative!");
        }
    }

    public void if8DiscsardSpecial()
    {
        GameObject eight = gm.AIValues.Find(obj => obj.GetComponent<ValueCard>().value == 8);
        if(eight != null)
        {
            DiscardSpecial();
            gm.cardsDiscarded += 1;
        }
    }
}
