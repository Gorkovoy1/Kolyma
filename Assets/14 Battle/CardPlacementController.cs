using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CardPlacementController : MonoBehaviour
{
    public GameObject twoSpace;
    public GameObject threeSpace;
    public GameObject fourSpace;
    public GameObject fiveSpace;
    public GameObject sixSpace;
    public GameObject sevenSpace;
    public GameObject eightSpace;
    public GameObject nineSpace;
    public GameObject negTwoSpace;
    public GameObject negThreeSpace;
    public GameObject negFourSpace;
    public GameObject negFiveSpace;

    public Transform opponentPositiveArea;
    public Transform opponentNegativeArea;
    public Transform playerPositiveArea;
    public Transform playerNegativeArea;

    public Transform numberDeckTransform;

    public List<GameObject> numberDeck;



    // Start is called before the first frame update
    void Start()
    {
        DealNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DealNumbers()
    {
        ShuffleNumbers();
        //DealOpponentNumbers();
        StartCoroutine(DealPlayerNumbers());

    }

    IEnumerator DealPlayerNumbers()
    {
        for (int i = 0; i < 4; i++)
        {
            //give each number appropriate tag, then search for tags in scene for conditionals
            //or check each gameobj for stats

            //instantiate the space and then the image
            //the image is the one that is clickable
            //on start instantiate space to parent opponent or player, show image, then move to space
            //will know which by parent 
            //deck of spaces
            if (numberDeck[i].GetComponent<NumberStats>().positive)
            {
                Instantiate(numberDeck[i], playerPositiveArea);
            }
            else if(numberDeck[i].GetComponent<NumberStats>().negative)
            {
                Instantiate(numberDeck[i], playerNegativeArea);
            }

            //Delay between cards
            yield return new WaitForSeconds(1f);
        }
    }

    void DealOpponentNumbers()
    {

    }


    void ShuffleNumbers()
    {
        for (int i = numberDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = numberDeck[i];
            numberDeck[i] = numberDeck[j];
            numberDeck[j] = temp;
        }
    }
}
