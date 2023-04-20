using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//makes a list of prefabs (the deck)
//shuffles the prefabs
//yields list valuePrefabs

public class ValueDeck : MonoBehaviour
{
    public GameObject cardPrefab;

    public List<GameObject> valuePrefabs;
    public int cardIndex;

    public int newIndex;

    public List<GameObject> tempList;
    
    
    
    public void Start()
    {

        //on start, produces a shuffled list of sprites

        for(int i = 0; i < 40; i++)
        {
            GameObject temp = valuePrefabs[i];
            int rand = Random.Range(0,40);
            valuePrefabs[i] = valuePrefabs[rand];
            valuePrefabs[rand] = temp;
        }

        foreach(GameObject i in valuePrefabs)
        {
            tempList.Add(i);
        }
    }

    public GameObject GetList(int n)
    {
        int index = n;
        return valuePrefabs[index];
    }



}
