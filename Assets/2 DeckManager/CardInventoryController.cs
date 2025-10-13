using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInventoryController : MonoBehaviour
{
    public static CardInventoryController instance;

    public List<GameObject> playerDeck;
    public List<GameObject> cardInventory;

    public List<GameObject> rowList;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        

        //FIX ERROR WHEN IN NON BATTLE SCENE
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManageDeck()
    {
        //instantiate buttons for each obj
        int i = rowList.Count;
        int k = -1;
        foreach(GameObject g in cardInventory)
        {
            k++;

            if (!rowList[i-1].activeSelf)
            {
                rowList[i-1].SetActive(true);
            }

            if (k%4 != 0)
            {
                Instantiate(g, rowList[i-1].transform);
            }
            else
            {
                i--;

                if (!rowList[i-1].activeSelf)
                {
                    rowList[i-1].SetActive(true);
                }

                Instantiate(g, rowList[i-1].transform);
            }

            
        }
    }
}
