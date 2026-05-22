using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryController : MonoBehaviour
{
    public static CardInventoryController instance;

    public List<GameObject> playerDeck;
    public List<GameObject> cardInventory;

    public List<GameObject> rowList;

    public List<GameObject> ownedCards; //for debug only

    public GameObject panel;

    public bool reset;

    public bool firstTime = true;

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
        if(reset)
        {
            //reset layout group
            foreach (GameObject row in rowList)
            {
                LayoutGroup layout = row.GetComponent<LayoutGroup>();

                layout.enabled = false;
                layout.enabled = true;
            }

            if(firstTime)
            {
                RectTransform rect = panel.GetComponent<RectTransform>();
                rect.anchoredPosition += new Vector2(0f, 22f);

                firstTime = false;
            }

            reset = false;
        }
    }

    public void ManageDeck()
    {
        StartCoroutine(RebuildDeck());
    }

    IEnumerator RebuildDeck()
    {
        //clear all leftover children
        foreach(GameObject row in rowList)
        {
            foreach(Transform child in row.transform)
            {
                Destroy(child.gameObject);
            }
        }

        yield return null;

        //instantiate buttons for each obj
        int i = rowList.Count;
        int k = -1;
        foreach(GameObject g in cardInventory)
        {
            if(g.GetComponent<DeckCardButton>().owned)
            {
                ownedCards.Add(g); //for debug list

                k++;

                if (!rowList[i - 1].activeSelf)
                {
                    rowList[i - 1].SetActive(true);
                }

                if (k % 4 != 0)
                {
                    Instantiate(g, rowList[i - 1].transform);
                }
                else
                {
                    i--;

                    if (!rowList[i - 1].activeSelf)
                    {
                        rowList[i - 1].SetActive(true);
                    }

                    Instantiate(g, rowList[i - 1].transform);
                }
            }
            

            
        }


        yield return null;
        reset = true;
        
    }
}
