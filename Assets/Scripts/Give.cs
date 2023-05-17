using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Give : MonoBehaviour
{
    //public bool self;
    //public bool opponent;


    public GameObject negFour;
    public GameObject negThree;
    public GameObject negTwo;
    public GameObject two;
    public GameObject three;
    public GameObject four;
    public GameObject five;
    public GameObject six;
    public GameObject seven;
    public GameObject eight;
    public GameObject nine;

    public GameObject duplicatePrefab;

    public GameManager gm;

    public Transform board;

    

    //public bool has7;


    private void Start()
    {
        GameObject manager = GameObject.Find("Game Manager");    
        gm = manager.GetComponent<GameManager>();
        GameObject tempBoard = GameObject.Find("Canvas/Panel");
        board = tempBoard.transform; 
    }

    public void GiveSelf2() //negativeTwo
    {
        GameObject givePrefab = (GameObject) Instantiate (negTwo, board);
        givePrefab.transform.SetParent(board, false);
        gm.playerValues.Add(givePrefab);
        gm.OrganizeCards();
        DestroyMe();
    }

    public void GiveOpp2() //positiveTwo
    {
        GameObject givePrefab = (GameObject) Instantiate (two, board);
        givePrefab.transform.SetParent(board, false);
        //gm.AIValues.Insert(0, givePrefab);
        gm.AIValues.Add(givePrefab);
        gm.OrganizeCards();
        gm.cardsGiven += 1;
        DestroyMe();
        //force sorting order so given card is at very bottom
    }

    public void GiveAll4()
    {
        GameObject givePrefab = (GameObject) Instantiate (four, board);
        GameObject givePrefab2 = (GameObject) Instantiate (four, board);
        //gm.AIValues.Insert(0, givePrefab);
        //gm.playerValues.Insert(0, givePrefab2);
        gm.AIValues.Add(givePrefab);
        givePrefab.transform.SetParent(board, false);
        
        Debug.Log("New object parent: " + givePrefab.transform.parent.name);
        gm.playerValues.Add(givePrefab2);
        givePrefab2.transform.SetParent(board, false);
        gm.OrganizeCards();
        gm.cardsGiven += 1;

        DestroyMe();
    }

    public void Give3()
    {

        GameObject seven = gm.AIValues.Find(obj => obj.GetComponent<ValueCard>().value == 7);
        if(seven != null)
        {
            GameObject givePrefab = (GameObject) Instantiate (three, board);
            givePrefab.transform.SetParent(board, false);
            gm.AIValues.Add(givePrefab);
            gm.OrganizeCards();
            gm.cardsGiven += 1;
        }

        
        DestroyMe();
        /*
        if(has7)
        {
            for(int i = 0; i < gm.AIValues.Count; i++)
            {
                if(gm.AIValues[i].GetComponent<ValueCard>().value == 7)
                {
                    GameObject givePrefab = (GameObject) Instantiate (three);
                    gm.AIValues.Add(givePrefab);
                    gm.OrganizeCards();
                }
            }
        }
        */
        //will do it for every 7

    }

    public void GiveDuplicate()
    {
        // Assuming you have a list of prefabs called myPrefabList and another list called duplicatesList
        List<GameObject> myPrefabList = new List<GameObject>();

        // Finding duplicates using LINQ
        for(int i = 0; i < gm.playerValues.Count; i++)
        {
            myPrefabList.Add(gm.playerValues[i]);
        }
        for(int k = 0; k < gm.tempNegsPlayer.Count; k++)
        {
            myPrefabList.Add(gm.tempNegsPlayer[k]);
        }


        var duplicateGroups = myPrefabList.GroupBy(x => x.GetComponent<ValueCard>().value)
            .Where(g => g.Count() > 1)
            .Select(g => new { Value = g.Key, Objects = g.ToList() });

        // Moving one duplicate to the duplicatesList
        foreach (var group in duplicateGroups)
        {
            // Select the first duplicate and remove it from the myPrefabList
            GameObject duplicatePrefab2 = group.Objects[0];
            myPrefabList.Remove(duplicatePrefab2);
            GameObject duplicatePrefab = (GameObject) Instantiate (duplicatePrefab2, board);

            if(duplicatePrefab.GetComponent<ValueCard>().value > 0)
            {
                gm.playerValues.Remove(group.Objects[0]);
                gm.AIValues.Add(duplicatePrefab);
                duplicatePrefab.transform.SetParent(board, false);
                //gm.AIValues.Insert(gm.AIValues.Count, duplicatePrefab);
                gm.OrganizeCards();
                gm.cardsGiven += 1;
            }
            if(duplicatePrefab.GetComponent<ValueCard>().value < 0)
            {
                gm.tempNegsPlayer.Remove(group.Objects[0]);
                gm.tempNegsAI.Add(group.Objects[0]);
                group.Objects[0].transform.SetParent(board, false);
                gm.OrganizeCards();
                gm.cardsGiven += 1;
            }

            
            
        }
        DestroyMe();
    }
    
    public void ifDiscardGive2()
    {
        if(gm.hasDiscarded == true)
        {
            GiveOpp2();
            gm.cardsGiven += 1;
        }
        DestroyMe();
    }

    void DestroyMe()
    {
        gm.playerSpecials.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
    
}
